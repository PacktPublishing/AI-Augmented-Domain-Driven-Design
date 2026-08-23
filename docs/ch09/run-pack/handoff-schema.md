# Handoff Schema

Every specialist returns its work in this shape. The schema exists so that the
person who reads the output next can judge it without reconstructing the
conversation that produced it.

Return one YAML document and nothing else. No prose before it, no commentary
after it.

---

## Shape

```yaml
candidates:
  - id: <prefix>-01
    kind: <specialist-specific, see your contract>
    name: <the name you propose, in the language of the business>
    statement: <one sentence saying what this is>
    source:
      - QUOTE-05
      - QUOTE-06
    status: observed | inferred | proposed | unresolved
    decision_owner: <who must decide this, or "unknown">
    uncertainty: <what would have to be true for this to hold, and what is missing>
    review_question: <the question a human must answer before this advances>

summary:
  candidates_returned: <n>
  unresolved: <n>
  unaccounted_sources: [<source identifiers you could not place, or empty>]
  information_needed: <one or two sentences>
```

Your contract gives you the `id` prefix to use. Use it on every candidate, so
that outputs from different specialists can be read side by side without
collision.

## The fields

**`source`** — identifiers taken from the evidence, exactly as written there.
Every candidate must cite at least one. Cite only identifiers that genuinely
support the candidate: an identifier that exists but does not say what you are
claiming is worse than no identifier at all, because it survives a spot check.

**`status`** — how the candidate stands in relation to the evidence:

| Status | Meaning |
|---|---|
| `observed` | The evidence states this directly. |
| `inferred` | You concluded it from the evidence. The evidence does not say it. |
| `proposed` | You are suggesting it. The evidence neither states nor implies it. |
| `unresolved` | The evidence raises the matter and settles nothing. |

`unresolved` is a successful result, not a gap in your work. Where the evidence
raises a matter and settles nothing, the correct output is `unresolved` with a
`review_question`. Producing a rule in that situation is an error, however
sensible the rule.

**`decision_owner`** — who has the authority to decide this, not who wrote it
down and not who carries it out. Write `unknown` when the evidence does not say.
`unknown` is a real answer and is frequently the right one.

**`uncertainty`** — what would have to be true for the candidate to hold, and
what is missing from the evidence. Do not write a confidence score. A number
expressing how sure you feel is not a measurement and will be ignored.

**`review_question`** — the single question a human must answer before anything
downstream may rely on this candidate. A useful review question names the
missing decision and the authority or evidence needed to resolve it. It does not
merely ask whether the candidate is correct.

## Statuses you may not use

`accepted` and `rejected` are not available to you. Deciding whether a candidate
becomes part of the model is not part of your work, and an output that claims
either is a failed output regardless of its content.

## Naming

Name things in the language the business used. Where the evidence uses several
words for what you believe is one thing, choose one, say which words you are
unifying, and record the choice as a candidate with its own `review_question`.
Do not silently pick a favourite.

Where the business has no word for something you believe exists, say so in
`uncertainty` rather than inventing a term that sounds established.

## The summary block

`unaccounted_sources` matters more than the counts. If part of the evidence did
not fit anywhere in your output, list those identifiers. An empty list is read as
a claim that you placed everything, and it will be checked.
