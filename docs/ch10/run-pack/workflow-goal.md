# Workflow goal

Coordinate the complete BrewUp Stock Management modeling chain:

```text
raw evidence
  -> EventStormer -> human gate
  -> Storyteller -> human gate
  -> Command & Event Writer -> human gate
  -> Context Mapper -> human gate
  -> accepted context map
```

The workflow may finish with unresolved questions.

`Completed` means that the agreed transformations and review gates finished.
It does not mean that the domain has no remaining uncertainty.
