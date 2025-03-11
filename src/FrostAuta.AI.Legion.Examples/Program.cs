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

// Create dependency injection service collection (standard practice).
var legion = new ServiceCollection()
	.AddLegion()
	.BuildServiceProvider()
	.GetLegionInstance();

Console.Write("Your Query: ");
var query = "What is the meaning of life?";// Console.ReadLine();
Console.WriteLine(await legion.ChatAsync(query, CancellationToken.None));
Console.ReadLine();