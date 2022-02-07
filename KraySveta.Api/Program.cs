using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using KraySveta.Api.DataLayer.Options;
using LightInject;
using LightInject.Microsoft.AspNetCore.Hosting;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KraySveta.Api;

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

        await CreateHostBuilder(args, container)
            .Build()
            .RunAsync(cancellationTokenSource.Token);
    }

    private static IHostBuilder CreateHostBuilder(string[] args, IServiceContainer container) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new LightInjectServiceProviderFactory(container))
            .ConfigureServices((hostContext, serviceCollection) =>
            {
                serviceCollection.AddOptions();
                serviceCollection.Configure<MongoDbConfiguration>(
                    hostContext.Configuration.GetSection(MongoDbConfiguration.ConfigPath));
            })
            .ConfigureLogging(builder =>
            {
                builder.AddConsole();
                builder.AddFile($"Logs/{Assembly.GetExecutingAssembly().GetName().Name}.{{Date}}.log");
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