candidates:
  - id: CM-01
    kind: boundary
    name: beer availability and holds
    statement: The warehouse responsibility decides whether requested beer can be had and carries out the set-aside, recorded-hold, and return-to-general-stock work, while Sales decides whether the customer order goes through.
    source:
      - QUOTE-03
      - QUOTE-43
      - QUOTE-53
    status: inferred
    decision_owner: warehouse
    uncertainty: The business did not use "beer availability and holds" as a boundary name, and the evidence leaves hold duration, short-quantity policy, and some release authority unsettled.
    review_question: Which warehouse authority can confirm that availability and hold responsibilities belong in one boundary, and what should the business call it?

  - id: CM-02
    kind: boundary
    name: order decisions
    statement: Sales owns the customer-order decision, sends order and cancellation information, receives warehouse availability information, and decides whether an order goes through.
    source:
      - QUOTE-02
      - QUOTE-43
      - QUOTE-53
      - QUOTE-54
    status: inferred
    decision_owner: Sales
    uncertainty: The business names Sales but does not name this responsibility boundary or identify the Sales role that owns each decision.
    review_question: Which Sales authority confirms the extent and business name of the responsibility that decides whether an order goes through and sends cancellations?

  - id: CM-03
    kind: boundary
    name: payment decisions
    statement: Payment status belongs outside the warehouse responsibility and is attributed to the office, while the warehouse learns about unpaid orders only second-hand.
    source:
      - QUOTE-48
      - QUOTE-49
      - OBS-06
    status: inferred
    decision_owner: the office
    uncertainty: The business did not use "payment decisions" as a boundary name, the responsible office role is not identified, and no direct exchange with the warehouse is established.
    review_question: Which office role owns payment status, what is this responsibility called, and is there any direct information exchange with the warehouse?

  - id: CM-04
    kind: boundary
    name: delivery
    statement: The shipping lads take held beer from the end bay, load it onto a van, and deliver it after the order has gone through, outside the warehouse responsibility.
    source:
      - QUOTE-51
      - OBS-06
    status: inferred
    decision_owner: unknown
    uncertainty: Delivery is a proposed responsibility name; the evidence identifies the actors but not the dispatch authority, van-booking authority, or formal boundary.
    review_question: Which authority owns dispatch and delivery, and what does the business call that responsibility?

  - id: CM-05
    kind: boundary
    name: batch check
    statement: The lab decides that a batch has failed its check and tells the warehouse that the batch is not going out.
    source:
      - QUOTE-56
      - OBS-06
    status: inferred
    decision_owner: the lab
    uncertainty: Batch check is a proposed responsibility name; the evidence does not describe the lab's decision criteria, formal outcome, or communication route.
    review_question: Which lab authority owns the batch decision, and what business name should identify that responsibility?

  - id: CM-06
    kind: boundary
    name: failed-batch order handling
    statement: After the warehouse reports holds affected by a failed batch, the office sorts the consequences for those orders.
    source:
      - QUOTE-58
      - OBS-05
    status: unresolved
    decision_owner: the office
    uncertainty: The business did not name this responsibility, the office role and decisions are unspecified, and the evidence does not establish whether this is the same responsibility as payment or a different one.
    review_question: Which office authority owns the consequences of a failed batch for affected orders, and is that responsibility separate from payment handling?

  - id: CM-07
    kind: relationship
    name: order request sent for availability
    statement: "Order decisions -> beer availability and holds: Sales passes the customer order number, product codes, and requested quantities so the warehouse can say whether the beer can be had."
    source:
      - QUOTE-02
      - QUOTE-03
    status: observed
    decision_owner: Sales
    uncertainty: The evidence does not say what prior Sales state or decision makes the request ready to send.
    review_question: Which Sales authority decides that an order request is ready to be passed for an availability answer?

  - id: CM-08
    kind: relationship
    name: availability passed back to Sales
    statement: "Beer availability and holds -> order decisions: the warehouse tells Sales that the requested beer can be had or that the beer is there, and Sales retains the decision about what happens to the order."
    source:
      - QUOTE-03
      - QUOTE-53
      - QUOTE-54
    status: observed
    decision_owner: warehouse
    uncertainty: The required contents, timing, and form of the availability information are not described.
    review_question: Which warehouse authority defines the availability information that is passed to Sales and when it is complete?

  - id: CM-09
    kind: relationship
    name: cancellation passed from Sales
    statement: "Order decisions -> beer availability and holds: Sales tells the warehouse that a specified order is off, after which the warehouse removes its recorded hold and returns the beer."
    source:
      - QUOTE-43
    status: observed
    decision_owner: Sales
    uncertainty: The evidence does not identify who originated the cancellation or the exact Sales message that makes it authoritative.
    review_question: Which Sales authority decides the cancellation and what information must cross the boundary for the warehouse to act on it?

  - id: CM-10
    kind: relationship
    name: failed-batch decision passed from the lab
    statement: "Batch check -> beer availability and holds: the lab tells the warehouse that a batch already supplying held orders is not going out."
    source:
      - QUOTE-56
    status: observed
    decision_owner: the lab
    uncertainty: The exact lab status, its permanence, and the communication route are not described.
    review_question: Which lab authority defines the failed-batch outcome and what exact fact is passed to the warehouse?

  - id: CM-11
    kind: relationship
    name: failed-batch hold information passed to the office
    statement: "Beer availability and holds -> failed-batch order handling: after removing the affected holds, the warehouse tells the office about them, but the evidence does not establish the required handoff or what the office must do next."
    source:
      - QUOTE-58
      - OBS-05
    status: unresolved
    decision_owner: unknown
    uncertainty: The handoff happened once without an agreed practice, required information, or identified authority deciding what must be passed.
    review_question: Which authority requires this handoff, what information must be passed, and which office role receives it?

  - id: CM-12
    kind: relationship
    name: held beer taken for delivery
    statement: "Beer availability and holds -> delivery: after Sales decides that the order goes through, the shipping lads take the held beer from the end bay onto a van; the evidence does not show the warehouse creating, requesting, or authorizing a shipment."
    source:
      - QUOTE-51
      - QUOTE-53
      - OBS-06
    status: observed
    decision_owner: unknown
    uncertainty: The physical movement is observed, but no dispatch instruction, owner, direct message, van-booking responsibility, or timing is established.
    review_question: Which authority authorizes shipping to collect the held beer, and what fact or instruction allows that collection to begin?

summary:
  candidates_returned: 12
  unresolved: 2
  unaccounted_sources: []
  information_needed: Domain authority is needed to confirm the responsibility-based boundary names and extents, identify dispatch and office roles, and decide whether payment and failed-batch order handling are separate responsibilities. No direct payment-to-warehouse relationship or Sales-to-delivery relationship is drawn because the evidence does not establish one.
