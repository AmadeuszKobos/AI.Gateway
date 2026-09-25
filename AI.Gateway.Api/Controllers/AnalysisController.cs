using Microsoft.AspNetCore.Mvc;
using AI.Gateway.Api.Models;
using System.Collections.Generic;

namespace AI.Gateway.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        [HttpPost]
        public ActionResult<AnalysisResponse> Post([FromBody] AnalysisRequest? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest(new { error = "Prompt is required" });
            }

            var prompt = request.Prompt.ToLowerInvariant();

            var completeness = new List<Finding>();
            var assumptions = new List<Finding>();

            void AddDatabase()
            {
                completeness.Add(new Finding
                {
                    Observation = "Persistence requirements unspecified",
                    Impact = "Requires DB schema, migrations, and transaction handling which affects implementation time and complexity"
                });
                assumptions.Add(new Finding
                {
                    Observation = "Assume relational DB acceptable",
                    Impact = "May require ORM and migration tooling"
                });
            }

            void AddAuth()
            {
                completeness.Add(new Finding
                {
                    Observation = "Authentication/authorization not defined",
                    Impact = "Needs identity model, roles, and secure endpoints which expands API surface and testing"
                });
                assumptions.Add(new Finding
                {
                    Observation = "Assume token-based auth",
                    Impact = "Integration with identity provider required"
                });
            }

            void AddIntegration()
            {
                completeness.Add(new Finding
                {
                    Observation = "External integration contracts unspecified",
                    Impact = "Requires contract definition, error handling and retry strategies"
                });
                assumptions.Add(new Finding
                {
                    Observation = "Assume synchronous HTTP integration",
                    Impact = "Implementation will need HTTP clients and timeout strategies"
                });
            }

            void AddValidation()
            {
                completeness.Add(new Finding
                {
                    Observation = "Input validation rules missing",
                    Impact = "Risk of incorrect data reaching business logic; needs validation layer"
                });
                assumptions.Add(new Finding
                {
                    Observation = "Assume basic field validation sufficient",
                    Impact = "Implementation uses model validation attributes"
                });
            }

            var matched = false;

            if (prompt.Contains("database") || prompt.Contains("db"))
            {
                AddDatabase();
                matched = true;
            }

            if (prompt.Contains("auth") || prompt.Contains("authentication") || prompt.Contains("authorization"))
            {
                AddAuth();
                matched = true;
            }

            if (prompt.Contains("integration") || prompt.Contains("api"))
            {
                AddIntegration();
                matched = true;
            }

            if (prompt.Contains("validation") || prompt.Contains("input"))
            {
                AddValidation();
                matched = true;
            }

            if (!matched)
            {
                completeness.Add(new Finding
                {
                    Observation = "Acceptance criteria missing",
                    Impact = "Unclear success criteria may cause rework and implementation uncertainty"
                });
                assumptions.Add(new Finding
                {
                    Observation = "Assume no external services required",
                    Impact = "Simplifies scope but may be invalid later"
                });
            }

            var response = new AnalysisResponse
            {
                Completeness = completeness,
                Assumptions = assumptions
            };

            return response;
        }
    }
}
