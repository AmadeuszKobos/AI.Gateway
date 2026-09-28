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

## Stack technologiczny
- Backend: TODO
- Runtime: TODO
- API: TODO
- AI provider(s): TODO
- Storage: TODO
- Observability: TODO
- Testy: TODO
- Deployment: TODO

## Stack technologiczny (aktualny)
- .NET 10
- ASP.NET Core Web API
- xUnit (testy jednostkowe)
- wbudowany OpenAPI
- Swagger UI (uruchamiany tylko w trybie Development)

## Aktualny stan
Zdefiniowano problem, docelowego użytkownika, główny demonstracyjny use case oraz granice MVP.
Zaimplementowano pierwszy techniczny vertical slice: API przyjmuje prompt i zwraca deterministyczną analizę (FakePromptAnalyzer). System nie jest połączony z rzeczywistym dostawcą AI.

## Najważniejsze komponenty
- AnalysisController (HTTP API) — przyjmuje żądania z promptem i deleguje analizę
- IPromptAnalyzer (interfejs) — kontrakt analizy promptu
- FakePromptAnalyzer — deterministyczna implementacja analizatora
- Wbudowane OpenAPI i Swagger UI (Swagger UI działa tylko w środowisku Development)

## Ukończone funkcjonalności
- [x] Zdefiniowano MVP projektu.
- [x] Pierwszy techniczny vertical slice: endpoint analizy promptu z deterministycznym FakePromptAnalyzer i testami jednostkowymi.

## Aktualny task
Brak aktywnego tasku.

## Otwarte problemy / pytania
Brak otwartych pytań technicznych zapisanych w repozytorium.

## Aktualne elementy konfiguracji GitHub Copilot
.github/copilot-instructions.md: TODO
.github/instructions/: TODO
.github/prompts/: TODO
AGENTS.md: TODO
custom agents: TODO

## Jak wykorzystujemy Copilota
Copilot został wykorzystany do analizy repozytorium, wsparcia przy implementacji kodu i testów oraz przy edycji dokumentacji widocznej w repozytorium.

## Ostatnia aktualizacja
2026-09-28
