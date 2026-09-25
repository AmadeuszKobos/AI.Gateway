using Xunit;
using AI.Gateway.Api.Controllers;
using AI.Gateway.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace AI.Gateway.Tests
{
    public class AnalysisControllerTests
    {
        [Fact]
        public void Post_WithKnownKeywords_ReturnsFindingsAndAssumptions()
        {
            // Arrange
            var controller = new AnalysisController();
            var request = new AnalysisRequest { Prompt = "Implement auth and database for feature X" };

            // Act
            var actionResult = controller.Post(request);

            // Assert
            Assert.NotNull(actionResult);
            // Successful path returns the response as Value
            Assert.Null(actionResult.Result);
            var response = actionResult.Value;
            Assert.NotNull(response);
            Assert.NotEmpty(response.Completeness);
            Assert.NotEmpty(response.Assumptions);
            foreach (var f in response.Completeness)
            {
                Assert.False(string.IsNullOrWhiteSpace(f.Observation));
                Assert.False(string.IsNullOrWhiteSpace(f.Impact));
            }
            foreach (var a in response.Assumptions)
            {
                Assert.False(string.IsNullOrWhiteSpace(a.Observation));
                Assert.False(string.IsNullOrWhiteSpace(a.Impact));
            }
        }

        [Fact]
        public void Post_WithEmptyPrompt_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AnalysisController();
            var request = new AnalysisRequest { Prompt = "" };

            // Act
            var actionResult = controller.Post(request);

            // Assert
            Assert.NotNull(actionResult);
            // Expect a BadRequest result
            Assert.NotNull(actionResult.Result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var bad = actionResult.Result as BadRequestObjectResult;
            Assert.NotNull(bad);
            Assert.NotNull(bad.Value);
        }
    }
}
