using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace SimpleOidcE2eTest.Data;

public class AppPersistedGrantDbContext : DbContext
{
    public AppPersistedGrantDbContext(DbContextOptions<PersistedGrantDbContext> options) : base(options)
    {
    }
}