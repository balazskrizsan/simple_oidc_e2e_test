using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleOidcE2eTest.Data;
using SimpleOidcE2eTest.Services;

namespace SimpleOidcE2eTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    services.AddScoped<IExtensionGrantValidator, TokenExchangeGrantValidatorService>();
                    services.AddDbContext<AppDbContext>(AppConfigService.ConfigDbContext);
                    services.AddIdentity<IdentityUser, IdentityRole>()
                        .AddEntityFrameworkStores<AppDbContext>()
                        .AddDefaultTokenProviders();

                    services.AddIdentityServer(options =>
                        {
                            options.Events.RaiseErrorEvents = true;
                            options.Events.RaiseInformationEvents = true;
                            options.Events.RaiseFailureEvents = true;
                            options.Events.RaiseSuccessEvents = true;
                            options.EmitStaticAudienceClaim = true;
                            options.IssuerUri = "e2e.test";
                        })
                        .AddAspNetIdentity<IdentityUser>()
                        .AddConfigurationStore(AppConfigService.ConfigConfigurationStore)
                        .AddOperationalStore(AppConfigService.ConfigOperationalStore)
                        .AddInMemoryClients(OidcConfig.Clients)
                        .AddInMemoryApiResources(OidcConfig.ApiResources)
                        .AddInMemoryApiScopes(OidcConfig.ApiScopes)
                        .AddInMemoryIdentityResources(OidcConfig.IdentityResources)
                        .AddProfileService<ProfileService<IdentityUser>>();
                    ;
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}