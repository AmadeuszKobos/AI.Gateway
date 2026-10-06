using AI.Gateway.Api.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AI.Gateway.Api.Services;

namespace AI.Gateway.Api.Services
{
    public class OpenAIPromptAnalyzer : IPromptAnalyzer
    {
        private readonly IOpenAIResponsesClient _openAiClient;
        private readonly string _model;

        public OpenAIPromptAnalyzer(IOpenAIResponsesClient openAiClient, IConfiguration configuration)
        {
            _openAiClient = openAiClient ?? throw new ArgumentNullException(nameof(openAiClient));
            _model = configuration["OpenAI:Model"] ?? throw new ArgumentException("Configuration key 'OpenAI:Model' is required and was not found.", "OpenAI:Model");
        }

        public async Task<AnalysisResponse> AnalyzeAsync(string prompt, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt)) throw new ArgumentException("Prompt is required", nameof(prompt));

            // Keep prompt as raw user input and delegate schema handling to the adapter
            var userInput = prompt;

            // Delegate request building and call to the adapter which hides SDK types
            string jsonText = await _openAiClient.AnalyzePromptAsync(_model, userInput, cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(jsonText))
            {
                throw new AiServiceException(AiServiceErrorKind.InvalidResponse, "AI provider returned empty response.");
            }

            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            AnalysisResponse? analysis;
            try
            {
                analysis = JsonSerializer.Deserialize<AnalysisResponse>(jsonText, jsonOptions);
            }
            catch (JsonException jex)
            {
                throw new AiServiceException(AiServiceErrorKind.InvalidResponse, "Failed to parse AI provider JSON response.", jex);
            }

            if (analysis == null)
            {
                throw new AiServiceException(AiServiceErrorKind.InvalidResponse, "AI provider JSON deserialized to null.");
            }

            return analysis;
        }
    }
}
