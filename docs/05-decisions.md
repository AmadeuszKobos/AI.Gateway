# Architecture Decision Log — AI Gateway Portfolio

> Indeks istotnych decyzji. Dla większych decyzji twórz osobne ADR-y.

## Kiedy zapisywać decyzję
Zapisuj decyzję, jeśli:
- istotnie wpływa na architekturę,
- trudno będzie ją później odwrócić,
- istnieje kilka rozsądnych alternatyw,
- prawdopodobnie pojawi się pytanie „dlaczego zrobiliśmy to w ten sposób?”.

## Decision Log

| ID | Date | Decision | Status | ADR |
|---|---|---|---|---|


## Template

### DEC-XXX — Tytuł

**Date:** YYYY-MM-DD  
**Status:** Proposed / Accepted / Superseded / Rejected

**Context**

TODO

**Options Considered**
1. TODO
2. TODO

**Decision**

TODO

**Why**

TODO

**Consequences**

Pozytywne:
- TODO

Negatywne / trade-offs:
- TODO

**Follow-up**
- TODO

### DEC-OPENAI-ADAPTER — Introduce IOpenAIResponsesClient + OpenAIResponsesClient

**Date:** 2026-10-02
**Status:** Accepted

**Context**

Direct testing of OpenAIPromptAnalyzer against OpenAI.Responses.ResponsesClient required handling SDK-specific ClientResult<ResponseResult> construction and experimental SDK details that would leak into higher-level code and tests.

**Options Considered**
1. Depend directly on ResponsesClient everywhere.
2. Subclass or fake ResponsesClient in tests.
3. Introduce a minimal adapter (IOpenAIResponsesClient + OpenAIResponsesClient).

**Decision**

Introduce a minimal adapter: IOpenAIResponsesClient and OpenAIResponsesClient.

**Why (Rationale)**

- Isolates OpenAI SDK-specific code behind a small boundary.
- Keeps OpenAIPromptAnalyzer free from SDK types, simplifying its implementation and tests.
- Simplifies offline/unit tests without subclassing or faking the SDK client.
- Localizes experimental OPENAI001 / Patch usage to a single component.  
- Avoids prematurely creating a generic multi-provider framework.

**Consequences**

- One additional abstraction in the codebase.
- OpenAIResponsesClient becomes the SDK boundary where future SDK changes are handled.
- Most future changes related to the OpenAI SDK or structured-output Patch usage should be localized to OpenAIResponsesClient.

**Note**

Structured Outputs currently use Patch to set $.text.format because OpenAI .NET SDK 2.14.0 does not expose the required typed Responses member for structured outputs; this is an implementation detail confined to OpenAIResponsesClient.

### DEC-ERROR-HANDLING — Use native IExceptionHandler with ProblemDetails

**Date:** 2026-10-05
**Status:** Accepted

**Context**

The API needs a centralized mechanism for converting unhandled runtime exceptions
into safe and consistent HTTP responses without adding try/catch logic to controllers.

**Options Considered**
1. MVC exception filters.
2. Custom exception middleware.
3. Native ASP.NET Core IExceptionHandler with ProblemDetails.

**Decision**

Use ASP.NET Core `IExceptionHandler` together with `AddProblemDetails()`,
`AddExceptionHandler<T>()` and `UseExceptionHandler()`.

**Why**

- Central handling across the HTTP pipeline.
- Native framework support without external dependencies.
- Keeps controllers focused on request handling.
- Provides a standard ProblemDetails contract.
- Allows future exception-category mappings without changing controller code.

**Consequences**

Positive:
- consistent safe 500 responses,
- no exception details exposed to API clients,
- extensible place for future mappings.

Trade-offs:
- current implementation intentionally maps all unhandled runtime exceptions to 500,
- cancellation and provider-specific 502/503 mappings remain future work.

**Follow-up**
- Consider more granular mappings when there is a concrete client or observability use case.

### DEC-AI-SERVICE-ERRORS — Use a minimal provider-agnostic AI service error contract

**Date:** 2026-10-06  
**Status:** Accepted

**Context**

Runtime failures from the OpenAI integration previously surfaced as a mix of SDK-specific exceptions, `JsonException` and `InvalidOperationException`. The application needs a stable error vocabulary above the OpenAI SDK boundary without introducing a large exception hierarchy or changing the existing exception-based control flow.

**Options Considered**
1. Propagate existing SDK/runtime exception types unchanged.
2. Introduce multiple provider-specific custom exception classes.
3. Introduce one provider-agnostic `AiServiceException` with a small `AiServiceErrorKind` discriminator.
4. Replace exception flow with a `Result<T, Error>` model.

**Decision**

Use one provider-agnostic `AiServiceException` with two current categories: `UpstreamFailure` and `InvalidResponse`.

- `OpenAIResponsesClient` translates the confirmed SDK non-success exception `System.ClientModel.ClientResultException` to `UpstreamFailure`.
- `OpenAIPromptAnalyzer` translates empty/whitespace output, malformed JSON and deserialization to `null` to `InvalidResponse`.
- `OperationCanceledException` is not wrapped.
- No secondary SDK adapter is introduced solely for testability.

**Why**

- Keeps OpenAI SDK types localized to the existing SDK boundary.
- Preserves the current exception-based application flow and avoids a broader `Result` refactor.
- Uses one custom exception type instead of a hierarchy.
- Preserves native .NET cancellation semantics.
- Allows future HTTP mappings without coupling `GlobalExceptionHandler` to OpenAI SDK types.

**Consequences**

Positive:
- stable provider-agnostic error contract above the SDK boundary,
- clear separation between upstream/API failure and invalid provider content,
- original exceptions can remain available through `InnerException`,
- minimal additional production complexity.

Trade-offs:
- `GlobalExceptionHandler` does not yet map the categories to distinct HTTP status codes,
- direct deterministic unit testing of the concrete OpenAI SDK boundary remains limited without an integration-style HTTP test,
- low-level network failures not explicitly documented by the current SDK are not proactively classified.

**Follow-up**
- Add distinct HTTP mappings only when there is a concrete API/client requirement.
- Revisit transport/network classification if the SDK contract or observed runtime behavior provides confirmed exception types.
- Consider semantic validation of `AnalysisResponse` as a separate feature if false-success responses become a concrete risk.

