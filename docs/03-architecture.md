# Architecture — AI Gateway Portfolio

> Dokumentuj aktualną architekturę. Nie opisuj planowanej architektury jako istniejącej.

## 1. Overview
Projekt zawiera działający, minimalny vertical slice analizujący prompt: HTTP API przyjmuje prompt i zwraca AnalysisResponse z dwoma perspektywami (Completeness i Assumptions). Analiza jest deterministyczna i realizowana przez FakePromptAnalyzer.

## 2. System Context
### Użytkownicy / klienci
Developer / technical user korzystający z API do uzyskania uwag o brakach i niejawnych założeniach w prompcie.

### Systemy zewnętrzne
Brak zintegrowanych dostawców AI ani innych zewnętrznych systemów w bieżącej implementacji.

## 3. High-Level Architecture
Runtime: .NET 10
API: ASP.NET Core Web API

Główne elementy zaimplementowane w repozytorium:
- AnalysisController (HTTP API)
- IPromptAnalyzer (kontrakt)
- FakePromptAnalyzer (deterministyczna implementacja analizatora)
- wbudowane OpenAPI i Swagger UI (Swagger UI włączany tylko w środowisku Development)

## 4. Components

### Component: AnalysisController
**Responsibility:** Przyjmowanie żądań HTTP z promptem, walidacja wejścia i delegowanie analizy do IPromptAnalyzer. Zwraca AnalysisResponse lub BadRequest przy nieprawidłowym wejściu.
**Inputs:** AnalysisRequest (Prompt)
**Outputs:** AnalysisResponse lub HTTP 400 (BadRequest)
**Dependencies:** IPromptAnalyzer

### Component: IPromptAnalyzer
**Responsibility:** Definiuje kontrakt metody Analyze(string prompt) zwracającej AnalysisResponse.
**Inputs:** string prompt
**Outputs:** AnalysisResponse
**Dependencies:** brak (interfejs)

### Component: FakePromptAnalyzer
**Responsibility:** Deterministyczna analiza promptu — mapowanie prostych słów-kluczy na znalezione braki (Completeness) i założenia (Assumptions).
**Inputs:** string prompt
**Outputs:** AnalysisResponse (listy Finding w Completeness i Assumptions)
**Dependencies:** brak zewnętrznych usług; implementacja znajduje się w projekcie API.

## 5. Request Flow
HTTP request -> AnalysisController -> IPromptAnalyzer -> FakePromptAnalyzer -> AnalysisResponse

Przy pustym lub whitespace prompt zwracany jest BadRequest (walidacja w kontrolerze).

## 6. Configuration
Projekt udostępnia wygenerowany dokument OpenAPI (MapOpenApi). Swagger UI jest skonfigurowany i udostępniany wyłącznie w trybie Development (kod w Program.cs). Inne elementy konfiguracyjne pozostają niezaimplementowane / TODO.

## 7. Error Handling
Kontroler zwraca HTTP 400 (BadRequest) gdy prompt jest pusty lub null. Inne mechanizmy obsługi błędów nie zostały dodane i pozostają jako TODO.

## 8. Security Boundaries
TODO: brak wdrożonych mechanizmów uwierzytelniania/autoryzacji w obecnej implementacji.

## 9. Observability
TODO: brak zintegrowanych mechanizmów obserwowalności (logging/metrics/tracing) w bieżącej wersji.

## 10. Data Flow
Prompt (HTTP request body) -> AnalysisController validates input -> AnalysisController calls IPromptAnalyzer.Analyze -> FakePromptAnalyzer produces AnalysisResponse (Completeness, Assumptions) -> AnalysisController returns AnalysisResponse to the caller.

## 11. Deployment Model
TODO: brak zdefiniowanego modelu deploymentu w repozytorium.

## 12. Known Limitations
- Analiza jest deterministyczna/fake (FakePromptAnalyzer) i nie korzysta z rzeczywistego dostawcy AI.
- Brak integracji z zewnętrznymi providerami AI.
- Swagger UI jest uruchamiany tylko w środowisku Development.

## 13. Open Architecture Questions
- Brak otwartych pytań architektonicznych na tym etapie.
