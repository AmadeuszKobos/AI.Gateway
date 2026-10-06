using System;

namespace AI.Gateway.Api.Services
{
    public enum AiServiceErrorKind
    {
        UpstreamFailure,
        InvalidResponse
    }

    public class AiServiceException : Exception
    {
        public AiServiceErrorKind Kind { get; }

        public AiServiceException(AiServiceErrorKind kind, string? message = null, Exception? innerException = null)
            : base(message ?? kind.ToString(), innerException)
        {
            Kind = kind;
        }
    }
}
