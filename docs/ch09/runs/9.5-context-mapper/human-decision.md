# Context Mapper 9.5 — Human Decision

**Status:** pending explicit domain-authority confirmation.

**Primary review basis:** `raw-output.md`, SHA-256
`84c779c250b3fb33f71531d240e135d3603cfbc81a2ab330be22f71f794e2c85`.

**Correction review basis:** `correction-01/raw-output.md`, SHA-256
`d2849bc6b2a9ced704b55cd6fcc2069793cb4914eb04fdecb640d0ac1a70142d`.

The primary run returned 18 candidates. Three were referred for evidence or
authority defects and corrected without modifying the primary record. The body
contains four unresolved candidates; the primary summary's count of five was a
run defect and is not the effective count.

## Decision proposed for confirmation

| Candidates | Proposed decision | Constraint preserved |
|---|---|---|
| CM-01–CM-09, CM-11–CM-15, CM-18 | accepted as modeling material | Names remain proposals; acceptance is not a business naming or implementation decision. |
| CM-10 | accepted after correction | `QUOTE-01` is removed; Sales remains owner only of the evidenced order request. |
| CM-16 | accepted after correction | The notification happened once; authority over its required contents remains unknown. |
| CM-17 | accepted after correction | Only physical Shipping collection is recorded; release, instruction, and dispatch authority remain unknown. |
| CM-06, CM-08, CM-12, CM-14 | accepted while remaining `unresolved` | Failed-batch resolution, product-code correction, retry, and partial-quantity notification remain open. |

The gate must also confirm that no proposed boundary name is business-approved,
no payment or delivery edge is invented, and no authority is assigned for hold
duration, partial availability, retry, failed-batch handling, or Shipping beyond
the evidence.

After explicit confirmation, record reviewer/date/notes here and generate
`accepted-context-map.md` from the effective 18 candidates, replacing CM-10,
CM-16, and CM-17 with their corrected versions.
