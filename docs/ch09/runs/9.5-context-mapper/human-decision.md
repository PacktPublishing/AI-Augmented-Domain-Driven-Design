# Context Mapper 9.5 — Human Decision

Status: **pending human review**.

Review basis: `raw-output.md`, SHA-256 `567ffe9b06794160ecb53c45a3e452a077c11ed5019deb2dbc409783b308cc43`.

This file is deliberately not pre-filled with an acceptance claim. The Context Mapper output was prepared as an authoring reference in a session that had already seen evaluator-only material, and no human domain-authority gate has yet reviewed the 12 candidates.

## Recommended gate reading

The reference evaluation recommends accepting all 12 candidates as traceable modeling material while retaining CM-06 and CM-11 as `unresolved`. Acceptance must not convert a proposed boundary name into a business-approved bounded-context name, assign dispatch authority, merge the two office responsibilities, or introduce implementation structure.

## Decisions to record

| Candidates | Human decision | Question to settle |
|---|---|---|
| CM-01 | pending | Does availability plus hold/release work form one responsibility boundary, and what is its business name? |
| CM-02 | pending | Does Sales own the full order-decision responsibility described, and what is the boundary called? |
| CM-03 | pending | Which office role owns payment, and is any direct exchange with Stock established? |
| CM-04, CM-12 | pending | Who owns dispatch, and is the physical collection edge represented at the right boundary level? |
| CM-05, CM-10 | pending | Which lab authority owns the failed-batch outcome and what fact crosses the boundary? |
| CM-06, CM-11 | pending | Which office authority owns failed-batch consequences and what handoff is required? |
| CM-07, CM-08, CM-09 | pending | Are the Sales/availability exchanges and their directions correctly represented? |

After the gate, replace each `pending` decision with `accepted`, `referred`, or `rejected`, record reviewer/date/notes, and regenerate `accepted-context-map.md` from only the accepted effective candidates.
