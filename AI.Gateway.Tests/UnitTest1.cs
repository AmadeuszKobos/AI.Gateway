using Xunit;
using AI.Gateway.Api.Controllers;
using AI.Gateway.Api.Models;
using AI.Gateway.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.Gateway.Tests
{
    public class AnalysisControllerTests
    {
        private class AnalyzerStub : IPromptAnalyzer
        {
            private readonly AnalysisResponse _response;

            public AnalyzerStub(AnalysisResponse response)
            {
                _response = response;
            }

            public AnalysisResponse Analyze(string prompt)
            {
                return _response;
            }
        }

        [Fact]
        public void Post_WithValidPrompt_ReturnsAnalyzerResponse()
        {
            // Arrange - create a deterministic response the stub will return
            var response = new AnalysisResponse();
            response.Completeness.Add(new Finding { Observation = "obs", Impact = "imp" });
            response.Assumptions.Add(new Finding { Observation = "ass", Impact = "imp" });

            var stub = new AnalyzerStub(response);
            var controller = new AnalysisController(stub);
            var request = new AnalysisRequest { Prompt = "Implement auth and database for feature X" };

            // Act
            var actionResult = controller.Post(request);

            // Assert
            Assert.NotNull(actionResult);
            Assert.Null(actionResult.Result);
            var actual = actionResult.Value;
            Assert.Same(response, actual);
        }

        [Fact]
        public void Post_WithEmptyPrompt_ReturnsBadRequest()
        {
            // Arrange
            var stub = new AnalyzerStub(new AnalysisResponse());
            var controller = new AnalysisController(stub);
            var request = new AnalysisRequest { Prompt = "" };

            // Act
            var actionResult = controller.Post(request);

            // Assert
            Assert.NotNull(actionResult);
            Assert.NotNull(actionResult.Result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var bad = actionResult.Result as BadRequestObjectResult;
            Assert.NotNull(bad);
            Assert.NotNull(bad.Value);
        }
    }
}
