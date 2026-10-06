using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AI.Gateway.Api.Services;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace AI.Gateway.Api
{
    // Minimal typed exception handler that maps known AiServiceException kinds
    // (UpstreamFailure, InvalidResponse) to a safe 502 Bad Gateway ProblemDetails
    // response. Other unhandled runtime exceptions are mapped to a safe 500
    // ProblemDetails response. Implements only the required TryHandleAsync
    // method as requested.
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
            // Map known AI service failures to 502 Bad Gateway, otherwise keep as 500.
            // Get the framework-provided feature error if available (some hosts wrap
            // exceptions) then walk the exception chain to find any AiServiceException
            // and map the configured kinds to 502 while preserving safe ProblemDetails.
            var status = StatusCodes.Status500InternalServerError;

            var effectiveException = context.Features.Get<IExceptionHandlerFeature>()?.Error ?? exception;

            if (effectiveException is AiServiceException aiEx &&
                (aiEx.Kind == AiServiceErrorKind.UpstreamFailure ||
                 aiEx.Kind == AiServiceErrorKind.InvalidResponse))
            {
                status = StatusCodes.Status502BadGateway;
            }

            context.Response.StatusCode = status;

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
