/*
 * Nuget Dependencies:
 * -------------------
 * dotnet add package FrostAura.AI.Legion
 * 
 * Configuration:
 * --------------
 * appsettings.json or via environment variables (.env file).
 */
using FrostAura.AI.Legion.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

// Create dependency injection service collection (standard practice).
var legion = new ServiceCollection()
	.AddLegion()
	.Replace(ServiceDescriptor.Singleton<IOptions<object>>(sp =>
	{
		return Options.Create(new { });
	}))
	.BuildServiceProvider()
	.GetLegionInstance();

Console.Write("Your Query: ");
var query = "What is the weather like for the next 5 days?";//"What is the meaning of life?";// Console.ReadLine();
Console.WriteLine(Environment.NewLine + await legion.ChatAsync(query, CancellationToken.None));
Console.ReadLine();