# Context Mapper 9.5 — Effective Candidate Handoff

This is a derived handoff, not a raw specialist output. It contains the 18
candidate boundaries and relationships selected by the domain authority.
CM-10, CM-16, and CM-17 come from the approved correction record; every other
candidate comes from the primary output.

Open questions remain open. Inclusion in this handoff does not change an
`unresolved` specialist status into settled policy, an approved boundary name,
or an implementation decision.

## Provenance

- Primary output SHA-256: `84c779c250b3fb33f71531d240e135d3603cfbc81a2ab330be22f71f794e2c85`
- Correction output SHA-256: `d2849bc6b2a9ced704b55cd6fcc2069793cb4914eb04fdecb640d0ac1a70142d`
- Human decision SHA-256: `abd873a1121d8ddfad7cda3ac069165fadea64f04c6b507543b81915d49adadb`

## Candidates

```yaml
candidates:
- id: CM-01
  kind: boundary
  name: beer availability and holds
  statement: This boundary decides whether requested beer can be had, sets it aside,
    records and clears holds, and returns released beer to general stock.
  source:
  - QUOTE-03
  - QUOTE-05
  - QUOTE-06
  - QUOTE-12
  - QUOTE-34
  - QUOTE-43
  - QUOTE-44
  status: inferred
  decision_owner: warehouse
  uncertainty: The business names the warehouse rather than this responsibility-based
    boundary, and authority over short quantities, hold creation, and unattended-hold
    expiry remains unsettled.
  review_question: Which business authority can confirm the extent of the proposed
    beer availability and holds boundary and assign its unresolved policies?
- id: CM-02
  kind: boundary
  name: customer orders and order progression
  statement: This boundary takes customer orders, sends requests and cancellations,
    receives availability responses, and decides whether an order goes through.
  source:
  - QUOTE-02
  - QUOTE-29
  - QUOTE-43
  - QUOTE-52
  - QUOTE-53
  status: inferred
  decision_owner: Sales
  uncertainty: The business calls this area Sales rather than the proposed responsibility-based
    name, and the evidence does not identify the Sales roles or other conditions involved
    in progressing an order.
  review_question: Which Sales authority can confirm this boundary's responsibilities
    and the conditions under which an order goes through?
- id: CM-03
  kind: boundary
  name: customer ordering
  statement: This boundary originates customer orders and later enquiries about orders
    that have not progressed.
  source:
  - QUOTE-02
  - QUOTE-29
  status: inferred
  decision_owner: customer
  uncertainty: The evidence describes customer actions only second-hand through Sales
    and does not use customer ordering as a boundary name.
  review_question: Who can provide direct evidence defining the customer's ordering
    responsibilities and the information exchanged with Sales?
- id: CM-04
  kind: boundary
  name: batch testing and sale release
  statement: This boundary tests a batch and decides whether beer from that batch
    may go out.
  source:
  - QUOTE-55
  - QUOTE-56
  status: inferred
  decision_owner: the lab
  uncertainty: The business calls this area the lab rather than the proposed responsibility-based
    name, and its testing criteria and full extent are not described.
  review_question: Which lab authority can confirm the proposed boundary and the decision
    that a batch may or may not go out?
- id: CM-05
  kind: boundary
  name: payment administration
  statement: This boundary holds responsibility for payment matters that the warehouse
    neither sees nor decides.
  source:
  - QUOTE-47
  - QUOTE-48
  - QUOTE-49
  status: inferred
  decision_owner: the office
  uncertainty: The business refers only to the office, and the evidence does not define
    its payment process, roles, or authority over unpaid orders.
  review_question: Which office authority owns payment status and can define what
    happens when held beer belongs to an unpaid order?
- id: CM-06
  kind: boundary
  name: failed-batch order resolution
  statement: This boundary deals with the consequences for orders whose held beer
    belongs to a failed batch.
  source:
  - QUOTE-56
  - QUOTE-58
  - OBS-05
  status: unresolved
  decision_owner: unknown
  uncertainty: The warehouse said the office sorted the only known incident, but nobody
    described an agreed responsibility, decision, or outcome, and the proposed boundary
    name is not used by the business.
  review_question: Which authority owns failed-batch order resolution, and what decisions
    fall within that responsibility?
- id: CM-07
  kind: boundary
  name: customer delivery
  statement: This boundary removes released beer from the end bay, loads it onto a
    van, and takes it to the customer.
  source:
  - QUOTE-50
  - QUOTE-51
  status: inferred
  decision_owner: unknown
  uncertainty: The business names the shipping lads rather than this responsibility-based
    boundary, and the evidence does not identify who owns shipping instructions or
    van booking.
  review_question: Which authority owns customer delivery and authorizes collection
    of a particular held order?
- id: CM-08
  kind: boundary
  name: product code correction
  statement: This boundary would verify or correct a mistyped or obsolete product
    code that the warehouse cannot fulfil.
  source:
  - QUOTE-21
  - QUOTE-22
  - QUOTE-23
  status: unresolved
  decision_owner: unknown
  uncertainty: The evidence says somebody must fix the code but names neither a responsibility
    area nor an authority and shows no corrected code passing back.
  review_question: Which authority owns product code validation and correction, and
    what information must it receive and return?
- id: CM-09
  kind: relationship
  name: customer order and follow-up
  statement: A customer passes an order, and potentially a later enquiry about an
    unprogressed order, to customer orders and order progression through Sales.
  source:
  - QUOTE-02
  - QUOTE-29
  status: observed
  decision_owner: customer
  uncertainty: Both interactions are reported second-hand, and their exact contents
    and channels are not described.
  review_question: Which customer or Sales authority can confirm the authoritative
    contents of an order and a later follow-up?
- id: CM-10
  kind: relationship
  name: order request from Sales
  statement: Customer orders and order progression passes an order number, requested
    products, and quantities to beer availability and holds for an availability decision.
  source:
  - QUOTE-02
  - QUOTE-03
  status: observed
  decision_owner: Sales
  uncertainty: The evidence does not say what Sales decision makes the request ready
    or whether the displayed request is complete and authoritative.
  review_question: Which Sales authority defines when an order request is ready and
    what information it must contain?
- id: CM-11
  kind: relationship
  name: order cancellation from Sales
  statement: Customer orders and order progression passes notice that a specified
    held order is off to beer availability and holds.
  source:
  - QUOTE-42
  - QUOTE-43
  status: observed
  decision_owner: Sales
  uncertainty: The evidence does not identify who originated the cancellation or how
    its authority is established.
  review_question: Which Sales message constitutes an authoritative cancellation permitting
    the hold to be cleared and its beer released?
- id: CM-12
  kind: relationship
  name: request to retry an order
  statement: Customer orders and order progression may pass a request to beer availability
    and holds to check an out-of-stock order again.
  source:
  - QUOTE-26
  - QUOTE-29
  - QUOTE-30
  - QUOTE-31
  - QUOTE-32
  status: unresolved
  decision_owner: unknown
  uncertainty: Sales sometimes asks for another check, but retry ownership and the
    event authorizing a retry are not assigned.
  review_question: Which authority owns retrying an order, and what Sales request
    or replenishment evidence authorizes another availability check?
- id: CM-13
  kind: relationship
  name: warehouse order response
  statement: Beer availability and holds passes Sales a response that beer is there
    or that the request could not be done.
  source:
  - QUOTE-14
  - QUOTE-21
  - QUOTE-24
  - QUOTE-25
  - QUOTE-53
  - QUOTE-54
  status: observed
  decision_owner: warehouse
  uncertainty: The negative response does not distinguish unavailable stock from an
    invalid product code, and the required response contents have not been decided.
  review_question: Which authority defines the classifications and information that
    the warehouse order response must carry?
- id: CM-14
  kind: relationship
  name: short-quantity notice
  statement: After a partial set-aside, somebody passes Sales information that the
    requested quantity was short.
  source:
  - QUOTE-15
  - QUOTE-17
  - QUOTE-18
  - QUOTE-19
  - QUOTE-20
  status: unresolved
  decision_owner: unknown
  uncertainty: The evidence identifies neither the sender, the contents of the notice,
    nor the Sales decision expected in return.
  review_question: Who must notify Sales about a partial set-aside, and which authority
    decides what that notice contains?
- id: CM-15
  kind: relationship
  name: batch not going out
  statement: Batch testing and sale release passes beer availability and holds the
    decision that a specified batch is not going out.
  source:
  - QUOTE-55
  - QUOTE-56
  status: observed
  decision_owner: the lab
  uncertainty: The evidence describes one incident and does not define the authoritative
    message, batch identification, or notification process.
  review_question: Which lab authority defines the decision notice that requires affected
    holds to be removed?
- id: CM-16
  kind: relationship
  name: failed-batch holds notification
  statement: In the one known incident, beer availability and holds notified failed-batch
    order resolution after removing holds affected by a failed batch.
  source:
  - QUOTE-56
  - QUOTE-58
  - OBS-05
  status: observed
  decision_owner: unknown
  uncertainty: The notification happened once, and the evidence does not establish
    its required contents, who has authority to decide what is passed, or what downstream
    decisions must follow.
  review_question: Which authority decides what affected-order information must be
    passed for failed-batch order resolution and what decisions follow?
- id: CM-17
  kind: relationship
  name: Shipping collection of held beer
  statement: Customer delivery physically collects held beer from beer availability
    and holds after Sales decides that the order goes through.
  source:
  - QUOTE-51
  - QUOTE-53
  status: observed
  decision_owner: unknown
  uncertainty: The evidence does not establish the instruction route, who selects
    or releases the held beer, when collection is permitted, or who authorizes delivery.
  review_question: Which authority decides what held beer Shipping may collect, and
    what evidence authorizes its collection and delivery?
- id: CM-18
  kind: relationship
  name: beer delivered to customer
  statement: Customer delivery passes the released beer to the customer.
  source:
  - QUOTE-50
  - QUOTE-51
  status: observed
  decision_owner: unknown
  uncertainty: The evidence says the shipping lads take beer to customers but does
    not describe receipt, delivery completion, or who decides what constitutes successful
    delivery.
  review_question: Which authority defines the beer and delivery information passed
    to the customer and confirms delivery completion?
summary:
  candidates_returned: 18
  unresolved: 4
  unaccounted_sources: []
  information_needed: Direct evidence is needed from Sales, the lab, the office, shipping,
    customers, and whoever owns product codes. The largest gaps concern short quantities,
    retries, hold expiry, payment, shipment authorization, and failed-batch consequences.
```
