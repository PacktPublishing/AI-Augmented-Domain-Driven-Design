# BrewUp — Modeling Principles

An excerpt from the project constitution. It states how modeling work is
expected to be done. It says nothing about any particular bounded context.

---

The domain is the heart of the system and must be modeled explicitly using DDD
building blocks: aggregates, entities, value objects, domain events, and domain
services that speak the ubiquitous language of their context.

- State changes must be expressed as domain events. Communication across
  contexts uses explicit commands and events, never shared tables or implicit
  coupling.

- Each bounded context must speak its own ubiquitous language. Generic
  terminology is not acceptable when a domain-specific term exists.

- Domain ownership must be explicit. A context may depend on a decision produced
  by another context, but it must not own or reproduce that decision unless it
  has been explicitly assigned.

- Unknown business policy must remain visible as an open question. Policy must
  not be invented to make a model look complete.
