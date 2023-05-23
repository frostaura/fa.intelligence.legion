using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FrostAura.Intelligence.Iluvatar.Shared.Models.Config;
using FrostAura.Intelligence.Iluvatar.Telegram.Managers;
using FrostAura.Intelligence.Iluvatar.Shared.Extensions;

var tokenSource = new CancellationTokenSource();
var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// Bind configuration.
services
    .AddTelegramConfiguration(configuration)
    .AddSharedConfiguration(configuration)
    .AddCoreConfiguration(configuration);

// Register dependencies.
services
    .AddSharedServices(configuration)
    .AddCoreServices(configuration)
    .AddTelegramServices(configuration);

// Initialize the host.
var serviceProvider = services.BuildServiceProvider();
var host = serviceProvider.GetRequiredService<TelegramManager>();

try
{
    using (host)
    {
        await host.RunAsync(tokenSource.Token);

        Console.ReadLine();
        Console.WriteLine("Shutting down the host.");
    }

}
catch (Exception ex)
{
    tokenSource.Cancel();
}