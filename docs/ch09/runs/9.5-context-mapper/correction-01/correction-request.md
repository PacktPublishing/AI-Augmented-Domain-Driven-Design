# Context Mapper 9.5 — Correction Request 01

**Classification:** guided correction after evaluation of the primary run. This
is not a new primary run and must not replace or modify the primary
`raw-output.md`.

**Primary output:** SHA-256
`84c779c250b3fb33f71531d240e135d3603cfbc81a2ab330be22f71f794e2c85`.

Return corrected versions of CM-10, CM-16, and CM-17 and no other candidates.

## CM-10

`QUOTE-01` is an interviewer prompt and does not itself support the order data
or direction asserted by the relationship. The remaining cited passages do.

- Keep `id: CM-10`, `kind: relationship`, and the evidenced direction from
  Sales' order responsibility to beer availability and holds.
- Remove `QUOTE-01` from `source`.
- Preserve only source identifiers that directly support the order information,
  direction, and decision owner.
- Do not add an order policy, customer relationship, or implementation detail.

## CM-16

The evidence establishes that warehouse staff notified the office in one
incident. It does not establish who had authority to decide what information
must be passed. For a relationship, `decision_owner` means the authority that
decides what is passed, not the actor that happened to send it.

- Keep `id: CM-16`, `kind: relationship`, and the evidenced direction from beer
  availability and holds to failed-batch order resolution.
- Set `decision_owner: unknown`.
- Preserve that the notification happened once and that its required contents,
  authority, and downstream decisions remain unsettled.
- Do not assign the policy to the warehouse or the office.

## CM-17

The evidence establishes a sequence: Sales decides that the order goes through,
and Shipping later collects held beer. It does not establish that Sales releases
the beer, instructs Shipping, or decides what crosses from the warehouse-side
responsibility to delivery. The current name, statement, and
`decision_owner: Sales` overstate that relationship.

- Keep `id: CM-17` and `kind: relationship`.
- Represent only the evidenced physical collection of held beer by Shipping
  after the Sales decision.
- Use `decision_owner: unknown`.
- Keep the instruction route, selection/release authority, timing, and delivery
  authorization unresolved.
- Do not create a Sales-to-delivery relationship or imply that Stock creates,
  requests, or authorizes a shipment.

## Output constraints

- Follow `handoff-schema.md` exactly and return one YAML document only.
- Return exactly three candidates in this order: CM-10, CM-16, CM-17.
- Include all nine candidate fields for each revision.
- Use only source identifiers from `stock-walkthrough.md` that genuinely support
  the corrected candidate.
- Do not add a candidate, boundary, relationship, policy, authority, or human
  decision.
- Do not use a human-decision status value.
- Set summary counts from the returned body and list only genuinely unaccounted
  sources relevant to these three corrections.
