# Decision

PASS

# Scope

- Selected story — explicit-path rank:
  `_bmad-output/implementation-artifacts/1-2-complete-payment-projection-and-provider-callback-behavior.md`;
  its normalized path remains under `_bmad-output/`.
- Selected PRD — canonical monolith rank:
  `_bmad-output/planning-artifacts/prd.md`.
- Selected architecture — canonical monolith rank after the absent preferred
  `ARCHITECTURE-SPINE.md`:
  `_bmad-output/planning-artifacts/architecture.md`.
- Inspected governance/context:
  `_bmad-output/project-context.md`,
  `.bmad-harness/governance/gate-contract.md`,
  `.bmad-harness/governance/brewup-module-structure.md`, and
  `.bmad-harness/governance/brewup-sales-order-confirmation.md`.
- Inspected upstream evidence:
  `_bmad-output/planning-artifacts/epics.md`,
  `_bmad-output/implementation-artifacts/1-1-order-confirmation.md`,
  `docs/superpowers/specs/2026-07-27-brewup-order-confirmation-with-bmad-harness-design.md`,
  `docs/superpowers/plans/2026-07-27-brewup-order-confirmation-with-bmad-harness.md`,
  and
  `.superpowers/sdd/2026-07-27-brewup-order-confirmation-with-bmad-harness/task-3-brief.md`.
- Eligible but unselected fallback-rank story candidates:
  `_bmad-output/implementation-artifacts/1-1-order-confirmation.md` has a
  different story identifier, and
  `_bmad-output/implementation-artifacts/story-gate.md` is not the requested
  story; the explicit requested Story 1.2 path wins.
- Missing required artifacts: none.
- Excluded eligible candidates: none.
- The independent guard remained read-only.

# Findings

None.

# Open decisions

No blocking open decision applies to Story 1.2. Decline/unknown outcomes,
provider-timeout interpretation, authentication changes, amount/currency,
real-provider execution, compensation, void/refund, retry, notification,
expiry, reservation, Sales confirmation, saga behavior, shipment, and
invoicing remain explicitly excluded with stop conditions.

# Traceability

| Story concern | Upstream requirement | Architecture | Intended paths | Acceptance criteria | Tests / verification | Rules |
|---|---|---|---|---|---|---|
| Pending Payment projection | FR-001, FR-003; NFR-003 | AD-5, AD-9 | `BrewUp.Payment.ReadModel/{Dtos,Queries,Services,EventHandlers}` | AC2 | `ProjectPendingAuthorization` | BC-003, BC-004; AR-009 |
| Provider callback boundary | FR-003, FR-004 | AD-1, AD-5 | `BrewUp.Payment.Facade/{Endpoints,ExternalContracts}`, `IPaymentFacade`, `PaymentFacade` | AC3, AC5 | `RecordAuthorizedCallbackBoundary` | BC-003, BC-004, BC-008; AR-006–AR-008, AR-011, AR-017 |
| Projection and integration publication | FR-003, FR-004 | AD-5, AD-9 | ReadModel authorized handler; Facade integration publisher | AC4, AC5 | `ProjectAndPublishAuthorizedOutcome` | BC-003, BC-004; AR-007–AR-009, AR-011, AR-017 |
| Persistence and composition | FR-016–FR-018 | AD-3, AD-4, AD-11 | Payment Infrastructure/helper files, `PaymentFacadeHelper`, existing `PaymentModule` | AC1, AC6, AC7 | `PaymentPersistenceWiring` | AR-001, AR-002, AR-010, AR-011, AR-013–AR-016 |
| Module placement and boundaries | FR-016–FR-018; NFR-001 | AD-3, AD-4, AD-10 | Six Payment projects, `src/BrewUp.slnx`, REST Facade-only reference | AC7, AC8 | `PaymentModuleStructureAndComposition`, REST architecture suite | AR-001–AR-017 |
| Test-first delivery | FR-019 | AD-10 | Four named Payment test files and existing architecture test | AC9 | Mandatory meaningful RED, focused GREEN, full Payment suite, REST architecture suite, solution build | AR-000, AR-012 |
| Implementation conventions | NFR-005 | AD-10, AD-11 | Story 1.2 changed production-file set derived from recorded baseline | AC10 | `Story12ImplementationConventionReview` after GREEN | AR-000, AR-012 |
| Policy containment | PRD exclusions | AD-11 | Explicit exclusions and stop conditions | AC11 | Scope review | BC-000, BC-003, BC-011 |

The strict read-only BrewUp Story Guard returned `PASS` on its first run for
the exact Story 1.2 path; no fix round was required.
