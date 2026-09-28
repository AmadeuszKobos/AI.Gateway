using Microsoft.AspNetCore.Mvc;
using AI.Gateway.Api.Models;
using System.Collections.Generic;
using AI.Gateway.Api.Services;

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
        public ActionResult<AnalysisResponse> Post([FromBody] AnalysisRequest? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest(new { error = "Prompt is required" });
            }

            var response = _analyzer.Analyze(request.Prompt);
            return response;
        }
    }
}
