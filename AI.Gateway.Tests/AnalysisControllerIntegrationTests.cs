using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using AI.Gateway.Api.Services;

namespace AI.Gateway.Tests
{
    public class AnalysisControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AnalysisControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private class ThrowingOpenAIResponsesClient : IOpenAIResponsesClient
        {
            public Task<string> AnalyzePromptAsync(string model, string prompt, System.Threading.CancellationToken cancellationToken = default)
            {
                throw new Exception("boom");
            }
        }

        [Fact]
        public async Task Post_WhenAnalyzerThrows_Returns500ProblemDetails()
        {
            // Arrange: replace IOpenAIResponsesClient with a throwing implementation
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove existing registration if present and add our test double
                    services.AddSingleton<IOpenAIResponsesClient, ThrowingOpenAIResponsesClient>();
                });
            }).CreateClient();
            client.BaseAddress = new Uri("https://localhost");

            var req = new { prompt = "this will cause an exception" };

            // Act
            var response = await client.PostAsJsonAsync("/api/analysis", req);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            Assert.True(root.TryGetProperty("title", out var title));
            Assert.Equal("An error occurred while processing the request.", title.GetString());
            Assert.True(root.TryGetProperty("status", out var status));
            Assert.Equal(500, status.GetInt32());

            // Ensure no exception details leaked
            Assert.DoesNotContain("boom", body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("StackTrace", body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Exception", body, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Post_WithEmptyPrompt_Returns400()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
            var req = new { prompt = "" };

            var response = await client.PostAsJsonAsync("/api/analysis", req);
            var body = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.BadRequest)
            {
                Assert.Fail($"Expected 400 but got {response.StatusCode}. Body: {body}");
            }
        }
    }
}
