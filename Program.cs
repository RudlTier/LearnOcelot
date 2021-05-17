using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ocelot.Middleware;
using System.IdentityModel.Tokens.Jwt;

namespace OcelotBasic
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Needed if you want to access the sub claim of jwt
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
                    .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                    .AddJsonFile("ocelot.json")
                    .AddEnvironmentVariables();
            })
            .ConfigureLogging(logging =>
            {
                logging.AddConsole();
            }
            )
            .ConfigureServices(s =>
            {
                var authenticationProviderKey = "TestKey";
                s.AddLogging();
                s.AddAuthentication()
                .AddJwtBearer(authenticationProviderKey, x =>
                {
                    x.Authority = "http://192.168.178.26:8080/auth/realms/master";
                    x.RequireHttpsMetadata = false;
                    x.Audience = "Test";
                });
                s.AddOcelot();
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                //add your logging
            })
            .UseIISIntegration()
            .Configure(app =>
            {
                app.UseOcelot().Wait();
            })
            .Build()
            .Run();
        }
    }
}
