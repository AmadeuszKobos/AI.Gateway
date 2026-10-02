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
