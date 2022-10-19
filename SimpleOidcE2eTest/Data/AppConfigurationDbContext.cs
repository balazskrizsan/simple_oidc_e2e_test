using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace SimpleOidcE2eTest.Data;

public class AppConfigurationDbContext : DbContext
{
    public AppConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options) : base(options)
    {
    }
}