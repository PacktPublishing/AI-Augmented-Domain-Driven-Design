# Storyteller 9.3 — Correction Request 01

**Classification:** guided correction after the human gate. This is not a new
primary run and must not replace or modify `../raw-output.md`.

**Primary output:** SHA-256
`51ad6561fb24af5e0319ffcdef8db3ad9090a95a24c46de77af1f94891fc01c5`.

The domain authority referred exactly two candidates back. Return corrected
versions of ST-04 and ST-10 and no other candidates.

## ST-04

The name `pale ale is genuinely out until Thursday` turns an expected delivery
into a proved end date for the shortage. The evidence establishes that beer is
genuinely out and that more is *expected* Thursday; it does not establish an
arrival or that the shortage ends then.

- Keep `id: ST-04` and `kind: scenario`.
- Rename it `pale ale is out; more is expected Thursday`.
- Preserve the distinction between the temporary stock shortage and the generic
  response sent to Sales.
- Do not state or imply that replenishment arrived, that it was verified, or
  that the request will be retried.

## ST-10

The name `Sales sends a held order through to shipping` implies an evidenced
direct instruction from Sales to Shipping. The evidence establishes that Sales
decides an order goes through and that Shipping later collects the beer; it does
not establish the message, route, or authority by which Shipping is instructed.

- Keep `id: ST-10` and `kind: scenario`.
- Rename it `Sales decides a held order goes through`.
- Preserve that the warehouse tells Sales the beer is available and that the
  shipping lads later take it from the end bay.
- Keep the instruction route, timing, and Shipping authority unresolved.

## Output constraints

- Follow `handoff-schema.md` exactly and return one YAML document only.
- Return exactly two candidates in this order: ST-04, ST-10.
- Include all nine candidate fields for each revision.
- Preserve only source identifiers that genuinely support the corrected
  candidate.
- Do not add a candidate, policy, delivery confirmation, retry owner, dispatch
  instruction, or human decision.
- Do not use a human-decision status value.
- Set summary counts from the returned body and list only genuinely
  unaccounted sources relevant to these two corrections.
