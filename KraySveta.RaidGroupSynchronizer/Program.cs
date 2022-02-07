using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using KraySveta.External.Discord.Configuration;
using KraySveta.External.ThatsMyBis;
using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KraySveta.RaidGroupSynchronizer;

internal static class Program
{
    private static async Task Main(string[] args)
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

        container.RegisterFrom<CompositionRoot>();

        await CreateHostBuilder(args, container)
            .Build()
            .RunAsync(cancellationTokenSource.Token);
    }

    private static IHostBuilder CreateHostBuilder(string[] args, IServiceContainer container) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new LightInjectServiceProviderFactory(container))
            .ConfigureServices((hostContext, serviceCollection) =>
            {
                serviceCollection.AddHostedService<Daemon>();
                serviceCollection.AddOptions();
                serviceCollection.Configure<ThatsMyBisConfiguration>(
                    hostContext.Configuration.GetSection(ThatsMyBisConfiguration.ConfigPath));
                serviceCollection.Configure<DiscordServerConfiguration>(
                    hostContext.Configuration.GetSection(DiscordServerConfiguration.ConfigPath));
                serviceCollection.Configure<DiscordBotConfiguration>(
                    hostContext.Configuration.GetSection(DiscordBotConfiguration.ConfigPath));
                serviceCollection.Configure<DaemonConfig>(
                    hostContext.Configuration.GetSection(DaemonConfig.ConfigPath));
            })
            .ConfigureLogging(builder =>
            {
                builder.AddConsole();
                builder.AddFile($"Logs/{Assembly.GetExecutingAssembly().GetName().Name}.{{Date}}.log");
            });

    private static IServiceContainer CreateContainer()
    {
        return new ServiceContainer(ContainerOptions.Default);
    }
}