using System.Threading;
using System.Threading.Tasks;

namespace AI.Gateway.Api.Services
{
    public interface IOpenAIResponsesClient
    {
        Task<string> AnalyzePromptAsync(string model, string prompt, CancellationToken cancellationToken = default);
    }
}
