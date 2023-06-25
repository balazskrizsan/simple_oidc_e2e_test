using System;
using System.Reflection;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace SimpleOidcE2eTest.Services;

public static class AppConfigService
{
    public static void ConfigDbContext(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(GetConnectionString());
    }

    public static void ConfigConfigurationStore(ConfigurationStoreOptions options)
    {
        options.ConfigureDbContext = b => b.UseNpgsql(
            GetConnectionString(),
            sql => sql.MigrationsAssembly(GetMigrationsAssembly())
        );
    }
    
    public static void ConfigOperationalStore(OperationalStoreOptions options)
    {
        options.ConfigureDbContext = b => b.UseNpgsql(GetConnectionString(),
            sql => sql.MigrationsAssembly(GetMigrationsAssembly())
        );
    }
    
    private static string GetConnectionString()
    {
        Console.WriteLine(AppSettingsService.Get()["psqlConnectionString"]);

        return AppSettingsService.Get()["psqlConnectionString"];
    }

    private static string GetMigrationsAssembly()
    {
        return typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
    }
}