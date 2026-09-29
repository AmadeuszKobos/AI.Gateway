using Microsoft.AspNetCore.Mvc;
using AI.Gateway.Api.Models;
using System.Collections.Generic;
using AI.Gateway.Api.Services;
using System.Threading.Tasks;
using System.Threading;

namespace AI.Gateway.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly IPromptAnalyzer _analyzer;

        public AnalysisController(IPromptAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

        [HttpPost]
        public async Task<ActionResult<AnalysisResponse>> Post([FromBody] AnalysisRequest? request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest(new { error = "Prompt is required" });
            }

            var response = await _analyzer.AnalyzeAsync(request.Prompt, cancellationToken);
            return response;
        }
    }
}
