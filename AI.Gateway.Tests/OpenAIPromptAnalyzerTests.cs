using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using AI.Gateway.Api.Services;
using AI.Gateway.Api.Models;
using System.Collections.Generic;

namespace AI.Gateway.Tests
{
    public class OpenAIPromptAnalyzerTests
    {
        private IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenAI:Model"] = "test-model"
            }).Build();
        }

        [Fact]
        public async Task AnalyzeAsync_ValidJson_ReturnsAnalysisResponse()
        {
            var validJson = "{\"completeness\": [{\"observation\": \"obs1\", \"impact\": \"imp1\"}], \"assumptions\": [{\"observation\": \"ass1\", \"impact\": \"impA\"}]}";
            var fakeClient = new FakeOpenAIResponsesClient(validJson);
            var analyzer = new OpenAIPromptAnalyzer(fakeClient, GetConfiguration());

            var result = await analyzer.AnalyzeAsync("prompt text");

            Assert.NotNull(result);
            Assert.Single(result.Completeness);
            Assert.Single(result.Assumptions);
            Assert.Equal("obs1", result.Completeness[0].Observation);
            Assert.Equal("imp1", result.Completeness[0].Impact);
            Assert.Equal("ass1", result.Assumptions[0].Observation);
        }

        [Fact]
        public async Task AnalyzeAsync_MalformedJson_ThrowsAiServiceException_WithInnerJsonException()
        {
            var badJson = "this is not valid json";
            var fakeClient = new FakeOpenAIResponsesClient(badJson);
            var analyzer = new OpenAIPromptAnalyzer(fakeClient, GetConfiguration());

            var ex = await Assert.ThrowsAsync<AiServiceException>(async () => await analyzer.AnalyzeAsync("prompt text"));
            Assert.Equal(AiServiceErrorKind.InvalidResponse, ex.Kind);
            Assert.IsType<System.Text.Json.JsonException>(ex.InnerException);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task AnalyzeAsync_EmptyOrWhitespaceOutput_ThrowsInvalidResponse(string output)
        {
            var fakeClient = new FakeOpenAIResponsesClient(output);
            var analyzer = new OpenAIPromptAnalyzer(fakeClient, GetConfiguration());

            var ex = await Assert.ThrowsAsync<AiServiceException>(
                () => analyzer.AnalyzeAsync("prompt"));

            Assert.Equal(AiServiceErrorKind.InvalidResponse, ex.Kind);
        }

        [Fact]
        public async Task AnalyzeAsync_NullJsonLiteral_ThrowsInvalidResponse()
        {
            var fakeClient = new FakeOpenAIResponsesClient("null");
            var analyzer = new OpenAIPromptAnalyzer(fakeClient, GetConfiguration());

            var ex = await Assert.ThrowsAsync<AiServiceException>(() => analyzer.AnalyzeAsync("prompt"));
            Assert.Equal(AiServiceErrorKind.InvalidResponse, ex.Kind);
        }

        private class FakeOpenAIResponsesClient : IOpenAIResponsesClient
        {
            private readonly string _response;

            public FakeOpenAIResponsesClient(string response) => _response = response;

            public Task<string> AnalyzePromptAsync(string model, string prompt, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(_response);
            }
        }
    }
}
