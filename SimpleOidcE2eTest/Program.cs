using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleOidcE2eTest.Data;

namespace SimpleOidcE2eTest
{
    public class Program
    {
        private static string CONNECTION_STRING =
            "Host=192.168.33.10;Database=stackjudge;Port=54322;Username=admin;Password=admin_pass;";

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
                    services.AddDbContext<AppDbContext>(options => { options.UseNpgsql(CONNECTION_STRING); });
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
                        })
                        .AddAspNetIdentity<IdentityUser>()
                        .AddInMemoryClients(Config.Clients)
                        .AddInMemoryApiResources(Config.ApiResources)
                        .AddInMemoryApiScopes(Config.ApiScopes)
                        .AddInMemoryIdentityResources(Config.IdentityResources);
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}