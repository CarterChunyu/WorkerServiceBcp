using Coravel;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Diagnostics;

namespace WorkerServiceBcp
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.MSSqlServer(
                    connectionString: configuration.GetConnectionString("MyConnStr"),
                    sinkOptions: new MSSqlServerSinkOptions { TableName = "Log" }
                )
                .Enrich.FromLogContext()
                .CreateLogger();

            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                Debug.Print(msg);
                Debugger.Break();
            });

            try
            {
                var separator = new string('-', 30);
                Log.Information($"{separator} Starting host {separator}");

                var builder = Host.CreateApplicationBuilder(args);
                //builder.Services.AddHostedService<Worker>();
                builder.Services.AddTransient<SchedualWork>();
                builder.Services.AddScheduler();
                builder.Services.AddSerilog();
                var host = builder.Build();
                host.Services.UseScheduler(schedule =>
                {
                    int hour = 14;
                    int minute = 0;
                    schedule.Schedule<SchedualWork>().DailyAt(hour - 8 < 0 ? hour + 16 : hour - 8, minute);
                });
                host.Run();

                Log.Information($"{separator} Exit host {separator}");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}