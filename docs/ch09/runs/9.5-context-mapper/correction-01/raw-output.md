candidates:
  - id: CM-10
    kind: relationship
    name: order request from Sales
    statement: Customer orders and order progression passes an order number, requested products, and quantities to beer availability and holds for an availability decision.
    source:
      - QUOTE-02
      - QUOTE-03
    status: observed
    decision_owner: Sales
    uncertainty: The evidence does not say what Sales decision makes the request ready or whether the displayed request is complete and authoritative.
    review_question: Which Sales authority defines when an order request is ready and what information it must contain?

  - id: CM-16
    kind: relationship
    name: failed-batch holds notification
    statement: In the one known incident, beer availability and holds notified failed-batch order resolution after removing holds affected by a failed batch.
    source:
      - QUOTE-56
      - QUOTE-58
      - OBS-05
    status: observed
    decision_owner: unknown
    uncertainty: The notification happened once, and the evidence does not establish its required contents, who has authority to decide what is passed, or what downstream decisions must follow.
    review_question: Which authority decides what affected-order information must be passed for failed-batch order resolution and what decisions follow?

  - id: CM-17
    kind: relationship
    name: Shipping collection of held beer
    statement: Customer delivery physically collects held beer from beer availability and holds after Sales decides that the order goes through.
    source:
      - QUOTE-51
      - QUOTE-53
    status: observed
    decision_owner: unknown
    uncertainty: The evidence does not establish the instruction route, who selects or releases the held beer, when collection is permitted, or who authorizes delivery.
    review_question: Which authority decides what held beer Shipping may collect, and what evidence authorizes its collection and delivery?

summary:
  candidates_returned: 3
  unresolved: 0
  unaccounted_sources: []
  information_needed: Evidence is needed from the authorities responsible for failed-batch information and Shipping collection or delivery authorization. Sales must also confirm when an order request is ready and what it must contain.