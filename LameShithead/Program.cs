using GamesClient.Shithead;
using LameShithead;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

try
{
    var hostBuilder = Host.CreateDefaultBuilder();

    hostBuilder.ConfigureServices((ctx, services) =>
    {
        services.AddShitheadClient();

        services.AddHostedService<ShitheadGameManager>();
        services.AddLogging();
    });

    var host = hostBuilder.Build();

    await host.StartAsync();
    return 0;
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex);

    return -1;
}
