using FrostAura.AI.Legion.Consts.Middleware;
using FrostAura.AI.Legion.Interfaces.Data;
using FrostAura.AI.Legion.Interfaces.Managers;
using FrostAura.AI.Legion.Middleware.HTTP;
using FrostAura.AI.Legion.Models.Communication;
using FrostAura.AI.Legion.Services.Data;
using FrostAura.AI.Legion.Services.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FrostAura.AI.Legion.Extensions;

/// <summary>
/// Service collection extensions for bootstrapping the project.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Register required services for the project.
	/// </summary>
	/// <param name="services">The service collection.</param>
	/// <returns>The augmented service collection.</returns>
	public static IServiceCollection AddLegion(this IServiceCollection services)
	{
		return services
			.AddConfiguration()
			.AddTransient<OllamaNormalizationHttpHandler>()
			.AddHttpClient(HttpClientNames.OllamaHttpClient, client =>
			{
				client.Timeout = TimeSpan.FromHours(1);
			})
			.AddHttpMessageHandler<OllamaNormalizationHttpHandler>()
			.Services
			.AddSingleton<ISemanticOrchestrator, LegionOrchestrator>()
			.AddSingleton<IStream<StreamMessage>, InMemoryStream>()
			.AddSingleton<ILargeLanguageModel, OllamaLargeLanguageModel>()
			.AddSingleton<IToolOrchestrator, GatewayToolOrchestrator>()
			.AddLogging(builder =>
			{
				builder.AddConsole();
				builder.SetMinimumLevel(LogLevel.Debug);
			});
	}

	/// <summary>
	/// Register required configuration for the project.
	/// </summary>
	/// <param name="services">The service collection.</param>
	/// <returns>The augmented service collection.</returns>
	private static IServiceCollection AddConfiguration(this IServiceCollection services)
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.AddEnvironmentVariables()
			.Build();

		return services
				.AddSingleton<IConfiguration>(configuration)
				.Configure<object>(configuration.GetSection("TODO: Register actual configuration."));
	}
}
