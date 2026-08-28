# Storyteller autonomy policy

Output kind: `candidate-scenarios`

Allowed evidence kinds:

- `raw-evidence`
- `accepted-facts`

May introduce new candidate terms: yes

Maximum correction attempts: 2

Timeout: 30 seconds

Allowed tools:

- `read-accepted-artifact`
- `read-evidence`

A correction request changes the reason for rerunning the transformation. It does
not expand the specialist's authority.
