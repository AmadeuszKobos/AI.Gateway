# Project Context — AI Gateway Portfolio

> Krótkie, aktualne źródło prawdy o projekcie.

## Cel projektu
Zbudować profesjonalny projekt portfolio AI Gateway pokazujący praktyczne umiejętności w obszarze backendu, API i modeli AI, bezpieczeństwa, observability, testów, CI/CD, Git/GitHub oraz świadomego wykorzystania GitHub Copilot.

## Problem, który rozwiązujemy
Developerzy piszący prompty do implementacji feature nie zawsze widzą istotne braki, niejawne założenia i ich wpływ na kierunek implementacji. W efekcie model może przyjąć niezamierzone założenia albo wygenerować rozwiązanie, które nie odpowiada rzeczywistej intencji użytkownika.

## Docelowy użytkownik
Developer / technical user korzystający z modeli AI do implementacji feature.

## Główny demonstracyjny use case
Developer podaje prompt opisujący feature do implementacji. System analizuje prompt z dwóch niezależnych perspektyw — Completeness i Assumptions — oraz zwraca uwagi dotyczące brakującego kontekstu, niejawnych założeń i ich potencjalnego wpływu na kierunek implementacji.

System nie przepisuje promptu i nie implementuje feature. Użytkownik sam decyduje, które uwagi wykorzystać i jak zmodyfikować prompt.

## Aktualny zakres MVP
MVP:
- przyjmuje jeden prompt dotyczący implementacji feature,
- analizuje go z perspektywy `Completeness`,
- analizuje go z perspektywy `Assumptions`,
- zwraca uwagi z obu perspektyw,
- wskazuje potencjalny wpływ wykrytych braków i założeń na implementację.

Świadomie poza MVP:
- automatyczne przepisywanie lub poprawianie promptu,
- implementowanie feature za użytkownika,
- dodatkowe perspektywy analizy,
- automatyczne rozstrzyganie, która sugestia jest najlepsza.

## Stack technologiczny (aktualny)
- .NET 10
- ASP.NET Core Web API
- xUnit (testy jednostkowe)
- wbudowany OpenAPI
- Swagger UI (uruchamiany tylko w trybie Development)

Dodatkowe elementy runtime:
- oficjalny OpenAI .NET SDK 2.14.0
- OpenAI Responses API (wykorzystane przez adapter OpenAIResponsesClient)
- strukturalizowany output skonfigurowany przy użyciu JSON Schema (ustawiany przez Patch w SDK)

## Aktualny stan
Zdefiniowano problem, docelowego użytkownika, główny demonstracyjny use case oraz granice MVP.
Zaimplementowano techniczny vertical slice z rzeczywistą integracją runtime z OpenAI Responses API. Zamiast FakePromptAnalyzer w czasie wykonywania aplikacji używany jest OpenAIPromptAnalyzer, który deleguje do adaptera IOpenAIResponsesClient i dalej do OpenAIResponsesClient (który korzysta z oficjalnego SDK 2.14.0). FakePromptAnalyzer pozostaje w repozytorium jako deterministyczna implementacja, ale nie jest używany jako aktywny analyzer runtime.

Ważne: osiągnięto rzeczywistą komunikację z API OpenAI podczas kontrolnych testów smoke; pełna, opłacona end-to-end (E2E) walidacja poprawnego structured-output pozostaje nieukończona.

## Najważniejsze komponenty
- AnalysisController (HTTP API) — przyjmuje żądania z promptem i deleguje analizę
- IPromptAnalyzer (interfejs) — kontrakt analizy promptu
- OpenAIPromptAnalyzer — implementacja uruchomieniowa delegująca do IOpenAIResponsesClient
- IOpenAIResponsesClient / OpenAIResponsesClient — minimalny adapter izolujący zależności od OpenAI SDK
- FakePromptAnalyzer — deterministyczna implementacja pozostająca w repozytorium, ale niewykorzystywana jako aktywny runtime analyzer
- Wbudowane OpenAPI i Swagger UI (Swagger UI działa tylko w środowisku Development)

## Ukończone funkcjonalności
- [x] Zdefiniowano MVP projektu.
- [x] Pierwszy techniczny vertical slice: endpoint analizy promptu z deterministycznym FakePromptAnalyzer i testami jednostkowymi.
- [x] Zastąpiono fake runtime analizator (FakePromptAnalyzer) implementacją opartą o OpenAI Responses API (OpenAIPromptAnalyzer + OpenAIResponsesClient). Integracja osiągnięta bez wprowadzania sekretów do repozytorium.
- [x] Osiągnięto rzeczywistą komunikację z API OpenAI podczas kontrolnych testów smoke; pełna, opłacona end-to-end (E2E) walidacja poprawnego structured-output pozostaje nieukończona.

## Aktualny task
Brak aktywnego tasku.

## Otwarte problemy / pytania
Otwarte pytania / ograniczenia do uwzględnienia:
- Pełna, opłacona E2E walidacja poprawnej strukturalnej odpowiedzi pozostaje pending (testy smoke wykonały połączenie z providerem, ale płatne żądanie z pełnym structured-output nie zostało ukończone).
- SDK OpenAI 2.14.0 nie wystawia jeszcze w pełni typowanego, publicznego API do structured outputs, dlatego implementacja używa Patch do ustawienia $.text.format.
- Brak retry/limitowania/observability/authentication/deployment — te aspekty nie zostały zaimplementowane i nie są przedmiotem tego sprintu.

## Aktualne elementy konfiguracji GitHub Copilot
.github/copilot-instructions.md: TODO
.github/instructions/: TODO
.github/prompts/: TODO
AGENTS.md: TODO
custom agents: TODO

## Jak wykorzystujemy Copilota
Copilot został wykorzystany do analizy repozytorium, wsparcia przy implementacji kodu i testów oraz przy edycji dokumentacji widocznej w repozytorium.

## Ostatnia aktualizacja
2026-10-02
