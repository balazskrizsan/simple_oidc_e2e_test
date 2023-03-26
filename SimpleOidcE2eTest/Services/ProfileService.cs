using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SimpleOidcE2eTest.Data;

namespace SimpleOidcE2eTest.Services;

public class ProfileService<TUser> : IProfileService
    where TUser : class
{
    protected readonly IUserClaimsPrincipalFactory<TUser> ClaimsFactory;
    protected readonly ILogger<ProfileService<TUser>> Logger;
    protected readonly UserManager<TUser> UserManager;
    protected readonly AppDbContext AppDbContext;

    public ProfileService(
        UserManager<TUser> userManager,
        IUserClaimsPrincipalFactory<TUser> claimsFactory
    )
    {
        UserManager = userManager;
        ClaimsFactory = claimsFactory;
    }

    public ProfileService(
        UserManager<TUser> userManager,
        IUserClaimsPrincipalFactory<TUser> claimsFactory,
        ILogger<ProfileService<TUser>> logger,
        AppDbContext appDbContext
    )
    {
        UserManager = userManager;
        ClaimsFactory = claimsFactory;
        Logger = logger;
        AppDbContext = appDbContext;
    }

    public virtual async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var sub = context.Subject?.GetSubjectId();
        if (sub == null) throw new Exception("No sub claim present");

        await GetProfileDataAsync(context, sub);
    }

    protected virtual async Task GetProfileDataAsync(ProfileDataRequestContext context, string subjectId)
    {
        var user = await FindUserAsync(subjectId);
        if (user != null)
        {
            await GetProfileDataAsync(context, user);
        }
    }

    protected virtual async Task GetProfileDataAsync(ProfileDataRequestContext context, TUser user)
    {
        var principal = await GetUserClaimsAsync(user);
        context.AddRequestedClaims(principal.Claims);
    }

    protected virtual async Task<ClaimsPrincipal> GetUserClaimsAsync(TUser user)
    {
        var principal = await ClaimsFactory.CreateAsync(user);
        if (principal == null) throw new Exception("ClaimsFactory failed to create a principal");

        var identity = principal.Identities.First();

        var url = "";

        identity.AddClaim(new Claim(JwtClaimTypes.Picture, url));

        return principal;
    }

    public virtual async Task IsActiveAsync(IsActiveContext context)
    {
        var sub = context.Subject?.GetSubjectId();
        if (sub == null) throw new Exception("No subject Id claim present");

        await IsActiveAsync(context, sub);
    }

    protected virtual async Task IsActiveAsync(IsActiveContext context, string subjectId)
    {
        var user = await FindUserAsync(subjectId);
        if (user != null)
        {
            await IsActiveAsync(context, user);
        }
        else
        {
            context.IsActive = false;
        }
    }

    protected virtual async Task IsActiveAsync(IsActiveContext context, TUser user)
    {
        context.IsActive = await IsUserActiveAsync(user);
    }

    public virtual Task<bool> IsUserActiveAsync(TUser user)
    {
        return Task.FromResult(true);
    }

    protected virtual async Task<TUser> FindUserAsync(string subjectId)
    {
        if (subjectId.StartsWith("xc/"))
        {
            return new IdentityUser("mock_user") as TUser;
        }

        var user = await UserManager.FindByIdAsync(subjectId);
        if (user == null)
        {
            Logger?.LogWarning("No user found matching subject Id: {subjectId}", subjectId);
        }

        return user;
    }
}