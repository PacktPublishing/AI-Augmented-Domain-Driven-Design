# Storyteller 9.3 — Effective Candidate Handoff

This is a derived handoff, not a raw specialist output. It contains the eleven
candidate scenarios selected by the domain authority. ST-04 and ST-10 come from
the approved correction record; every other candidate comes from the primary
output.

Open questions remain open. Inclusion in this handoff does not change an
`unresolved` scenario status into a settled policy.

## Provenance

- Primary output SHA-256: `51ad6561fb24af5e0319ffcdef8db3ad9090a95a24c46de77af1f94891fc01c5`
- Primary decision SHA-256: `49eb7d1e492273c7cfc45cdac638c0c15eb721457c58ab73a9f9eacb2c284ffc`
- Correction output SHA-256: `289e4772dca5c8de07a169fc90e4ecc0e6cae1072bb80e5ce0e0b12a92fe5a62`
- Correction decision SHA-256: `f62ea355f28a6a86d3eb7df5b9e587b66f8f76675adabf0d960eb7dd97d07a78`

## Candidates

```yaml
candidates:
  - id: ST-01
    kind: scenario
    name: twelve cases of pale ale can be had
    statement: "Sales sends the warehouse an order for twelve cases of pale ale; Andrea reads the order number, product code, and quantity on the screen; she subtracts the pale ale quantity already on the board from the screen quantity; when twelve cases remain, Nadia counts twelve cases into the end bay; Andrea writes the order number, product code, quantity, and date on the board; and the warehouse tells Sales the beer is there."
    source:
      - ES-01
      - ES-02
      - ES-04
      - ES-05
      - ES-06
      - ES-10
      - ES-11
      - ES-12
      - ES-13
      - ES-14
      - ES-15
      - ES-16
      - ES-34
      - OBS-01
      - OBS-02
      - OBS-03
      - QUOTE-01
      - QUOTE-02
      - QUOTE-03
      - QUOTE-04
      - QUOTE-05
      - QUOTE-06
      - QUOTE-07
      - QUOTE-08
      - QUOTE-09
      - QUOTE-10
      - QUOTE-11
      - QUOTE-12
      - QUOTE-52
      - QUOTE-53
      - QUOTE-54
    status: unresolved
    decision_owner: unknown
    uncertainty: "The sequence was described directly, but the evidence does not settle whether moving the beer, writing the board row, or both together creates the hold."
    review_question: "Which business authority can decide whether the physical set-aside and the board entry are one hold or separate required facts?"

  - id: ST-02
    kind: variation
    name: Andrea refuses a short order
    statement: "Sales sends the warehouse an order for twelve cases; only eight can be had; Andrea places none of the eight in the end bay, marks the whole request not done, and returns to Sales the single response that it could not be done."
    source:
      - ES-17
      - ES-18
      - ES-20
      - QUOTE-13
      - QUOTE-14
      - QUOTE-15
      - QUOTE-16
      - QUOTE-18
      - QUOTE-19
      - QUOTE-20
    status: unresolved
    decision_owner: unknown
    uncertainty: "This is Andrea's learned practice, but it conflicts with Nadia's practice and no short-quantity rule was written down."
    review_question: "Who has authority to decide whether a request for twelve must be refused completely when only eight can be had?"

  - id: ST-03
    kind: variation
    name: Nadia sets aside the available part of a short order
    statement: "Sales sends the warehouse an order for twelve cases; only eight can be had; Nadia puts the eight in the end bay and somebody rings Sales, after which the evidence does not say whether the eight remain held or what becomes of the order."
    source:
      - ES-17
      - ES-19
      - OBS-04
      - QUOTE-13
      - QUOTE-15
      - QUOTE-17
      - QUOTE-18
      - QUOTE-19
      - QUOTE-20
    status: unresolved
    decision_owner: unknown
    uncertainty: "This is Nadia's practice, but it conflicts with Andrea's complete refusal; the evidence does not identify who must ring Sales or what decision follows."
    review_question: "Who decides whether the available eight cases are held, and what authoritative Sales response determines what happens next?"

  - id: ST-04
    kind: scenario
    name: pale ale is out; more is expected Thursday
    statement: "The warehouse checks a pale-ale request and finds the beer sold and gone, although more is expected Thursday; Andrea sends Sales only the response that the request could not be done, which does not identify the stock shortage."
    source:
      - ES-20
      - ES-21
      - ES-23
      - ES-24
      - QUOTE-21
      - QUOTE-22
      - QUOTE-23
      - QUOTE-24
      - QUOTE-25
      - QUOTE-26
    status: unresolved
    decision_owner: unknown
    uncertainty: "The evidence identifies the temporary stock shortage and says more beer is expected Thursday, but does not establish that replenishment arrives, that the shortage ends then, or that anyone retries the request."
    review_question: "Who owns the failure classification, and what response must distinguish a temporary stock shortage from other reasons a request could not be done?"

  - id: ST-05
    kind: variation
    name: product code is not a beer BrewUp makes
    statement: "The warehouse checks a request whose product code was mistyped or belongs to a dropped recipe; Andrea sends Sales the same response that it could not be done, after which the evidence does not say who corrects the code."
    source:
      - ES-20
      - ES-22
      - ES-23
      - QUOTE-21
      - QUOTE-22
      - QUOTE-23
      - QUOTE-24
      - QUOTE-25
    status: unresolved
    decision_owner: unknown
    uncertainty: "The invalid or obsolete code requires somebody to fix it, but the response conceals the reason and the evidence names neither the responsible role nor the correction process."
    review_question: "Which authority validates or corrects an invalid product code, and how must that reason be communicated to Sales?"

  - id: ST-06
    kind: scenario
    name: Thursday retry depends on somebody remembering
    statement: "After a request was returned because its beer was gone and more was expected Thursday, Nadia may remember the order and try it again on Thursday, Sales may ring and request another check, or nobody may retry it until the customer rings Sales cross."
    source:
      - ES-02
      - ES-03
      - ES-24
      - ES-25
      - QUOTE-22
      - QUOTE-26
      - QUOTE-27
      - QUOTE-28
      - QUOTE-29
      - QUOTE-30
      - QUOTE-31
      - QUOTE-32
    status: unresolved
    decision_owner: unknown
    uncertainty: "No evidence assigns ownership of the retry, confirms replenishment, or says which event authorizes another availability check; Andrea would not initiate it independently."
    review_question: "Who owns retrying the order, and what authoritative evidence on Thursday permits the warehouse to try it again?"

  - id: ST-07
    kind: scenario
    name: Nadia clears an unattended hold on Friday
    statement: "A dated hold remains on the board while its beer sits in the end bay and nobody tells the warehouse what became of the order; on a Friday Nadia may decide it has sat for a while, rub out the row, and put the beer back into general stock, where it is available to other orders."
    source:
      - ES-04
      - ES-06
      - ES-26
      - ES-27
      - ES-28
      - QUOTE-33
      - QUOTE-34
      - QUOTE-35
      - QUOTE-36
      - QUOTE-37
      - QUOTE-38
      - QUOTE-39
      - QUOTE-40
      - QUOTE-41
      - QUOTE-44
      - QUOTE-45
      - QUOTE-46
    status: unresolved
    decision_owner: unknown
    uncertainty: "There is no set holding period; Nadia varies it by customer and personal judgment, and a hold may remain for a month while she is away."
    review_question: "Who may decide that an unattended hold has expired, after what period, and may that period vary by customer?"

  - id: ST-08
    kind: variation
    name: Sales calls off a held order
    statement: "Sales tells the warehouse that a specified held order is off; Andrea rubs its row off the board; Nadia returns its beer from the end bay to general stock; and the beer immediately becomes available to anybody again."
    source:
      - ES-02
      - ES-29
      - ES-30
      - ES-31
      - ES-32
      - QUOTE-42
      - QUOTE-43
      - QUOTE-44
      - QUOTE-45
      - QUOTE-46
    status: observed
    decision_owner: Sales
    uncertainty: "The warehouse action is described directly, but the evidence does not identify who originated the cancellation or how Sales established its authority."
    review_question: "Which Sales message constitutes an authoritative cancellation that permits the board row to be removed and the beer released?"

  - id: ST-09
    kind: scenario
    name: held order turns out not to have been paid
    statement: "The warehouse sets beer aside for an order without seeing payment information; the beer remains in the end bay; the warehouse learns only from its continued presence that the order was never paid, after which the evidence does not say who acts or what happens to the hold."
    source:
      - ES-09
      - ES-33
      - OBS-06
      - QUOTE-47
      - QUOTE-48
      - QUOTE-49
    status: unresolved
    decision_owner: the office
    uncertainty: "Payment status is absent from the warehouse screen, and no reliable notification, responsible office role, or resulting warehouse action is described."
    review_question: "Which office role decides and communicates payment status, and what must the warehouse do when a held order is unpaid?"

  - id: ST-10
    kind: scenario
    name: Sales decides a held order goes through
    statement: "After the warehouse tells Sales that a particular order's beer is in the end bay, Sales decides that the order goes through; at an unspecified later time the shipping lads take the beer from the end bay, load it onto a van, and take it to the customer."
    source:
      - ES-02
      - ES-06
      - ES-07
      - ES-34
      - ES-35
      - ES-36
      - ES-37
      - OBS-06
      - QUOTE-50
      - QUOTE-51
      - QUOTE-52
      - QUOTE-53
      - QUOTE-54
    status: unresolved
    decision_owner: Sales
    uncertainty: "Sales owns the decision that the order goes through, but the evidence does not establish an instruction from Sales to Shipping, its route or timing, Shipping's authority to collect the beer, who books the van, or how much time passes."
    review_question: "Which authority releases the order for shipping, how is that authority communicated, and when should the shipping lads collect the held beer?"

  - id: ST-11
    kind: scenario
    name: four stout holds are affected by a failed batch
    statement: "In the spring, after beer from one stout batch had been put aside for four orders, the lab said the batch was not going out; the warehouse removed the four holds from the board and told the office, after which the evidence says only that the office sorted it."
    source:
      - ES-08
      - ES-09
      - ES-38
      - ES-39
      - ES-40
      - ES-41
      - OBS-05
      - OBS-06
      - QUOTE-55
      - QUOTE-56
      - QUOTE-57
      - QUOTE-58
    status: unresolved
    decision_owner: unknown
    uncertainty: "The event happened once; the evidence does not define the lab status, establish the warehouse's authority to remove the holds, say what happened to the beer or orders, or identify how the office resolved them."
    review_question: "Which lab and office authorities decide the status of the failed batch and its four orders, and what must the warehouse do with their board rows and held beer?"

summary:
  candidates_returned: 11
  unresolved: 10
  unaccounted_sources: []
  information_needed: "Authority is needed to settle short quantities, retry ownership, hold expiry, failure classification, and failed-batch handling. The evidence must also define how Sales authorizes shipping and how the office communicates payment and order resolution."
```
