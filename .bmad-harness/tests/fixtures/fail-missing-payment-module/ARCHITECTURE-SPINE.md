# Invalid Sales Order Confirmation Architecture

Payment owns payment authorization, and Payment behavior is implemented in
this repository. Sales stores only `PaymentAuthorizationId`, but the
architecture defines no physical Payment module or Payment projects.
