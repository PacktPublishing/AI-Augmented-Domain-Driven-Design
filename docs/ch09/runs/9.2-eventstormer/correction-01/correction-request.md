# EventStormer 9.2 — Correction Request 01

**Classification:** guided correction after the human gate. This is not a new
primary run and must not replace or modify `../raw-output.md`.

**Primary output:** SHA-256
`0f62f5949c7e16065e4872fc84bbd39953ce4566bb0fb7ea7252cb1c0fe4b494`.

The domain authority referred exactly two candidates back. Return corrected
versions of ES-24 and ES-29 and no other candidates.

## ES-24

The name `more beer arrived` claims an arrival that the cited evidence never
reports. The evidence says that more beer was expected and that Nadia might
retry on the expected day.

- Keep `id: ES-24` and `kind: trigger`.
- Rename it `more beer was expected`.
- Do not state or imply that replenishment actually arrived or was verified.
- Keep the retry trigger and owner unresolved.
- Preserve a review question asking who initiates a retry and what authoritative
  replenishment evidence may trigger it.

## ES-29

The trigger and the fact ES-30 were both named `order was called off`, hiding the
difference between the external commercial fact and the warehouse receiving it.

- Keep `id: ES-29` and `kind: trigger`.
- Rename it `order cancellation was received from Sales`.
- Preserve that receiving the Sales cancellation prompted Andrea to remove the
  board row and Nadia to return the beer.
- Do not change the order-lifecycle authority or imply that the warehouse caused
  the cancellation.

## Output constraints

- Follow `handoff-schema.md` exactly and return one YAML document only.
- Return exactly two candidates in this order: ES-24, ES-29.
- Include all nine candidate fields for each revision.
- Use only source identifiers from `stock-walkthrough.md` that genuinely support
  the corrected candidate.
- Do not add a candidate, policy, duration, retry owner, or decision.
- Do not use a human-decision status value.
- Set summary counts from the returned body and list only genuinely unaccounted
  sources relevant to these two corrections.
