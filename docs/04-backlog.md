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
