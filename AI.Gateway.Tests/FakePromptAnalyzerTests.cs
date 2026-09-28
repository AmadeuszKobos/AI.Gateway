using Xunit;
using AI.Gateway.Api.Services;
using AI.Gateway.Api.Models;

namespace AI.Gateway.Tests
{
    public class FakePromptAnalyzerTests
    {
        [Fact]
        public void Analyze_WithKeywords_ReturnsFindingsAndAssumptions()
        {
            // Arrange
            var analyzer = new FakePromptAnalyzer();
            var prompt = "This feature needs auth and database integration";

            // Act
            var result = analyzer.Analyze(prompt);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Completeness);
            Assert.NotEmpty(result.Assumptions);

            // Verify at least one concrete deterministic mapping from the fake analyzer
            Assert.Contains(result.Completeness, f => f.Observation == "Authentication/authorization not defined");
            Assert.Contains(result.Completeness, f => f.Observation == "Persistence requirements unspecified");

            Assert.Contains(result.Assumptions, a => a.Observation == "Assume token-based auth");
            Assert.Contains(result.Assumptions, a => a.Observation == "Assume relational DB acceptable");

            foreach (var f in result.Completeness)
            {
                Assert.False(string.IsNullOrWhiteSpace(f.Observation));
                Assert.False(string.IsNullOrWhiteSpace(f.Impact));
            }
            foreach (var a in result.Assumptions)
            {
                Assert.False(string.IsNullOrWhiteSpace(a.Observation));
                Assert.False(string.IsNullOrWhiteSpace(a.Impact));
            }
        }

        [Fact]
        public void Analyze_WithNoKeywords_ReturnsFallbackFindings()
        {
            // Arrange
            var analyzer = new FakePromptAnalyzer();
            var prompt = "Do something unspecified";

            // Act
            var result = analyzer.Analyze(prompt);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Completeness);
            Assert.Single(result.Assumptions);
            Assert.Equal("Acceptance criteria missing", result.Completeness[0].Observation);
            Assert.Equal("Assume no external services required", result.Assumptions[0].Observation);
        }
    }
}
