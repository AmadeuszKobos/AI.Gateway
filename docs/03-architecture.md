# Architecture — AI Gateway Portfolio

> Dokumentuj aktualną architekturę. Nie opisuj planowanej architektury jako istniejącej.

## 1. Overview
Projekt zawiera działający, minimalny vertical slice analizujący prompt: HTTP API przyjmuje prompt i zwraca AnalysisResponse z dwiema perspektywami (Completeness i Assumptions). Runtime używa OpenAIPromptAnalyzer, który deleguje do adaptera IOpenAIResponsesClient i dalej do OpenAIResponsesClient wykorzystującego oficjalny OpenAI .NET SDK 2.14.0 i Responses API.

## 2. System Context
### Użytkownicy / klienci
Developer / technical user korzystający z API do uzyskania uwag o brakach i niejawnych założeniach w prompcie.

### Systemy zewnętrzne
- OpenAI Responses API (integracja runtime przez OpenAIResponsesClient)

## 3. High-Level Architecture
Runtime: .NET 10
API: ASP.NET Core Web API

Główne elementy zaimplementowane w repozytorium:
- AnalysisController (HTTP API)
- IPromptAnalyzer (kontrakt)
- OpenAIPromptAnalyzer (runtime implementation)
- IOpenAIResponsesClient, OpenAIResponsesClient (adapter + SDK boundary)
- FakePromptAnalyzer — deterministyczna implementacja pozostająca w repozytorium, ale niewykorzystywana jako aktywny runtime analyzer
- wbudowane OpenAPI i Swagger UI (Swagger UI włączany tylko w środowisku Development)

## 4. Components

### Component: AnalysisController
**Responsibility:** Przyjmowanie żądań HTTP z promptem, walidacja wejścia i delegowanie analizy do IPromptAnalyzer. Zwraca AnalysisResponse lub BadRequest przy nieprawidłowym wejściu.
**Inputs:** AnalysisRequest (Prompt)
**Outputs:** AnalysisResponse lub HTTP 400 (BadRequest)
**Dependencies:** IPromptAnalyzer

### Component: IPromptAnalyzer
**Responsibility:** Aplikacyjny, provider-agnostyczny kontrakt analizy promptu. Metoda jest asynchroniczna i akceptuje CancellationToken.
**Inputs:** string prompt, CancellationToken
**Outputs:** AnalysisResponse
**Dependencies:** brak (interfejs)

### Component: OpenAIPromptAnalyzer
**Responsibility:** Implementacja uruchomieniowa IPromptAnalyzer, która waliduje prompt, wywołuje IOpenAIResponsesClient, sprawdza, że odpowiedź dostawcy nie jest pusta oraz deserializuje JSON do AnalysisResponse. Nie zawiera typów SDK OpenAI (zależność realizowana wyłącznie przez interfejs adaptera).
**Inputs:** string prompt, CancellationToken
**Outputs:** AnalysisResponse
**Dependencies:** IOpenAIResponsesClient

### Component: IOpenAIResponsesClient / OpenAIResponsesClient
**Responsibility (IOpenAIResponsesClient):** Minimalna warstwa-adapter dla wywołania OpenAI Responses; utrzymuje kod wyższego poziomu wolny od typów SDK.
**Responsibility (OpenAIResponsesClient):** Odpowiada za konstrukcję żądania specyficznego dla SDK, konfiguruje strukturalizowany output (JSON Schema) poprzez Patch na $.text.format, wywołuje ResponsesClient.CreateResponseAsync oraz wydobywa tekst wyjściowy za pomocą GetOutputText.
**Dependencies:** OpenAI .NET SDK 2.14.0 (Responses API)

### Component: FakePromptAnalyzer
**Responsibility:** Deterministyczna implementacja pozostająca w repozytorium, ale niewykorzystywana jako aktywny runtime analyzer.

## 5. Request Flow
HTTP POST /api/Analysis -> AnalysisController -> IPromptAnalyzer -> OpenAIPromptAnalyzer -> IOpenAIResponsesClient -> OpenAIResponsesClient -> OpenAI Responses API

CancellationToken jest przekazywany z AnalysisController przez IPromptAnalyzer do wywołania SDK OpenAI. Kontroler waliduje wejście i zwraca HTTP 400 dla pustych lub zawierających tylko whitespace promptów.

## 6. Configuration
Projekt udostępnia wygenerowany dokument OpenAPI (MapOpenApi). Swagger UI jest skonfigurowany i udostępniany wyłącznie w trybie Development (kod w Program.cs). Inne elementy konfiguracyjne pozostają niezaimplementowane / TODO.

## 7. Error Handling

AnalysisController returns HTTP 400 for a missing or whitespace-only prompt.

Unhandled runtime exceptions are handled centrally using ASP.NET Core `IExceptionHandler`.
`GlobalExceptionHandler` uses `IProblemDetailsService` to return a safe HTTP 500
`ProblemDetails` response without exposing exception messages, stack traces or provider-specific details.

The handler is registered using:
- `AddProblemDetails()`
- `AddExceptionHandler<GlobalExceptionHandler>()`
- `UseExceptionHandler()`

More granular mappings such as provider availability vs invalid upstream responses
are intentionally not implemented yet.

## 8. Security Boundaries
TODO: brak wdrożonych mechanizmów uwierzytelniania/autoryzacji w obecnej implementacji.

## 9. Observability
TODO: brak zintegrowanych mechanizmów obserwowalności (logging/metrics/tracing) w bieżącej wersji.

## 10. Data Flow
Prompt (ciało żądania HTTP) -> AnalysisController waliduje wejście -> AnalysisController wywołuje IPromptAnalyzer.Analyze -> FakePromptAnalyzer generuje AnalysisResponse (Completeness, Assumptions) -> AnalysisController zwraca AnalysisResponse do klienta.

## 11. Deployment Model
TODO: brak zdefiniowanego modelu deploymentu w repozytorium.

## 12. Known Limitations
- OpenAI Responses surface in SDK 2.14.0 is experimental/evaluation and subject to change.
- Structured output uses Patch to set $.text.format because typed SDK support for structured outputs is not available in 2.14.0.
- No retries or rate-limiting implemented.
- No observability (metrics/tracing/log aggregation) implemented.
- No provider fallback or multi-provider support.
- Full paid structured-output end-to-end success is still pending (controlled smoke tests reached the provider, but final paid structured-output confirmation remains incomplete).

## 13. Open Architecture Questions
Brak nowych otwartych pytań architektonicznych z perspektywy wdrożonej integracji runtime; zobacz Known Limitations dla elementów wymagających dalszego rozwoju.
