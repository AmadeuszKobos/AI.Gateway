using System.Collections.Generic;

namespace AI.Gateway.Api.Models
{
    public class AnalysisResponse
    {
        public List<Finding> Completeness { get; set; } = new List<Finding>();
        public List<Finding> Assumptions { get; set; } = new List<Finding>();
    }
}
