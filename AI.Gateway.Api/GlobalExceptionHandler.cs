using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AI.Gateway.Api
{
    // Minimal typed exception handler that maps all unhandled runtime exceptions
    // to a safe 500 ProblemDetails response. Implements only the required
    // TryHandleAsync method as requested.
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;

        public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
        {
            _problemDetailsService = problemDetailsService ?? throw new ArgumentNullException(nameof(problemDetailsService));
        }

        public async System.Threading.Tasks.ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            // Build a minimal, safe ProblemDetails for a server error
            var status = 500;
            var instance = context.Request?.Path.Value ?? string.Empty;
            var pd = new ProblemDetails
            {
                Type = "about:blank",
                Title = "An error occurred while processing the request.",
                Status = status,
                Instance = instance
            };

            var problemDetailsContext = new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = pd
            };

            // Use the framework-provided service to write the ProblemDetails response
            await _problemDetailsService.WriteAsync(problemDetailsContext).ConfigureAwait(false);

            return true;
        }
    }
}
