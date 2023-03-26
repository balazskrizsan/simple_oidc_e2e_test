using System.Collections.Generic;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace SimpleOidcE2eTest;

public static class OidcConfig
{
    public const int LIFETIME_10SEC = 10;
    public const int LIFETIME_1MIN = 60;
    public const int LIFETIME_5MINS = 300;
    public const int LIFETIME_1HOUR = 3600;
    public const int LIFETIME_2HOURS = 7200;
    public const int LIFETIME_4HOURS = 14400;

    private static Dictionary<string, string> exchangeMap = new()
    {
        { "xc/test_token_exchange", "test_token_exchange" }
    };

    public static bool IsValidateExchange(string exchangeFrom, string exchangeTo)
    {
        exchangeMap.TryGetValue(exchangeFrom, out string exchangeFromValue);
        if (null == exchangeFromValue)
        {
            return false;
        }

        return exchangeFromValue == exchangeTo;
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
        new ApiScope("xc/test_token_exchange"),
        new ApiScope("test_token_exchange"),
    };

    public static IEnumerable<ApiResource> ApiResources => new[]
    {
        new ApiResource("test_resource_a")
        {
            Scopes = new List<string>
            {
                "test_scope",
                "test_scope.a",
                "test_token_exchange",
                "xc/test_token_exchange",
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
            AccessTokenType = AccessTokenType.Jwt
        },
        new Client
        {
            ClientId = "client2_client_credentials",
            ClientName = "Client2 client credentials",
            AllowAccessTokensViaBrowser = false,
            AllowedGrantTypes = GrantTypes.Code,
            RequireClientSecret = false,
            RequirePkce = false, // go true
            ClientSecrets = { new Secret("client2_client_credentials_secret".Sha256()) },
            RedirectUris = { "https://localhost:4200" },
            PostLogoutRedirectUris = { },
            AllowedCorsOrigins = { "https://localhost:4200" },
            AccessTokenLifetime = LIFETIME_1HOUR,
            RequireConsent = false,
            AllowedScopes =
            {
                "test_scope",
                "test_scope.a",
                "openid",
                "profile",
            },
            EnableLocalLogin = false,
            IdentityProviderRestrictions = { "Facebook" },
            AccessTokenType = AccessTokenType.Reference
        },
        new Client
        {
            ClientId = "client.scope_for_token_exchange",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("client.scope_for_token_exchange".Sha256()) },
            AllowOfflineAccess = true,
            RequireClientSecret = false,
            RedirectUris = { },
            PostLogoutRedirectUris = { },
            RequirePkce = true,
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.LocalApi.ScopeName,
                "xc/test_token_exchange",
            }
        },
        new Client
        {
            ClientId = "client.token_exchange",
            AllowedGrantTypes = new[] { "token_exchange" },
            ClientSecrets = { new Secret("client.token_exchange".Sha256()) },
            AllowOfflineAccess = true,
            RequireClientSecret = false,
            RedirectUris = { },
            PostLogoutRedirectUris = { },
            RequirePkce = true,
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.LocalApi.ScopeName,
                "test_token_exchange",
            }
        }
    };
}