using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityModel;

namespace SimpleOidcE2eTest;

public static class Config
{
    public const int LIFETIME_10SEC = 10;
    public const int LIFETIME_1MIN = 60;
    public const int LIFETIME_5MINS = 300;
    public const int LIFETIME_1HOUR = 3600;
    public const int LIFETIME_2HOURS = 7200;
    public const int LIFETIME_4HOURS = 14400;

    public static List<TestUser> Users
    {
        get
        {
            var address = new
            {
                street_address = "One Hacker Way",
                locality = "Heidelberg",
                postal_code = 69118,
                country = "Germany"
            };

            return new List<TestUser>
            {
                new()
                {
                    SubjectId = "818727",
                    Username = "alice",
                    Password = "alice",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.Role, "admin"),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                            IdentityServerConstants.ClaimValueTypes.Json)
                    }
                },
                new()
                {
                    SubjectId = "88421113",
                    Username = "bob",
                    Password = "bob",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.Role, "user"),
                        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                            IdentityServerConstants.ClaimValueTypes.Json)
                    }
                }
            };
        }
    }

    public static IEnumerable<IdentityResource> IdentityResources => new[]
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResource
        {
            Name = "role",
            UserClaims = new List<string> { "role" }
        }
    };

    public static IEnumerable<ApiScope> ApiScopes => new[]
    {
        new ApiScope("test_scope"),
        new ApiScope("test_scope.a"),
        new ApiScope("test_scope.b"),
    };

    public static IEnumerable<ApiResource> ApiResources => new[]
    {
        new ApiResource("test_resource_a")
        {
            Scopes = new List<string>
            {
                "test_scope",
                "test_scope.a",
            },
            ApiSecrets = new List<Secret> { new("test_resource_a_secret".Sha256()) },
        }
    };

    public static IEnumerable<Client> Clients => new[]
    {
        new Client
        {
            ClientId = "client1_client_credentials",
            ClientName = "Client1 client credentials",
            AllowAccessTokensViaBrowser = false,
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("client1_client_credentials_secret".Sha256()) },
            AccessTokenLifetime = LIFETIME_1HOUR,
            AllowedScopes =
            {
                "test_scope",
                "test_scope.a"
            },
        }
    };
}