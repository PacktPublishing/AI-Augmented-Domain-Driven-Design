candidates:
  - id: ES-01
    kind: participant
    name: warehouse
    statement: "The warehouse was the group receiving order information, checking beer, setting it aside, recording holds, and returning beer to general stock."
    source:
      - OBS-01
      - QUOTE-01
      - QUOTE-02
      - QUOTE-03
    status: observed
    decision_owner: unknown
    uncertainty: "The evidence describes Nadia and Andrea working together but does not establish a formal boundary or named owner for the warehouse."
    review_question: "Who has authority to confirm the warehouse's responsibilities and boundary?"

  - id: ES-02
    kind: participant
    name: Sales
    statement: "Sales dealt with customers, sent order and cancellation information to the warehouse, received warehouse responses, and decided whether an order went through."
    source:
      - QUOTE-02
      - QUOTE-14
      - QUOTE-17
      - QUOTE-29
      - QUOTE-43
      - QUOTE-52
      - QUOTE-53
    status: observed
    decision_owner: Sales
    uncertainty: "The evidence does not identify which roles within Sales performed or owned these decisions."
    review_question: "Which Sales role has authority over orders, cancellations, and the decision that an order goes through?"

  - id: ES-03
    kind: participant
    name: customer
    statement: "A customer put an order in through Sales and could later call when an unresolved order had not been retried."
    source:
      - QUOTE-02
      - QUOTE-29
    status: observed
    decision_owner: customer
    uncertainty: "The warehouse did not communicate directly with customers, and the evidence does not describe the customer's process."
    review_question: "Who can confirm which customer actions are communicated to the warehouse through Sales?"

  - id: ES-04
    kind: participant
    name: the board
    statement: "The board held handwritten order numbers, product codes, quantities, and sometimes dates for beer already spoken for."
    source:
      - OBS-02
      - QUOTE-05
      - QUOTE-06
      - QUOTE-10
      - QUOTE-12
      - QUOTE-33
    status: observed
    decision_owner: warehouse
    uncertainty: "No evidence establishes whether every physical set-aside was recorded or what overwritten rows represented."
    review_question: "Who can confirm the board's required contents and whether it is the complete record of current holds?"

  - id: ES-05
    kind: participant
    name: the screen
    statement: "The screen showed order information received from Sales and a quantity against each product code."
    source:
      - OBS-01
      - QUOTE-02
      - QUOTE-10
    status: observed
    decision_owner: unknown
    uncertainty: "The source and meaning of the displayed quantity were not explained."
    review_question: "Who owns the screen quantity and can define exactly what it represents?"

  - id: ES-06
    kind: participant
    name: end bay
    statement: "The end bay held beer separated from general stock for customer orders."
    source:
      - QUOTE-05
      - QUOTE-17
      - QUOTE-34
      - QUOTE-43
      - QUOTE-44
      - QUOTE-50
      - QUOTE-51
    status: observed
    decision_owner: warehouse
    uncertainty: "The evidence does not say whether every hold had to be placed in the end bay."
    review_question: "Who can confirm whether physical placement in the end bay is required for every hold?"

  - id: ES-07
    kind: participant
    name: the shipping lads
    statement: "The shipping lads took beer from the end bay, loaded it onto a van, and took it to customers."
    source:
      - QUOTE-50
      - QUOTE-51
    status: observed
    decision_owner: unknown
    uncertainty: "The evidence does not identify who instructed shipping or owned van booking."
    review_question: "Who authorizes the shipping lads to collect an order and owns the shipment?"

  - id: ES-08
    kind: participant
    name: the lab
    statement: "The lab tested batches and told the warehouse when a batch was not going out."
    source:
      - QUOTE-55
      - QUOTE-56
    status: observed
    decision_owner: the lab
    uncertainty: "The evidence describes the lab's decision second-hand and does not describe its criteria."
    review_question: "Which lab authority decides that a batch may not go out and communicates that decision?"

  - id: ES-09
    kind: participant
    name: the office
    statement: "The office handled administrative matters including payment and was asked to sort the consequences of a failed batch."
    source:
      - QUOTE-47
      - QUOTE-48
      - QUOTE-49
      - QUOTE-58
      - OBS-05
    status: observed
    decision_owner: unknown
    uncertainty: "The office was not defined as a specific group, and both warehouse speakers deferred to it without describing its practice."
    review_question: "Which office role owns payment information and resolution of orders affected by a failed batch?"

  - id: ES-10
    kind: trigger
    name: order came through from Sales
    statement: "An order arriving from Sales prompted the warehouse to determine whether the requested quantity could be had."
    source:
      - QUOTE-01
      - QUOTE-02
      - QUOTE-03
    status: observed
    decision_owner: Sales
    uncertainty: "The evidence does not say what prior Sales decision caused the order to be sent."
    review_question: "What Sales decision or state makes an order ready to be sent to the warehouse?"

  - id: ES-11
    kind: fact
    name: order request was received
    statement: "The warehouse was told by Sales that a customer had put an order in, including its number, requested products, and quantities; the warehouse did not bring the customer order about."
    source:
      - QUOTE-01
      - QUOTE-02
      - QUOTE-03
      - OBS-06
    status: observed
    decision_owner: Sales
    uncertainty: "The evidence does not say whether receipt on the screen was complete, reliable, or separately acknowledged."
    review_question: "Who can confirm the authoritative contents and receipt point of an order request?"

  - id: ES-12
    kind: fact
    name: quantity available to hold was worked out
    statement: "Andrea brought about the availability calculation by subtracting the quantity on the board for a product code from the quantity on the screen."
    source:
      - QUOTE-09
      - QUOTE-10
      - QUOTE-11
      - QUOTE-12
    status: observed
    decision_owner: warehouse
    uncertainty: "The evidence gives the calculation used by Andrea but does not establish it as an agreed business rule or explain the screen quantity."
    review_question: "Who has authority to confirm how the quantity available to hold must be calculated?"

  - id: ES-13
    kind: trigger
    name: enough beer could be had
    statement: "Finding enough beer for the requested quantity prompted the warehouse to separate it and record a hold."
    source:
      - QUOTE-03
      - QUOTE-04
      - QUOTE-05
      - QUOTE-06
    status: observed
    decision_owner: warehouse
    uncertainty: "The evidence does not identify a separate approval between the availability check and the hold."
    review_question: "Who confirms that finding the full requested quantity is sufficient authority to place the hold?"

  - id: ES-14
    kind: fact
    name: beer was set aside
    statement: "Nadia brought about the physical set-aside by counting the requested beer and moving it to the end bay so nobody else took it."
    source:
      - QUOTE-04
      - QUOTE-05
      - QUOTE-08
    status: observed
    decision_owner: warehouse
    uncertainty: "The evidence does not say whether Nadia alone had authority to set beer aside or whether another check was required."
    review_question: "Who has authority to commit beer to an order by moving it to the end bay?"

  - id: ES-15
    kind: fact
    name: hold was put on
    statement: "Andrea brought about the recorded hold by writing the order number, product code, quantity, and date on the board."
    source:
      - QUOTE-05
      - QUOTE-06
      - QUOTE-08
    status: observed
    decision_owner: warehouse
    uncertainty: "The evidence does not settle whether the board entry itself creates the hold or merely records Nadia's physical action."
    review_question: "Does authority to create a hold lie in the physical set-aside, the board entry, or both together?"

  - id: ES-16
    kind: fact
    name: set aside, put a hold on, and earmark were treated as the same thing
    statement: "The warehouse speakers treated 'set aside', 'put a hold on', and 'earmark' as interchangeable even though the phrases described physical and paperwork actions."
    source:
      - QUOTE-07
      - QUOTE-08
      - OBS-03
    status: unresolved
    decision_owner: unknown
    uncertainty: "The speakers treated the terms as one action, while their descriptions may distinguish a physical change from its record; no canonical language was agreed."
    review_question: "Which business authority can decide whether these are one fact or separate facts and select the accepted term or terms?"

  - id: ES-17
    kind: trigger
    name: requested quantity was short
    statement: "Having less beer available than the requested quantity prompted either a complete refusal or a partial set-aside, depending on who was working."
    source:
      - QUOTE-13
      - QUOTE-15
      - QUOTE-16
      - QUOTE-17
      - QUOTE-18
      - QUOTE-19
    status: unresolved
    decision_owner: unknown
    uncertainty: "The short-quantity policy was never written down, and Andrea and Nadia described different practices without treating either as wrong."
    review_question: "Who has authority to decide what must happen when the available quantity is less than the requested quantity?"

  - id: ES-18
    kind: fact
    name: short order was put back as not done
    statement: "Andrea described bringing about a complete refusal when the full requested quantity was unavailable by marking the order as not done and returning it to Sales."
    source:
      - QUOTE-13
      - QUOTE-14
      - QUOTE-15
      - QUOTE-16
      - QUOTE-20
    status: unresolved
    decision_owner: unknown
    uncertainty: "This was Andrea's learned practice, but it conflicted with Nadia's practice and was not an agreed rule."
    review_question: "Should the warehouse refuse the whole request when only part of the requested quantity is available, and who decides?"

  - id: ES-19
    kind: fact
    name: available part of a short order was put in the end bay
    statement: "Nadia described bringing about a partial set-aside by moving the available beer to the end bay and having somebody ring Sales."
    source:
      - QUOTE-15
      - QUOTE-17
      - QUOTE-18
      - QUOTE-19
      - QUOTE-20
      - OBS-04
    status: unresolved
    decision_owner: unknown
    uncertainty: "This conflicted with Andrea's complete-refusal practice, and there was no decision on record about short quantities."
    review_question: "Should available beer be held for a short order, and which authority decides whether Sales must be contacted?"

  - id: ES-20
    kind: fact
    name: order was returned to Sales as not done
    statement: "Andrea brought about a warehouse response saying the request could not be done, which went back to Sales."
    source:
      - QUOTE-14
      - QUOTE-21
      - QUOTE-24
      - QUOTE-25
    status: observed
    decision_owner: warehouse
    uncertainty: "The response did not communicate why the request could not be done."
    review_question: "Who owns the meaning and required contents of the 'could not be done' response?"

  - id: ES-21
    kind: fact
    name: beer was genuinely out
    statement: "The warehouse determined that a request could not be done because the beer had been sold and was gone, while more stock might be coming later."
    source:
      - QUOTE-21
      - QUOTE-22
      - QUOTE-23
    status: observed
    decision_owner: warehouse
    uncertainty: "The screen response did not distinguish this temporary stock problem from an invalid product code."
    review_question: "Who can confirm how a genuine out-of-stock result must be distinguished and communicated?"

  - id: ES-22
    kind: fact
    name: product code was not a beer BrewUp made
    statement: "The warehouse determined that a request could not be done because its product code was mistyped or belonged to a dropped recipe."
    source:
      - QUOTE-21
      - QUOTE-22
      - QUOTE-23
    status: observed
    decision_owner: unknown
    uncertainty: "The evidence does not identify who verifies or corrects the code, and the warehouse response concealed this reason."
    review_question: "Who has authority to validate and correct an invalid or obsolete product code?"

  - id: ES-23
    kind: fact
    name: reasons for a no were reported as the same result
    statement: "Andrea brought about the same 'could not be done' response for both unavailable stock and an invalid product code."
    source:
      - QUOTE-21
      - QUOTE-22
      - QUOTE-23
      - QUOTE-24
      - QUOTE-25
    status: observed
    decision_owner: unknown
    uncertainty: "The evidence says the problems require different responses but supplies no agreed classification or reporting rule."
    review_question: "Who decides which failure reasons Sales must receive and how they must be distinguished?"

  - id: ES-24
    kind: trigger
    name: more beer arrived
    statement: "The expected arrival of more beer could prompt somebody to look again at an order that had previously been out of stock."
    source:
      - QUOTE-26
      - QUOTE-27
      - QUOTE-29
    status: unresolved
    decision_owner: unknown
    uncertainty: "No owner, reliable reminder, or agreed retry trigger was identified."
    review_question: "Who must initiate a retry after replenishment, and what authoritative notification prompts it?"

  - id: ES-25
    kind: fact
    name: out-of-stock order was tried again
    statement: "Nadia sometimes brought about a retry when she remembered an order after delivery, while Sales sometimes prompted another check and some orders may never have been retried."
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
    uncertainty: "Responsibility for retrying was undocumented, depended on memory or a Sales call, and Andrea would not initiate it independently."
    review_question: "Which role owns retrying an out-of-stock order after replenishment, and who has authority to assign that responsibility?"

  - id: ES-26
    kind: trigger
    name: time had passed
    statement: "The passage of an informal amount of time prompted Nadia to clear holds from the board."
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
    uncertainty: "There was no set holding period; timing varied by Friday routines, customer treatment, Nadia's judgment, and her presence."
    review_question: "Who has authority to establish when an unattended hold expires and whether customer-specific variation is allowed?"

  - id: ES-27
    kind: fact
    name: board was cleared because time had passed
    statement: "Nadia brought about removal of old hold rows on her own judgment, mostly on Fridays, because time had passed rather than because somebody had cancelled the order."
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
    uncertainty: "No duration or authority was written down, and holds could remain for a month when Nadia was away."
    review_question: "Who may decide that elapsed time is sufficient to clear a hold, and what evidence must support that decision?"

  - id: ES-28
    kind: fact
    name: beer was put back after a hold was cleared
    statement: "The warehouse brought about the return of beer from the end bay to general stock after deciding that an unattended hold had remained too long."
    source:
      - QUOTE-34
      - QUOTE-36
      - QUOTE-40
      - QUOTE-45
      - QUOTE-46
    status: unresolved
    decision_owner: unknown
    uncertainty: "The physical outcome was described, but the policy authorizing the time-based decision was missing."
    review_question: "Which authority decides when a time-based release makes beer available to other orders again?"

  - id: ES-29
    kind: trigger
    name: order was called off
    statement: "A cancellation received from Sales prompted Andrea to remove the order from the board and Nadia to return its beer."
    source:
      - QUOTE-42
      - QUOTE-43
    status: observed
    decision_owner: Sales
    uncertainty: "The evidence does not say who initiated the cancellation or how Sales authenticated it."
    review_question: "Which Sales decision constitutes an authoritative order cancellation for the warehouse?"

  - id: ES-30
    kind: fact
    name: order was called off
    statement: "The warehouse was told by Sales that a specified order was off; the warehouse did not bring the cancellation about."
    source:
      - QUOTE-42
      - QUOTE-43
      - QUOTE-46
      - OBS-06
    status: observed
    decision_owner: Sales
    uncertainty: "The evidence does not say whether the customer, Sales, or another office role originated the cancellation."
    review_question: "Who has authority to cancel an order before Sales tells the warehouse?"

  - id: ES-31
    kind: fact
    name: cancelled order was rubbed off the board
    statement: "Andrea brought about removal of a cancelled order's board row after receiving the cancellation from Sales."
    source:
      - QUOTE-43
      - QUOTE-45
      - QUOTE-46
    status: observed
    decision_owner: Sales
    uncertainty: "Andrea executed the removal, but the authority for it came from the Sales cancellation."
    review_question: "Who can confirm that a Sales cancellation is the only authority needed to remove the board row?"

  - id: ES-32
    kind: fact
    name: cancelled order's beer was put back
    statement: "Nadia brought about the return of a cancelled order's beer to general stock, where it became available to anybody again."
    source:
      - QUOTE-43
      - QUOTE-44
      - QUOTE-45
      - QUOTE-46
    status: observed
    decision_owner: Sales
    uncertainty: "The evidence does not describe any check between removal from the board and renewed availability."
    review_question: "Who confirms that cancellation immediately makes the returned beer available to other orders?"

  - id: ES-33
    kind: fact
    name: order was found not to have been paid for
    statement: "The warehouse was told or discovered second-hand that some orders for which beer had been set aside had never been paid; the warehouse did not decide payment status."
    source:
      - QUOTE-47
      - QUOTE-48
      - QUOTE-49
      - OBS-06
    status: observed
    decision_owner: the office
    uncertainty: "Payment was absent from the warehouse screen, and the evidence does not describe a reliable notification or the precise office owner."
    review_question: "Which office role decides payment status and must communicate it to the warehouse?"

  - id: ES-34
    kind: fact
    name: Sales was told the beer was there
    statement: "The warehouse brought about a notification to Sales that beer had been set aside and was there for the order."
    source:
      - QUOTE-52
      - QUOTE-53
      - QUOTE-54
    status: observed
    decision_owner: warehouse
    uncertainty: "The evidence does not describe the notification's form, timing, or required contents."
    review_question: "Who owns and verifies the warehouse notification that beer is available for an order?"

  - id: ES-35
    kind: fact
    name: order went through
    statement: "The warehouse was told or observed that an order went through after Sales decided it should; the warehouse did not bring that decision about."
    source:
      - QUOTE-50
      - QUOTE-51
      - QUOTE-52
      - QUOTE-53
      - QUOTE-54
      - OBS-06
    status: observed
    decision_owner: Sales
    uncertainty: "The warehouse knew it was only one of the things Sales waited on and did not know the other conditions."
    review_question: "Which Sales authority decides that an order goes through, and what other conditions does that decision depend on?"

  - id: ES-36
    kind: trigger
    name: order went through properly
    statement: "Sales deciding that an order had gone through prompted the shipping lads to take its beer from the end bay."
    source:
      - QUOTE-50
      - QUOTE-51
      - QUOTE-52
      - QUOTE-53
    status: observed
    decision_owner: Sales
    uncertainty: "The evidence does not describe how the shipping lads received this trigger."
    review_question: "What authoritative Sales message releases held beer to shipping?"

  - id: ES-37
    kind: fact
    name: beer was taken onto a van
    statement: "The warehouse observed the shipping lads, not warehouse staff, taking held beer from the end bay onto a van."
    source:
      - QUOTE-50
      - QUOTE-51
      - OBS-06
    status: observed
    decision_owner: unknown
    uncertainty: "The evidence does not identify who booked the van or held authority for dispatch."
    review_question: "Who owns and authorizes dispatch after Sales decides that an order goes through?"

  - id: ES-38
    kind: trigger
    name: batch failed its check
    statement: "The lab saying that a batch was not going out prompted action on four orders holding beer from that batch."
    source:
      - QUOTE-55
      - QUOTE-56
      - OBS-05
    status: observed
    decision_owner: the lab
    uncertainty: "The warehouse received the lab decision second-hand and the evidence does not describe the notification route."
    review_question: "What lab decision and communication authoritatively trigger action on held beer from a failed batch?"

  - id: ES-39
    kind: fact
    name: batch was said not to be going out
    statement: "The warehouse was told that the lab had failed a batch of stout and decided it was not going out; the warehouse did not bring the batch decision about."
    source:
      - QUOTE-56
      - OBS-06
    status: observed
    decision_owner: the lab
    uncertainty: "The evidence does not say whether the lab rejected the batch permanently or imposed another restriction."
    review_question: "Which lab authority can define the exact status created when a batch is said not to be going out?"

  - id: ES-40
    kind: fact
    name: failed batch holds were taken off the board
    statement: "The warehouse brought about removal of four order holds from the board after being told that their stout batch was not going out."
    source:
      - QUOTE-56
      - QUOTE-57
      - QUOTE-58
    status: unresolved
    decision_owner: unknown
    uncertainty: "This happened once, and the speakers said the right warehouse practice had never been agreed."
    review_question: "Who has authority to decide what the warehouse must do with holds affected by a failed batch?"

  - id: ES-41
    kind: fact
    name: office was told about failed batch holds
    statement: "The warehouse brought about a notification to the office after removing holds affected by the failed batch, and the office was then said to have sorted it."
    source:
      - QUOTE-57
      - QUOTE-58
      - OBS-05
    status: unresolved
    decision_owner: unknown
    uncertainty: "No warehouse practice, office action, or responsible office role was described, and no agreed rule existed."
    review_question: "Which office authority owns resolution of affected orders, and what must the warehouse hand on after a batch failure?"

summary:
  candidates_returned: 41
  unresolved: 11
  unaccounted_sources: []
  information_needed: "Business authorities must settle the short-quantity, retry, hold-expiry, terminology, and failed-batch practices. The warehouse also needs authoritative definitions for screen quantities, failure reporting, payment notification, Sales release, and shipping handoff."