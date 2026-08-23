candidates:
  - id: ES-24
    kind: trigger
    name: more beer was expected
    statement: "Expecting more beer on a stated day could prompt Nadia to retry an order that had previously been out of stock."
    source:
      - QUOTE-22
      - QUOTE-26
      - QUOTE-29
    status: unresolved
    decision_owner: unknown
    uncertainty: "The evidence reports only an expected replenishment and does not establish that beer arrived, who owns the retry, or what authoritative evidence may trigger it."
    review_question: "Who initiates a retry, and what authoritative replenishment evidence may trigger it?"

  - id: ES-29
    kind: trigger
    name: order cancellation was received from Sales
    statement: "Receiving an order cancellation from Sales prompted Andrea to remove the board row and Nadia to return the beer to general stock."
    source:
      - QUOTE-42
      - QUOTE-43
      - QUOTE-46
    status: observed
    decision_owner: Sales
    uncertainty: "The warehouse received and acted on the cancellation but did not cause it; the evidence does not identify who originated it or how Sales established its authority."
    review_question: "Which Sales decision or message constitutes an authoritative order cancellation for the warehouse?"

summary:
  candidates_returned: 2
  unresolved: 1
  unaccounted_sources: []
  information_needed: "The retry owner and authoritative replenishment evidence remain unsettled. Sales must confirm what constitutes an authoritative cancellation received by the warehouse."