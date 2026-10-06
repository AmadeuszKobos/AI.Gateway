using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using AI.Gateway.Api;
using AI.Gateway.Api.Services;

namespace AI.Gateway.Tests
{
    public class GlobalExceptionHandlerTests
    {
        private class FakeProblemDetailsService : IProblemDetailsService
        {
            public System.Threading.Tasks.ValueTask WriteAsync(ProblemDetailsContext problemDetailsContext)
            {
                var pd = problemDetailsContext.ProblemDetails;
                var ctx = problemDetailsContext.HttpContext;
                ctx.Response.StatusCode = pd.Status ?? 500;
                ctx.Response.ContentType = "application/problem+json";

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };

                var json = JsonSerializer.Serialize(pd, options);
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                ctx.Response.Body.Write(bytes, 0, bytes.Length);
                return default;
            }
        }

        [Fact]
        public async Task TryHandleAsync_AiServiceException_UpstreamFailure_Returns502()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<IProblemDetailsService, FakeProblemDetailsService>();
            var provider = services.BuildServiceProvider();

            var context = new DefaultHttpContext();
            context.Request.Path = "/api/analysis";
            context.RequestServices = provider;
            var mem = new MemoryStream();
            context.Response.Body = mem;

            var handler = new GlobalExceptionHandler(provider.GetRequiredService<IProblemDetailsService>());

            // Act
            var handled = await handler.TryHandleAsync(context, new AiServiceException(AiServiceErrorKind.UpstreamFailure, "upstream"), CancellationToken.None);

            // Assert
            Assert.True(handled);
            Assert.Equal(502, context.Response.StatusCode);
            Assert.Equal("application/problem+json", context.Response.ContentType);

            mem.Position = 0;
            using var reader = new StreamReader(mem);
            var body = await reader.ReadToEndAsync();
            Assert.False(string.IsNullOrWhiteSpace(body));

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            Assert.True(root.TryGetProperty("title", out var title));
            Assert.Equal("An error occurred while processing the request.", title.GetString());
            Assert.True(root.TryGetProperty("status", out var status));
            Assert.Equal(502, status.GetInt32());

            // Ensure no provider or exception details leaked
            Assert.DoesNotContain("upstream", body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("StackTrace", body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Exception", body, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task TryHandleAsync_AiServiceException_InvalidResponse_Returns502()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<IProblemDetailsService, FakeProblemDetailsService>();
            var provider = services.BuildServiceProvider();

            var context = new DefaultHttpContext();
            context.Request.Path = "/api/analysis";
            context.RequestServices = provider;
            var mem = new MemoryStream();
            context.Response.Body = mem;

            var handler = new GlobalExceptionHandler(provider.GetRequiredService<IProblemDetailsService>());

            // Act
            var handled = await handler.TryHandleAsync(context, new AiServiceException(AiServiceErrorKind.InvalidResponse, "invalid"), CancellationToken.None);

            // Assert
            Assert.True(handled);
            Assert.Equal(502, context.Response.StatusCode);
            Assert.Equal("application/problem+json", context.Response.ContentType);

            mem.Position = 0;
            using var reader = new StreamReader(mem);
            var body = await reader.ReadToEndAsync();
            Assert.False(string.IsNullOrWhiteSpace(body));

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            Assert.True(root.TryGetProperty("title", out var title));
            Assert.Equal("An error occurred while processing the request.", title.GetString());
            Assert.True(root.TryGetProperty("status", out var status));
            Assert.Equal(502, status.GetInt32());

            // Ensure no provider or exception details leaked
            Assert.DoesNotContain("invalid", body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("StackTrace", body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Exception", body, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task TryHandleAsync_WritesSafeProblemDetails()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<IProblemDetailsService, FakeProblemDetailsService>();
            var provider = services.BuildServiceProvider();

            var context = new DefaultHttpContext();
            context.Request.Path = "/api/analysis";
            context.RequestServices = provider;
            var mem = new MemoryStream();
            context.Response.Body = mem;

            var handler = new GlobalExceptionHandler(provider.GetRequiredService<IProblemDetailsService>());

            // Act
            var handled = await handler.TryHandleAsync(context, new Exception("boom"), CancellationToken.None);

            // Assert
            Assert.True(handled);
            Assert.Equal(500, context.Response.StatusCode);
            Assert.Equal("application/problem+json", context.Response.ContentType);

            mem.Position = 0;
            using var reader = new StreamReader(mem);
            var body = await reader.ReadToEndAsync();
            Assert.False(string.IsNullOrWhiteSpace(body));

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            Assert.True(root.TryGetProperty("title", out var title));
            Assert.Equal("An error occurred while processing the request.", title.GetString());
            Assert.True(root.TryGetProperty("status", out var status));
            Assert.Equal(500, status.GetInt32());
            Assert.True(root.TryGetProperty("instance", out var instance));
            Assert.Equal("/api/analysis", instance.GetString());

            // Ensure no exception details leaked
            Assert.DoesNotContain("boom", body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("StackTrace", body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Exception", body, StringComparison.OrdinalIgnoreCase);
        }
    }
}
