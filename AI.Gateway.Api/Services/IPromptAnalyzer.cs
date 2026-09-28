using AI.Gateway.Api.Models;

namespace AI.Gateway.Api.Services
{
    public interface IPromptAnalyzer
    {
        AnalysisResponse Analyze(string prompt);
    }
}
