# Invisible Traces - Design Rationale and Iteration Record

This document records why the prototype changed, not only what changed. It is
intended to support the DDES9902 criteria for problem diagnosis,
reconceptualisation design, ethical consideration and credible work history.

## Problem diagnosis

Peanut cross-contamination in a shared kitchen is difficult for a novice to
reason about because the hazard is normally invisible. It also spreads through
space, contact and sequence: a worker can clean one object while unknowingly
moving residue to a hand, tool, ingredient or preparation area. A checklist or
video can state the correct procedure, but cannot reveal how the learner's own
spatial decisions create a contact chain.

## Why an immersive prototype

The design makes the learner act inside a spatial model of the task. They must
inspect separate risk and safe-equipment areas, decide what to touch, carry
objects, and observe the consequences of contact. This supports embodied
cause-and-effect learning that a passive explanation cannot test.

The experience remains a low-fidelity greybox because the assessment value is
in the interaction model and reasoning. Simple geometry keeps the causal
relationships legible and protects WebGL performance.

## Elegant design mechanism: Trace Mode

Trace Mode is one deliberately simple mechanism that addresses several
non-trivial problems:

1. It makes an invisible allergen pathway inspectable.
2. It links an unsafe outcome to prior spatial contact rather than appearance.
3. It supports safe failure, investigation, reset and retry.
4. It uses both magenta highlighting and an `ALLERGEN TRACE` label, so the
   critical state is not communicated by colour alone.
5. It turns feedback into a debrief: failure names the full contact chain and
   success names the sequence that broke that chain.

## Procedural fidelity

Cleaning and handwashing are not independent checklist boxes. If the learner
washes first and then cleans the contaminated worktop, the hand state becomes
unsafe again. They must wash after controlling the surface. This small state
transition is important: it teaches that sequence changes risk.

## Ethical and safety decisions

- The scene identifies itself as a training simulation, not a food-safety
  certification or substitute for workplace procedures.
- Unsafe choices cause explanatory feedback, not exposure to a real allergen.
- The user can reset and retry without punishment, time pressure or startling
  effects.
- Redundant colour and text encoding reduces dependence on colour perception.
- The experience avoids claiming that a short prototype proves workplace
  competence.

## Iteration history

| Iteration | Evidence or problem | Design response |
| --- | --- | --- |
| V1 | Correct and unsafe paths existed, but controls were hard to find. | Consolidated primary actions on the front counter. |
| V2 | Users needed clearer orientation and outcome feedback. | Added numbered steps, a control guide, persistent progress and causal messages. |
| V3 | WebGL testing showed text overlap and poor viewing distance. | Shortened labels, moved detail into hover feedback and adjusted the starting view. |
| V4 | The procedure treated cleaning and handwashing as independent. | Made cleaning invalidate an earlier handwash and explicitly taught sequence. |
| V5 | Magenta alone excluded some users and outcomes lacked reflection. | Added text hazard labels, safety boundaries and success/failure contact-chain debriefs. |

## Intended evaluation questions

- Can a first-time user identify the initial contaminated objects?
- Can they explain why washing before cleaning does not complete the safe path?
- After one unsafe attempt, can they name the contact chain that caused it?
- On a second attempt, can they complete the safe sequence without prompting?
- Can users identify hazards without relying on colour alone?
