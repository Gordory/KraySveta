using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KraySveta.Core;
using LightInject;
using LightInject.Microsoft.AspNetCore.Hosting;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KraySveta.App
{
    internal static class Program
    {
        internal static async Task Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellationTokenSource.Cancel(true);
            };

            var container = CreateContainer();
            container.RegisterInstance(cancellationTokenSource);
            container.RegisterInstance(cancellationTokenSource.Token);

            var hostTask = CreateHostBuilder(args, container)
                .Build()
                .RunAsync(cancellationTokenSource.Token);

            var daemonsTasks = container
                .GetAllInstances<IDaemon>()
                .Select(daemon => daemon.RunAsync(cancellationTokenSource.Token));

            var allTasks = daemonsTasks.Append(hostTask).ToArray();
            await Task.WhenAny(allTasks);
            cancellationTokenSource.Cancel();

            await Task.WhenAll(allTasks);
            
        }

        private static IHostBuilder CreateHostBuilder(string[] args, IServiceContainer container) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new LightInjectServiceProviderFactory(container))
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole();
                    builder.AddFile("Logs/KraySveta.App.{Date}.log");
                })
                .ConfigureWebHostDefaults(webBuilder => 
                {
                    webBuilder
                        .UseLightInject(container)
                        .UseStartup<Startup>(); 
                });

        private static IServiceContainer CreateContainer()
        {
            return new ServiceContainer(ContainerOptions.Default);
        }
    }
}