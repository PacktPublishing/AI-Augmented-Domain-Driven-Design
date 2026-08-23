# Command & Event Writer 9.4 — Effective Candidate Handoff

This is a derived handoff, not a raw specialist output. It contains the 36
candidate commands and events selected by the domain authority.

Open questions remain open. Inclusion in this handoff does not change an
`unresolved` behavioural status into a settled policy, command authority, or
implementation decision.

## Provenance

- Primary output SHA-256: `5abc8b75da79572185f53e06adbc9bdcac27097ec83079c37127613a8bc10813`

## Candidates

```yaml
candidates:
  - id: CEW-01
    kind: command
    name: say whether the requested beer can be had
    statement: Sales sends the warehouse an order whose requested products and quantities are to be checked for availability.
    source:
      - QUOTE-02
      - QUOTE-03
    status: observed
    decision_owner: Sales
    uncertainty: The evidence does not identify the Sales decision that makes an order ready for this check.
    review_question: Which Sales role has authority to instruct the warehouse to check whether requested beer can be had?

  - id: CEW-02
    kind: event
    name: quantity available to hold was worked out
    statement: The warehouse worked out the quantity available to hold by subtracting the quantity on the board from the quantity on the screen for the product code.
    source:
      - QUOTE-09
      - QUOTE-10
      - QUOTE-11
      - QUOTE-12
    status: observed
    decision_owner: warehouse
    uncertainty: Andrea described this calculation, but the meaning and ownership of the screen quantity were not established.
    review_question: Which authority can confirm the required availability calculation and define the screen quantity?

  - id: CEW-03
    kind: command
    name: set aside the requested beer
    statement: When the requested quantity can be had, the warehouse physically separates that beer from general stock in the end bay.
    source:
      - QUOTE-03
      - QUOTE-04
      - QUOTE-05
    status: observed
    decision_owner: unknown
    uncertainty: Nadia carries out the instruction, but the evidence does not identify who has authority to commit beer to an order.
    review_question: Who may instruct the warehouse to set aside beer once the full requested quantity can be had?

  - id: CEW-04
    kind: event
    name: beer was set aside
    statement: Nadia counted the requested beer and moved it to the end bay so nobody else took it.
    source:
      - QUOTE-04
      - QUOTE-05
      - QUOTE-08
    status: observed
    decision_owner: warehouse
    uncertainty: The evidence does not establish whether this physical action alone creates the hold.
    review_question: Does moving beer to the end bay create the hold, or must the board entry also occur?

  - id: CEW-05
    kind: command
    name: put a hold on the beer
    statement: The warehouse records the order number, product code, quantity, and date on the board for beer set aside.
    source:
      - QUOTE-05
      - QUOTE-06
      - QUOTE-08
    status: observed
    decision_owner: unknown
    uncertainty: Andrea carries out the instruction, but the evidence does not identify who authorizes the recorded hold.
    review_question: Who may instruct Andrea to put a hold on beer, and is the physical set-aside also required?

  - id: CEW-06
    kind: event
    name: hold was put on
    statement: Andrea wrote the order number, product code, quantity, and date on the board.
    source:
      - QUOTE-05
      - QUOTE-06
      - QUOTE-08
    status: observed
    decision_owner: warehouse
    uncertainty: The speakers treated set aside, put a hold on, and earmark as the same thing despite describing separate physical and paperwork actions.
    review_question: Which business authority can decide whether the set-aside and board entry are one event or two required events?

  - id: CEW-07
    kind: command
    name: tell Sales the beer is there
    statement: After beer has been set aside, the warehouse notifies Sales that it is there for the order.
    source:
      - QUOTE-52
      - QUOTE-53
      - QUOTE-54
    status: observed
    decision_owner: warehouse
    uncertainty: The notification's required contents, form, and timing were not described.
    review_question: Which warehouse authority defines when and how Sales must be told that the beer is there?

  - id: CEW-08
    kind: event
    name: Sales was told the beer was there
    statement: The warehouse informed Sales that beer had been set aside for the order.
    source:
      - QUOTE-52
      - QUOTE-53
      - QUOTE-54
    status: observed
    decision_owner: warehouse
    uncertainty: The evidence does not establish whether Sales acknowledged or relied on this notification.
    review_question: What evidence confirms that Sales received the warehouse notification for the specified order?

  - id: CEW-09
    kind: command
    name: put the short order back as not done
    statement: Andrea marks the whole request as not done and returns it to Sales when the full requested quantity cannot be had.
    source:
      - QUOTE-13
      - QUOTE-14
      - QUOTE-15
      - QUOTE-16
      - QUOTE-20
    status: unresolved
    decision_owner: unknown
    uncertainty: Andrea was shown this practice, but it conflicts with Nadia's partial set-aside and was never written down.
    review_question: Who has authority to decide whether the warehouse must refuse a whole request when only part can be had?

  - id: CEW-10
    kind: event
    name: short order was put back as not done
    statement: Andrea returned a short order to Sales as a complete refusal without setting aside the available beer.
    source:
      - QUOTE-13
      - QUOTE-14
      - QUOTE-15
      - QUOTE-16
      - QUOTE-20
    status: unresolved
    decision_owner: unknown
    uncertainty: The outcome reflects Andrea's practice rather than an agreed short-quantity policy.
    review_question: Which authority can establish whether this is the required outcome for every short order?

  - id: CEW-11
    kind: command
    name: put the available part in the end bay
    statement: Nadia moves the available beer to the end bay when less than the requested quantity can be had.
    source:
      - QUOTE-15
      - QUOTE-17
      - QUOTE-18
      - QUOTE-19
      - QUOTE-20
    status: unresolved
    decision_owner: unknown
    uncertainty: Nadia's practice conflicts with Andrea's complete refusal, and no short-quantity policy was agreed.
    review_question: Who has authority to decide whether the available part of a short order should be set aside?

  - id: CEW-12
    kind: event
    name: available part of a short order was put in the end bay
    statement: Nadia put the available quantity into the end bay despite the request being short.
    source:
      - QUOTE-15
      - QUOTE-17
      - QUOTE-18
      - QUOTE-19
      - QUOTE-20
      - OBS-04
    status: unresolved
    decision_owner: unknown
    uncertainty: The evidence does not say whether the partial quantity remains held or what later Sales decision governs it.
    review_question: Which authority decides the status of beer partially set aside for a short order?

  - id: CEW-13
    kind: command
    name: ring Sales about the short order
    statement: After Nadia puts the available part in the end bay, somebody contacts Sales about the short quantity.
    source:
      - QUOTE-17
      - QUOTE-18
      - QUOTE-19
      - QUOTE-20
    status: unresolved
    decision_owner: unknown
    uncertainty: The evidence names neither the person who contacts Sales nor the decision Sales must return.
    review_question: Who must ring Sales about a partial set-aside, and which authority assigns that responsibility?

  - id: CEW-14
    kind: command
    name: return the request to Sales as not done
    statement: Andrea sends Sales the same response that a request could not be done whether the beer is out or the product code is invalid.
    source:
      - QUOTE-21
      - QUOTE-22
      - QUOTE-23
      - QUOTE-24
      - QUOTE-25
    status: observed
    decision_owner: warehouse
    uncertainty: The instruction is carried out, but the response does not distinguish causes that require different next actions.
    review_question: Which authority defines the reasons and contents that the not-done response must communicate?

  - id: CEW-15
    kind: event
    name: order was returned to Sales as not done
    statement: The warehouse returned a response to Sales saying only that the request could not be done.
    source:
      - QUOTE-14
      - QUOTE-21
      - QUOTE-24
      - QUOTE-25
    status: observed
    decision_owner: warehouse
    uncertainty: The response conceals whether the cause was unavailable stock, an invalid code, or another problem.
    review_question: Who owns the classification of not-done outcomes reported to Sales?

  - id: CEW-16
    kind: event
    name: beer was found to be genuinely out
    statement: The warehouse found that the requested beer had been sold and was gone while more was expected on Thursday.
    source:
      - QUOTE-21
      - QUOTE-22
      - QUOTE-23
      - QUOTE-26
    status: observed
    decision_owner: warehouse
    uncertainty: Expected replenishment does not establish that more beer arrived or that the shortage ended on Thursday.
    review_question: What authoritative evidence confirms that expected replenishment has arrived and the request may be checked again?

  - id: CEW-17
    kind: event
    name: product code was found not to be a beer BrewUp made
    statement: The warehouse found that the requested product code was mistyped or belonged to a dropped recipe.
    source:
      - QUOTE-21
      - QUOTE-22
      - QUOTE-23
    status: observed
    decision_owner: warehouse
    uncertainty: The warehouse identified the problem, but the evidence does not name who verifies or corrects the code.
    review_question: Which authority owns validation and correction of an invalid or obsolete product code?

  - id: CEW-18
    kind: command
    name: try the out-of-stock order again
    statement: Nadia may check an out-of-stock order again after expected replenishment, or Sales may ask the warehouse to look again.
    source:
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
    uncertainty: Retry depends on Nadia remembering or Sales calling, and Andrea would not initiate it independently.
    review_question: Who owns retrying an out-of-stock order, and what authoritative event permits the retry?

  - id: CEW-19
    kind: event
    name: out-of-stock order was tried again
    statement: Nadia sometimes checked an order again after delivery, while Sales sometimes prompted another check and some orders were not retried.
    source:
      - QUOTE-26
      - QUOTE-27
      - QUOTE-28
      - QUOTE-29
      - QUOTE-30
      - QUOTE-31
      - QUOTE-32
    status: unresolved
    decision_owner: unknown
    uncertainty: No evidence assigns responsibility for producing this event or proves replenishment before the check.
    review_question: Which authority can assign retry ownership and specify the evidence required before another availability check?

  - id: CEW-20
    kind: command
    name: clear an unattended hold from the board
    statement: Nadia may rub out a hold row after deciding that it has remained unattended for a while.
    source:
      - QUOTE-33
      - QUOTE-34
      - QUOTE-35
      - QUOTE-36
      - QUOTE-37
      - QUOTE-38
      - QUOTE-39
      - QUOTE-40
      - QUOTE-41
    status: unresolved
    decision_owner: unknown
    uncertainty: There is no set period, and Nadia varies her decision by customer, Friday routine, and personal judgment.
    review_question: Who may decide that an unattended hold has expired, after what period, and may that period vary by customer?

  - id: CEW-21
    kind: event
    name: unattended hold was cleared from the board
    statement: Nadia removed an old hold row because time had passed rather than because Sales cancelled the order.
    source:
      - QUOTE-34
      - QUOTE-36
      - QUOTE-37
      - QUOTE-38
      - QUOTE-39
      - QUOTE-40
      - QUOTE-41
      - QUOTE-45
      - QUOTE-46
    status: unresolved
    decision_owner: unknown
    uncertainty: The event occurred under an undocumented and inconsistent expiry practice.
    review_question: Which authority can establish the evidence that permits a time-based hold removal?

  - id: CEW-22
    kind: command
    name: put back beer from an unattended hold
    statement: After clearing an unattended hold, the warehouse returns its beer from the end bay to general stock.
    source:
      - QUOTE-34
      - QUOTE-36
      - QUOTE-40
      - QUOTE-45
      - QUOTE-46
    status: unresolved
    decision_owner: unknown
    uncertainty: The physical action is described, but its authority depends on the unsettled decision that the hold has expired.
    review_question: Which authority may instruct the warehouse to release beer from an unattended hold?

  - id: CEW-23
    kind: event
    name: unattended hold's beer was put back
    statement: The warehouse returned beer from an unattended hold to general stock, making it available to other orders.
    source:
      - QUOTE-34
      - QUOTE-40
      - QUOTE-44
      - QUOTE-45
      - QUOTE-46
    status: unresolved
    decision_owner: unknown
    uncertainty: Renewed availability follows the physical return, but the time-based authority for that return is unsettled.
    review_question: Who decides that an unattended hold's beer may become available to other orders again?

  - id: CEW-24
    kind: event
    name: order cancellation was received from Sales
    statement: Sales told the warehouse that a specified held order was off.
    source:
      - QUOTE-42
      - QUOTE-43
      - QUOTE-46
    status: observed
    decision_owner: Sales
    uncertainty: The evidence does not identify who originated the cancellation or how Sales established its authority.
    review_question: Which Sales message constitutes an authoritative cancellation for the warehouse?

  - id: CEW-25
    kind: command
    name: rub the cancelled order off the board
    statement: After receiving a cancellation from Sales, Andrea removes the specified order's row from the board.
    source:
      - QUOTE-42
      - QUOTE-43
      - QUOTE-45
      - QUOTE-46
    status: observed
    decision_owner: Sales
    uncertainty: The evidence does not say whether any validation is required beyond the Sales message.
    review_question: Which Sales authority can issue the cancellation that permits Andrea to remove the board row?

  - id: CEW-26
    kind: command
    name: put back the cancelled order's beer
    statement: After Sales calls off an order, Nadia returns its beer from the end bay to general stock.
    source:
      - QUOTE-43
      - QUOTE-44
      - QUOTE-45
      - QUOTE-46
    status: observed
    decision_owner: Sales
    uncertainty: The evidence does not describe a check between the cancellation, removal of the board row, and renewed availability.
    review_question: Who confirms that the Sales cancellation is sufficient authority to return the beer immediately?

  - id: CEW-27
    kind: event
    name: cancelled order's beer was put back
    statement: Nadia returned the cancelled order's beer to general stock, where it immediately became available to anybody again.
    source:
      - QUOTE-43
      - QUOTE-44
      - QUOTE-45
      - QUOTE-46
    status: observed
    decision_owner: warehouse
    uncertainty: The physical outcome is explicit, but the evidence does not identify any reconciliation of the end bay and board.
    review_question: Which authority can confirm the required completion evidence for releasing a cancelled order's beer?

  - id: CEW-28
    kind: event
    name: held order was found not to have been paid
    statement: The warehouse learned second-hand that an order for which beer had been set aside had never been paid.
    source:
      - QUOTE-47
      - QUOTE-48
      - QUOTE-49
      - OBS-06
    status: observed
    decision_owner: the office
    uncertainty: Payment status is absent from the warehouse screen, and no reliable notification or resulting warehouse action was described.
    review_question: Which office role determines and communicates payment status, and what authority decides what follows for the hold?

  - id: CEW-29
    kind: event
    name: order was decided to go through
    statement: Sales decided that an order went through after the warehouse reported that its beer was there.
    source:
      - QUOTE-50
      - QUOTE-51
      - QUOTE-52
      - QUOTE-53
      - QUOTE-54
    status: observed
    decision_owner: Sales
    uncertainty: The warehouse is only one dependency, and the other conditions Sales waits on were not described.
    review_question: Which Sales role decides that an order goes through, and what authoritative outcome communicates that decision?

  - id: CEW-30
    kind: command
    name: take the held beer onto a van
    statement: After the order goes through, the shipping lads take its beer from the end bay and load it onto a van.
    source:
      - QUOTE-50
      - QUOTE-51
      - QUOTE-52
      - QUOTE-53
    status: observed
    decision_owner: unknown
    uncertainty: The shipping lads carry out the instruction, but the evidence does not identify who issues it, books the van, or sets its timing.
    review_question: Which authority instructs the shipping lads to collect held beer, and when must they do so?

  - id: CEW-31
    kind: event
    name: beer was taken onto a van
    statement: The shipping lads removed held beer from the end bay, loaded it onto a van, and took it to the customer.
    source:
      - QUOTE-50
      - QUOTE-51
      - OBS-06
    status: observed
    decision_owner: the shipping lads
    uncertainty: The evidence does not establish who owned or authorized dispatch.
    review_question: Which shipping authority owns dispatch and can confirm that the held beer was released correctly?

  - id: CEW-32
    kind: event
    name: batch was said not to be going out
    statement: The lab told the warehouse that a stout batch supplying four held orders was not going out.
    source:
      - QUOTE-55
      - QUOTE-56
      - OBS-06
    status: observed
    decision_owner: the lab
    uncertainty: The exact lab status, its permanence, and its communication route were not described.
    review_question: Which lab authority defines and communicates the status meant by a batch not going out?

  - id: CEW-33
    kind: command
    name: take failed-batch holds off the board
    statement: After the lab said the stout batch was not going out, the warehouse removed the four affected order holds from the board.
    source:
      - QUOTE-56
      - QUOTE-57
      - QUOTE-58
    status: unresolved
    decision_owner: unknown
    uncertainty: The warehouse performed this action once, but Nadia said the right action had never been agreed.
    review_question: Who has authority to instruct the warehouse to remove holds affected by a failed batch?

  - id: CEW-34
    kind: event
    name: failed-batch holds were taken off the board
    statement: The warehouse removed four order holds associated with the failed stout batch.
    source:
      - QUOTE-56
      - QUOTE-57
      - QUOTE-58
    status: unresolved
    decision_owner: unknown
    uncertainty: The event happened once without an agreed policy, and the evidence does not say what happened to the beer or orders.
    review_question: Which authority can confirm whether removing the affected holds was valid and what must happen to their beer?

  - id: CEW-35
    kind: command
    name: tell the office about failed-batch holds
    statement: After removing the four affected holds, the warehouse tells the office about them.
    source:
      - QUOTE-57
      - QUOTE-58
      - OBS-05
    status: unresolved
    decision_owner: unknown
    uncertainty: The warehouse performed this handoff once, but no agreed instruction, required information, or responsible office role was described.
    review_question: Which authority requires this notification, and what information must the warehouse give the office?

  - id: CEW-36
    kind: event
    name: office was told about failed-batch holds
    statement: The warehouse notified the office after removing the four holds affected by the failed stout batch.
    source:
      - QUOTE-57
      - QUOTE-58
      - OBS-05
    status: unresolved
    decision_owner: warehouse
    uncertainty: The evidence says only that the office sorted it and does not identify the resulting decisions or outcomes.
    review_question: Which office authority receives this notification and decides what happens to the affected orders?

summary:
  candidates_returned: 36
  unresolved: 15
  unaccounted_sources: []
  information_needed: Authority is needed for short quantities, retry ownership, hold expiry, unpaid holds, and failed-batch handling. The evidence must also define hold creation, failure reporting, Sales release for shipping, and the instructions received by the shipping lads.
```
