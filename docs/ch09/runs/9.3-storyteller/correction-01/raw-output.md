candidates:
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

summary:
  candidates_returned: 2
  unresolved: 2
  unaccounted_sources: []
  information_needed: "Authority is needed to classify temporary stock shortages and to define the route and timing by which Shipping may collect a held order. Evidence is also needed before relying on expected replenishment or any retry."