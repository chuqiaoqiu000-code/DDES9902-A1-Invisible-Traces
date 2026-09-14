# Invisible Traces

An immersive, low-fidelity allergen cross-contamination training prototype for
DDES9902 Assessment 1. The experience places a novice hospitality worker in a
shared cafe kitchen immediately after a peanut-butter order was prepared.

## Design question

How might spatial interaction and visualised contamination traces improve a
novice worker's understanding of allergen cross-contamination?

Cross-contamination is spatial, contact-based, sequential, and normally
invisible. A conventional video can state a safe procedure, but it cannot test
how a learner applies that procedure to tools, surfaces, and ingredients in a
shared workspace. This prototype lets the learner act, see the consequence of
contact, fail safely, and immediately retry.

## Core interaction

`Trace Mode` reveals contaminated objects in magenta. It externalises an
otherwise invisible chain of contact so the learner can connect an outcome to
its spatial and procedural cause.

The safe route is:

1. Inspect the workspace using Trace Mode.
2. Clean the shared worktop.
3. Wash hands.
4. Place the clean blue knife, bread, and vegetables in the preparation zone.

Placing a contaminated item in the zone produces an unsafe outcome and a
specific causal explanation.

## Controls

- WASD: move
- Mouse: look and aim the centre reticle
- Left click: activate, carry, or place
- L: reload the scene

## Unity setup

- Unity 6.3 LTS, editor `6000.3.8f1`
- EZPZ Interaction Toolkit
- Main scene:
  `Assets/__My Project/InvisibleTraces/InvisibleTraces_Main.unity`

To regenerate the authored greybox scene, use the Unity menu:

`DDES9902 > Build Invisible Traces Prototype`

## Prototype scope

This is deliberately greyboxed. It prioritises a reliable interaction model,
clear causal feedback, WebGL viability, and evidence of design reasoning over
high-resolution assets or commercial polish.

## Iteration evidence

- V1 established the correct and unsafe interaction paths.
- Initial user testing found that Reset and Trace Mode were difficult to find.
- V2 places all primary actions on the same counter, adds numbered steps and an
  in-world control guide, strengthens outcome colours, and supports contaminant
  transfer while an EZPZ Holdable is being carried.
- WebGL testing revealed that text which was readable in the Editor overlapped
  at the embedded browser viewport. V3 increases the initial viewing distance,
  shortens persistent labels, and moves object detail into contextual hover
  feedback.
