using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WikimediaProcessor.Runners;

namespace WikimediaProcessor
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "Wikimedia processor";
            Console.WriteLine("Starting the processor.");
            await ShowOptions();
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// This method has to have this signature for EF Core migrations to work.
        /// </summary>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) => Register.ConfigureServices(context, services))
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "hh:mm:ss ";
                    });
                })
                .UseConsoleLifetime();

        private static async Task ShowOptions()
        {
            Console.WriteLine("*****************************");
            Console.WriteLine("Write an option to run");
            Console.WriteLine("[1] Process Wikimedia page views");
            Console.WriteLine("[2] Get reports from page views");
            Console.WriteLine("[0] Exit");
            Console.WriteLine("Enter 1, 2 or 0 and press return.");
            Console.WriteLine("*****************************");

            await RunOption(Console.ReadLine());
            await ShowOptions();
        }

        private static Task RunOption(string selectedOption)
        {
            switch (selectedOption)
            {
                case "0":
                    Environment.Exit(0);
                    return Task.CompletedTask;
                case "1":
                case "2":
                    return RunProcessor(Convert.ToInt32(selectedOption));
                default:
                    Console.WriteLine($"{selectedOption} is not a valid option.");
                    return Task.CompletedTask;
            }
        }

        private static async Task RunProcessor(int option)
        {
            //TODO: DI in .NET Core in console app needs to handle its own scopes
            var builder = CreateHostBuilder(Array.Empty<string>());
            using var host = builder.Build();
            using var scope = host.Services.CreateScope();

            switch (option)
            {
                case 1:
                    var processor = scope.ServiceProvider.GetService<IProcessorRunner>();
                    await processor.RunAsync();
                    break;
                case 2:
                    var reporter = scope.ServiceProvider.GetService<IReportRunner>();
                    reporter.Run();
                    break;
            }
        }
    }
}
