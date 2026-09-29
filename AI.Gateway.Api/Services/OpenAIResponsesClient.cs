using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenAI.Responses;

namespace AI.Gateway.Api.Services
{
    public class OpenAIResponsesClient : IOpenAIResponsesClient
    {
        #pragma warning disable OPENAI001
        private readonly ResponsesClient _client;

        // JSON Schema used for structured outputs (kept inline content unchanged)
        private const string PromptAnalysisJsonSchema = @"{
  ""type"": ""json_schema"",
  ""name"": ""prompt_analysis"",
  ""schema"": {
    ""type"": ""object"",
    ""additionalProperties"": false,
    ""properties"": {
      ""completeness"": {
        ""type"": ""array"",
        ""items"": {
          ""type"": ""object"",
          ""additionalProperties"": false,
          ""properties"": {
            ""observation"": { ""type"": ""string"" },
            ""impact"": { ""type"": ""string"" }
          },
          ""required"": [ ""observation"", ""impact"" ]
        }
      },
      ""assumptions"": {
        ""type"": ""array"",
        ""items"": {
          ""type"": ""object"",
          ""additionalProperties"": false,
          ""properties"": {
            ""observation"": { ""type"": ""string"" },
            ""impact"": { ""type"": ""string"" }
          },
          ""required"": [ ""observation"", ""impact"" ]
        }
      }
    },
    ""required"": [ ""completeness"", ""assumptions"" ]
  },
  ""strict"": true
}";

        public OpenAIResponsesClient(ResponsesClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<string> AnalyzePromptAsync(string model, string prompt, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(model)) throw new ArgumentException("model is required", nameof(model));
            if (prompt is null) throw new ArgumentNullException(nameof(prompt));

            var reqOptions = new CreateResponseOptions { Model = model };
            reqOptions.InputItems.Add(ResponseItem.CreateUserMessageItem(prompt));



            #pragma warning disable SCME0001
            reqOptions.Patch.Set(Encoding.UTF8.GetBytes("$.text.format"), BinaryData.FromString(PromptAnalysisJsonSchema));

            var clientResult = await _client.CreateResponseAsync(reqOptions, cancellationToken).ConfigureAwait(false);
            #pragma warning restore SCME0001

            var responseResult = clientResult.Value;
            #pragma warning restore OPENAI001
            return responseResult.GetOutputText();
        }
    }
}
