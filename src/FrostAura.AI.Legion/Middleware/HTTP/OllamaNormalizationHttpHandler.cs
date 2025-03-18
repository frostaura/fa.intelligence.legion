using System.Text.Json;
using System.Text.Json.Nodes;

namespace FrostAura.AI.Legion.Middleware.HTTP;

/// <summary>
/// Middleware for normalizing the request payload for Ollama calls to support tools appropriately.
/// </summary>
public class OllamaNormalizationHttpHandler : DelegatingHandler
{
	/// <summary>
	/// An interceptor for HTTP requests.
	/// </summary>
	/// <param name="request">HTTP request.</param>
	/// <param name="cancellationToken">Token to cancel downstream operations.</param>
	/// <returns>The HTTP response.</returns>
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var newRequest = request;

		if (request.Content != null)
		{
			var reqBody = await request.Content.ReadAsStringAsync();
			var root = JsonNode.Parse(reqBody);
			var tools = root["tools"]?.AsArray();

			if (tools != null)
			{
				foreach (var tool in tools)
				{
					var function = tool?["function"];

					if (function != null)
					{
						// Parse the parameters string into an object
						var parametersJson = function["parameters"]?.GetValue<string>();
						var parsedParameters = JsonNode.Parse(parametersJson);
						var propertiesJson = parsedParameters["properties"]?.ToString();

						if (propertiesJson != null)
						{
							// Parse the "properties" JSON string into an object
							var parsedProperties = JsonNode.Parse(propertiesJson);

							// Replace the "properties" field in parsedParameters
							parsedParameters["properties"] = parsedProperties;
						}

						function["parameters"] = parsedParameters;
					}
				}
			}

			var newRequestJsonStr = root.ToJsonString(new JsonSerializerOptions { WriteIndented = true });

			newRequest = new HttpRequestMessage(request.Method, request.RequestUri)
			{
				Content = new StringContent(newRequestJsonStr)
			};

			foreach (var header in request.Headers)
			{
				newRequest.Headers.Add(header.Key, header.Value);
			}

			Console.WriteLine(reqBody);
		}

		return await base.SendAsync(newRequest, cancellationToken);
	}
}
