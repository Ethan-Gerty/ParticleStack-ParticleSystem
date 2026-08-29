# ParticleStack

**ParticleStack is a lightweight, code-first 2D particle system for Unity built around simple components, GPU-instanced rendering, and easily programmable particle behaviour.**

ParticleStack is designed for programmers who want a smaller, more direct alternative to Unity's built-in Particle System for 2D effects.

Instead of placing every possible feature inside one large particle component, ParticleStack separates effects into focused systems for **emission**, **shape**, **behaviour**, and **rendering**. The built-in components cover common use cases, while the abstract `PSBehaviour`, `PSEmission`, and `PSShape` classes make project-specific extensions straightforward.

ParticleStack is not intended to match Unity's Particle System or VFX Graph feature-for-feature. Its goal is to stay **small, understandable, performant, and close to code**.

---

## Current Version

### v0.3.0

v0.3.0 expands ParticleStack's core effect-building tools with additional shapes and a larger set of built-in behaviours.

The system continues to use the GPU-instanced renderer introduced in v0.2.0, allowing particles to remain lightweight data rather than individual GameObjects, Transforms, or SpriteRenderers.

ParticleStack is still in early development and its API may change between versions.

---

# Features

## Core

- Lightweight `PSParticle` struct-based particle data
- Preallocated particle storage
- Reusable particle slots
- Centralised particle simulation
- Automatic dead-particle removal using packed-array replacement
- No GameObject per particle
- No MonoBehaviour per particle
- No Rigidbody2D required for normal particle movement

## Rendering

- GPU-instanced mesh rendering
- Batched rendering in groups of up to 1023 instances
- Custom `ParticleStack/Instanced` shader
- Per-particle colour support
- Sprite-based particle mesh generation
- Runtime material instancing

## Emission

- `PSBurstEmission`
- `PSOngoingEmission`
- High-rate ongoing emission using an accumulator so fractional emissions are preserved between frames
- Extensible `PSEmission` base class

## Shapes

- `PSCircleShape`
- `PSLineShape`
- `PSConeShape`
- `PSBoxShape`
- Extensible `PSShape` base class

## Behaviours

### Randomisation

- `PSRandomColourBeh`
- `PSRandomLifetimeBeh`
- `PSRandomScaleBeh`
- `PSRandomSpeedBeh`

### Motion

- `PSGravityBeh`
- `PSForceBeh`
- `PSDragBeh`
- `PSVelocityOverLifetimeBeh`

### Over Lifetime

- `PSScaleOverTimeBeh`
- `PSColourOverTimeBeh`

## Configurable Particle Properties

- Lifetime
- Speed
- Scale
- Colour
- Rotation
- Angular velocity
- Maximum particle count
- Sprite
- Material

---

# Design Philosophy

ParticleStack is built around a few core ideas.

## Keep It Small

A particle effect should be understandable by looking at the components attached to its GameObject.

For example:

```text
Smoke
├── PSEmitter
├── PSOngoingEmission
├── PSCircleShape
├── PSDragBeh
├── PSScaleOverTimeBeh
└── PSColourOverTimeBeh
```

Each component has one clear responsibility.

---

## Keep It Close to Code

ParticleStack is designed primarily for programmers.

An individual particle is simply data:

```csharp
particle.position;
particle.velocity;

particle.zRotation;
particle.angularVelocity;

particle.scale;
particle.colour;

particle.lifeTime;
particle.age;
```

Behaviours work directly with that data rather than interacting with a hidden particle object or Rigidbody.

---

## Make Custom Behaviour Easy

ParticleStack deliberately does not include a specialised component for every possible effect.

Instead, users can extend the same base classes used by ParticleStack itself:

```text
PSBehaviour  → controls what particles do
PSEmission   → controls when particles are emitted
PSShape      → controls where particles spawn and their initial direction
```

There is no separate advanced extension API.

If a project needs homing particles, orbiting particles, gameplay-reactive particles, a custom spawn pattern, or an unusual emission trigger, it can be implemented as a normal ParticleStack component.

---

# Architecture

```text
PSEmission
    │
    │ decides when a particle should be created
    ▼

PSShape
    │
    │ provides spawn position and direction
    ▼

PSParticle
    │
    │ contains particle state
    ▼

PSEmitter
    │
    ├── stores particles
    ├── simulates particles
    ├── runs behaviours
    ├── removes dead particles
    └── prepares instance data
    │
    ├──────────────► PSBehaviour
    │                modifies particle state
    │
    ▼

GPU-Instanced Renderer
    │
    ├── transform matrices
    ├── per-particle colours
    └── ParticleStack shader
```

---

# Basic Setup

Create a GameObject and add:

1. `PSEmitter`
2. One `PSEmission`
3. One `PSShape`
4. Any optional `PSBehaviour` components

For example:

```text
Particle Effect
├── PSEmitter
├── PSBurstEmission
├── PSConeShape
├── PSGravityBeh
├── PSDragBeh
└── PSColourOverTimeBeh
```

On `PSEmitter`, configure the particle settings and assign a sprite and compatible material.

---

# Emission

## PSBurstEmission

Emits a configurable number of particles at once.

The current implementation bursts when the effect starts and again when the component is re-enabled after it has started.

It can also be triggered directly through:

```csharp
burstEmission.Burst();
```

Useful for:

- Explosions
- Impacts
- Enemy deaths
- Spell effects
- Hit particles

---

## PSOngoingEmission

Continuously emits particles using a configurable particles-per-second rate.

The emitter uses an accumulated fractional spawn count rather than relying on a simple timer. This allows high emission rates to produce multiple particles in a single frame when required and preserves fractional emissions between frames.

Useful for:

- Smoke
- Fire
- Rain
- Magic effects
- Environmental particles
- Trails

---

# Shapes

## PSCircleShape

Spawns particles at a random radius inside a configurable radius range, with particles travelling radially away from the emitter.

A radius range of zero effectively behaves as a point emitter, while larger values can create rings or radial bands.

---

## PSLineShape

Spawns particles along a line centred on the emitter.

The line follows the GameObject's local right axis and particles travel along its local up direction, so rotating the GameObject rotates the complete effect.

---

## PSConeShape

Emits particles within a configurable angular spread around the emitter's local up direction.

The radius controls how far from the emitter the particles begin.

Useful for:

- Fire
- Exhaust
- Sprays
- Weapon effects
- Directional magic

---

## PSBoxShape

Spawns particles from the edges of a configurable rectangular shape and emits them outward from the selected edge.

---

# Particle Behaviours

`PSBehaviour` exposes two extension points:

```csharp
public override void OnParticleSpawn(ref PSParticle particle)
{
}
```

and:

```csharp
public override void UpdateParticle(
    ref PSParticle particle,
    float deltaTime
)
{
}
```

ParticleStack automatically discovers `PSBehaviour` components attached to the same GameObject as the emitter.

---

## Random Behaviours

### PSRandomColourBeh

Selects a random colour from a configured array when the particle spawns.

### PSRandomLifetimeBeh

Assigns a random lifetime from a configurable range.

### PSRandomScaleBeh

Randomises the particle's X and Y scale from separate ranges.

### PSRandomSpeedBeh

Randomises the particle's initial speed while preserving its emission direction.

---

## Motion Behaviours

### PSGravityBeh

Applies downward acceleration to particles over time.

### PSForceBeh

Applies a constant force in a configurable direction.

### PSDragBeh

Applies frame-rate-independent exponential damping to particle velocity.

### PSVelocityOverLifetimeBeh

Interpolates particle velocity from its starting value toward a target velocity over its lifetime.

---

## Over-Time Behaviours

### PSScaleOverTimeBeh

Interpolates particle scale from its starting scale toward a target scale over its lifetime.

### PSColourOverTimeBeh

Interpolates particle colour from its starting colour toward a target colour over its lifetime.

---

# Creating a Custom Behaviour

Create a script that inherits from `PSBehaviour`:

```csharp
using UnityEngine;

public class MyParticleBehaviour : PSBehaviour
{
    public override void UpdateParticle(
        ref PSParticle particle,
        float deltaTime
    )
    {
        // Change the particle however you want.
    }
}
```

Add it to the same GameObject as `PSEmitter`.

ParticleStack treats custom behaviours exactly like its built-in behaviours.

---

# Creating a Custom Emission

Create a class that inherits from `PSEmission`.

When your custom trigger decides a particle should be created, call:

```csharp
EmitParticle();
```

The base emission system requests spawn data from the current `PSShape`, constructs the `PSParticle`, and sends it to the emitter.

This can be used for things such as:

- Gameplay-triggered emission
- Random intervals
- Rhythm-based emission
- Distance-based emission
- Custom timed patterns

---

# Creating a Custom Shape

Create a class that inherits from `PSShape` and implement:

```csharp
public override void GetSpawnData(
    out Vector2 position,
    out Vector2 direction
)
{
}
```

A shape only needs to provide:

- the particle's spawn position
- the particle's initial direction

Everything else remains independent of the shape.

---

# Rendering

ParticleStack does not create a renderer object for every particle.

Instead:

```text
PSParticle[]
    ↓
Particle Simulation
    ↓
Matrix4x4 Transform Data
+
Particle Colour Data
    ↓
GPU Instance Batches
    ↓
ParticleStack/Instanced Shader
```

The renderer converts each active particle into a transform matrix and colour value and submits the instances in batches.

This avoids maintaining thousands of:

```text
GameObjects
Transforms
SpriteRenderers
MonoBehaviours
```

for visual particles.

---

# Particle Storage

Particles are stored in a preallocated array.

When a particle dies, the last active particle is copied into its slot:

```text
Before:

[A] [B] [C] [D] [E]
     ↑
     dies

After:

[A] [E] [C] [D]
```

This keeps active particles packed together and avoids shifting every later element in the array.

The freed slot can then be reused by a future particle.

---

# What ParticleStack Is For

ParticleStack is particularly suited to:

- 2D games
- Indie games
- Pixel-art games
- Gameplay-driven particle effects
- Programmers who prefer direct C# control
- Projects that want a small and understandable particle architecture

ParticleStack is not intended to replace Unity's Particle System or VFX Graph in every use case.

Those systems provide much more mature tooling and a far larger feature set.

ParticleStack instead focuses on being:

> **Small, understandable, modular, performant, and easy to extend.**

---

# Status

ParticleStack is in active early development.

### Current release: `v0.3.0`

The API and project structure may change as development continues.

---

## ParticleStack

**Particle effects without the giant particle system.**

---

# Author

Created by **Ethan Gerty** as a code driven particle system built for simplicity and easy to use, programmable components.

GitHub: https://github.com/Ethan-Gerty