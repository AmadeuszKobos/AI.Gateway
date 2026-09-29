using AI.Gateway.Api.Models;
using System.Threading;
using System.Threading.Tasks;

namespace AI.Gateway.Api.Services
{
    public interface IPromptAnalyzer
    {
        Task<AnalysisResponse> AnalyzeAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
