using System.Net;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace S3_ForwardStream
{
    public class Program
    {
        private static IConfiguration Configuration { get; } = GetConfiguration();
        private static IConfiguration GetConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", true)
                .AddEnvironmentVariables();
            return builder.Build();
        }

        public static void Main(string[] args)
        {

            //Initialize Logger early
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                //How the application and release properties should be set for dynamic setting
                //.Enrich.WithProperty("Application", typeof(Program).Assembly.GetName().Name)
                //.Enrich.WithProperty("Version", typeof(Program).Assembly.GetName().Version)
                .Enrich.WithProperty("host", "a host")
                .Destructure.AsScalar<IPAddress>()
                .Destructure.ByTransforming<byte[]>(v => $"<ignored {v.Length}>")
                .Destructure.ByTransforming<Stream>(v => "<hidden>")
                .CreateLogger();

            try
            {
                Log.Information("Application starting...");
                CreateHostBuilder(args).Build().Run();
                Log.Information("Application terminating...");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The Application failed to start.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            var builder = WebApplication.CreateBuilder(args);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // Configure Serilog to be the default loggerprovider,
                // use global logger configured previously by default
                .UseSerilog()
                .ConfigureAppConfiguration(
                    configurationBuilder => configurationBuilder.AddConfiguration(Configuration))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}