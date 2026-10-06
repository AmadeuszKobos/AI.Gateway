# Backlog — AI Gateway Portfolio

## Statusy
- TODO
- IN PROGRESS
- REVIEW
- DONE
- BLOCKED

## Current

Brak aktywnego zadania.

## Next

Brak zdefiniowanych zadań.

## Done

### AG-001 — Define MVP

**Business/Technical Goal**

Zdefiniować problem, użytkownika, demonstracyjny use case i minimalny zakres pierwszej działającej wersji.

**Acceptance Criteria**
- [x] Problem projektu jest opisany w 1–3 zdaniach.
- [x] Docelowy użytkownik jest jasno określony.
- [x] Zdefiniowany jest jeden główny demonstracyjny use case.
- [x] MVP ma jasno określone granice.
- [x] Wiadomo, czego MVP świadomie nie obejmuje.

**Status**

DONE

**Copilot Approach**

Na etapie definiowania MVP Copilot nie był używany do podejmowania decyzji produktowych.

### AG-002 — First prompt analysis vertical slice

**Business/Technical Goal**

Zaimplementować minimalny vertical slice, który przyjmuje prompt i zwraca deterministyczną analizę (Completeness i Assumptions).

**Acceptance Criteria**
- [x] Istnieje endpoint API przyjmujący prompt i zwracający AnalysisResponse.
- [x] Zdefiniowany jest interfejs IPromptAnalyzer i implementacja FakePromptAnalyzer.
- [x] Obecne testy jednostkowe pokrywają FakePromptAnalyzer i AnalysisController i przechodzą.
- [x] Dostępny jest wygenerowany dokument OpenAPI, a Swagger UI jest dostępny w środowisku Development.

**Status**

DONE

**Copilot Approach**

Copilot został użyty do analizy repozytorium i wsparcia przy tworzeniu kodu i testów, które znajdują się w bieżącym repozytorium. Opis ogranicza się do działań już wykonanych i obecnych w workspace.

### AG-003 — OpenAI-backed prompt analysis

**Business/Technical Goal**

Replace FakePromptAnalyzer as the active runtime implementation with a real OpenAI Responses API integration while preserving the existing HTTP response model and provider-agnostic controller.

**Acceptance Criteria**
- [x] async IPromptAnalyzer contract is implemented and used at runtime.
- [x] OpenAIPromptAnalyzer implemented and wired into DI as the runtime analyzer.
- [x] minimal IOpenAIResponsesClient adapter introduced and implemented.
- [x] OpenAIResponsesClient implemented and uses the official OpenAI .NET SDK 2.14.0.
- [x] OpenAI Responses API is used for runtime analysis calls.
- [x] structured output configured via JSON Schema and applied using Patch on $.text.format.
- [x] provider/parsing failures are surfaced (not converted into fake success responses).
- [x] CancellationToken propagated from controller through analyzer to OpenAI call.
- [x] No secrets committed to the repository (API key read from configuration / environment).
- [x] Solution builds without warnings and tests pass.
- [x] 6/6 tests passing.
- [x] Real provider communication was reached during smoke testing (full paid structured-output E2E success pending).

**Status**

DONE

**Copilot Approach**

- Agent-driven, controlled multi-file implementation and refactors were used to implement the change.
- Compiler feedback (build errors) and small SDK spikes were used to validate SDK surface and Patch behavior rather than relying solely on generated code.
- Code review and manual cleanup were performed; Copilot suggestions were reviewed and not accepted blindly.

### AG-004 — Centralized API error handling

**Business/Technical Goal**

Introduce centralized handling of unhandled runtime exceptions using native ASP.NET Core mechanisms while keeping client-facing error responses safe and consistent.

**Acceptance Criteria**
- [x] Global exception handling uses ASP.NET Core `IExceptionHandler`.
- [x] `ProblemDetails` is registered and used for error responses.
- [x] Unhandled runtime exceptions return safe HTTP 500 `ProblemDetails`.
- [x] Exception messages, stack traces and provider details are not exposed to clients.
- [x] Existing HTTP 400 validation for an empty prompt remains unchanged.
- [x] Integration tests verify both 500 and 400 paths.
- [x] Test infrastructure uses `Microsoft.AspNetCore.Mvc.Testing` compatible with .NET 10.
- [x] Solution builds without warnings.
- [x] 9/9 tests pass.

**Status**

DONE

**Copilot Approach**

- Copilot Chat was used to analyze failure paths and compare error-handling approaches.
- The initial Copilot design was reviewed and corrected to use native `IExceptionHandler`, `AddProblemDetails()`, `AddExceptionHandler<T>()` and `UseExceptionHandler()`.
- Agent/Edit support was used for implementation and tests.
- A failing integration test was debugged manually with breakpoints rather than accepting Copilot's initial root-cause hypothesis.
- The actual issue was traced to an incompatible `Microsoft.AspNetCore.Mvc.Testing` version and corrected before removing the temporary serialization workaround.

### AG-005 — Preserve diagnostics for handled server exceptions

**Business/Technical Goal**

Preserve the framework diagnostics for exceptions that are handled by the centralized `GlobalExceptionHandler` so operational diagnostics emitted by ASP.NET Core are not suppressed for handled server errors.

**Acceptance Criteria**
- [x] `GlobalExceptionHandler` nadal zwraca bezpieczne `500 ProblemDetails`.
- [x] Diagnostyka obsłużonych wyjątków nie jest tłumiona (framework diagnostics zachowane).
- [x] Brak ręcznego logowania w `GlobalExceptionHandler`.
- [x] Brak nowych bibliotek (nie dodano Serilog/OpenTelemetry itp.).
- [x] Istniejące zachowanie `400` pozostaje bez zmian.
- [x] Solution/build przechodzi bez warnings.
- [x] Wszystkie istniejące testy przechodzą.

**Status**

DONE

**Copilot Approach**

AI / Copilot został użyty do analizy możliwych rozwiązań i porównania podejść. Sama zmiana w `Program.cs` (konfiguracja `UseExceptionHandler` z odpowiednimi `ExceptionHandlerOptions`) została wykonana ręcznie.

### AG-006 — Provider-agnostic AI service error classification

**Business/Technical Goal**

Introduce a minimal provider-agnostic error contract for runtime failures in the AI integration without leaking OpenAI SDK exception types above the SDK boundary.

**Acceptance Criteria**
- [x] `AiServiceException` introduced as a single provider-agnostic custom exception type.
- [x] `AiServiceErrorKind` contains `UpstreamFailure` and `InvalidResponse`.
- [x] `OpenAIResponsesClient` remains the direct OpenAI SDK boundary; no secondary production adapter was introduced.
- [x] Confirmed SDK non-success responses (`System.ClientModel.ClientResultException`) are translated to `AiServiceException(UpstreamFailure)` with the original exception preserved as `InnerException`.
- [x] `OperationCanceledException` is not wrapped and preserves native cancellation semantics.
- [x] Empty/whitespace provider output is classified as `InvalidResponse`.
- [x] Malformed JSON is classified as `InvalidResponse` and preserves the original `JsonException` as `InnerException`.
- [x] Deserialization to `null` is classified as `InvalidResponse`.
- [x] Semantic validation of incomplete `AnalysisResponse` remains outside the scope of this task.
- [x] `GlobalExceptionHandler` and HTTP status mappings remain unchanged.
- [x] No retry, logging, observability or new dependencies were introduced.
- [x] Solution builds without warnings and all tests pass.

**Status**

DONE

**Copilot Approach**

- Copilot Chat was used to identify and classify runtime failure paths and to separate confirmed behavior from SDK/runtime inference.
- Agent Mode implemented the first version of the exception contract; the generated secondary SDK adapter and broad `catch (Exception)` were rejected during manual architectural review.
- The design was simplified back to the existing `IOpenAIResponsesClient` / `OpenAIResponsesClient` boundary.
- Local SDK package/XML documentation was inspected with Copilot to confirm `System.ClientModel.ClientResultException` as the documented non-success HTTP exception for OpenAI .NET SDK 2.14.0.
- Final cleanup used targeted edits, followed by self-review, build and tests before commit.

### AG-007 — Map AI service failures to HTTP 502

**Business/Technical Goal**

Map the existing provider-agnostic AI service error categories to safe HTTP responses at the centralized error boundary.

**Acceptance Criteria**
- [x] `AiServiceException(UpstreamFailure)` maps to HTTP 502 Bad Gateway.
- [x] `AiServiceException(InvalidResponse)` maps to HTTP 502 Bad Gateway.
- [x] Other unhandled runtime exceptions continue to return HTTP 500.
- [x] Existing HTTP 400 validation for missing or whitespace-only prompts remains unchanged.
- [x] Error responses use safe `ProblemDetails`.
- [x] Exception messages, stack traces, provider-specific details and OpenAI SDK details are not exposed to clients.
- [x] `HttpContext.Response.StatusCode` and `ProblemDetails.Status` are kept consistent.
- [x] Integration tests verify both 502 paths through `WebApplicationFactory` / `HttpClient`.
- [x] Solution builds without warnings and all tests pass.
- [x] 16/16 tests passing.

**Status**

DONE

**Copilot Approach**

- Copilot Edit was used for the initial implementation.
- Manual review identified that the first added tests exercised `GlobalExceptionHandler` directly rather than the full HTTP pipeline.
- Integration tests were added using `WebApplicationFactory`.
- Debugging showed that `ProblemDetails.Status` was correctly set to 502 while `HttpContext.Response.StatusCode` remained 500.
- The final fix explicitly set `context.Response.StatusCode` before writing `ProblemDetails`.
- The implementation was simplified after debugging and validated with the full test suite.