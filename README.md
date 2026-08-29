# ParticleStack

**ParticleStack is a lightweight, code-first 2D particle system for Unity focused on simplicity, performance, and easy customisation through code.**

ParticleStack was created as a smaller, more understandable alternative to Unity's built-in Particle System.

Rather than putting every possible particle feature into one large component, ParticleStack separates effects into small, focused systems for **emission**, **shape**, **behaviour**, and **rendering**.

The goal is not to recreate every feature of Unity's Particle System.

The goal is to provide a simple foundation that handles the common particle-system infrastructure while making it easy for programmers to create whatever project-specific behaviour they need.

---

## Current Version

### v0.2.0

v0.2.0 is the first version of ParticleStack considered a proper usable release.

The original prototype renderer has been completely replaced with a **GPU-instanced mesh renderer**, removing the need for individual particle GameObjects and SpriteRenderers.

ParticleStack is still early in development and its API may change between versions.

---

# Features

- Lightweight data-based particles using `PSParticle`
- GPU-instanced particle rendering
- Batched rendering for large particle counts
- Custom ParticleStack instanced shader
- Per-particle colour support
- Preallocated particle storage
- Reusable particle slots
- No GameObject per particle
- No MonoBehaviour per particle
- No Rigidbody2D required for particle movement
- Burst emission
- Ongoing emission
- Circle emission shape
- Configurable:
  - Lifetime
  - Speed
  - Scale
  - Colour
  - Rotation
  - Angular velocity
- Modular particle behaviours
- Custom particle behaviour support
- Custom emission support
- Custom shape support
- Random colour behaviour
- Random lifetime behaviour
- Random scale behaviour
- Random speed behaviour

---

# Design Philosophy

ParticleStack is built around a few core ideas.

## Keep It Simple

Particle effects should be understandable by looking at the components attached to the GameObject.

For example:

```text
Fire Effect
├── PSEmitter
├── PSOngoingEmission
├── PSCircleShape
├── PSRandomLifetimeBeh
├── PSRandomScaleBeh
└── PSRandomColourBeh
```

Each component has one clear responsibility.

---

## Keep It Close to Code

ParticleStack is designed primarily for programmers.

Particle behaviour is intentionally exposed through simple C# classes rather than hidden behind a large editor interface.

A particle is ultimately just data:

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

Custom systems can directly manipulate this data.

---

## Make Custom Behaviour Easy

ParticleStack does not attempt to include a built-in component for every possible effect.

Instead, it provides simple base classes that users can extend:

```text
PSBehaviour
PSEmission
PSShape
```

Built-in ParticleStack functionality uses the same systems available to users.

There is no separate "advanced custom API".

---

# Architecture

ParticleStack separates particle effects into several main systems:

```text
PSEmission
    │
    │ decides when particles are created
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
    └── removes dead particles
    │
    ▼

PSBehaviour
    │
    │ modifies particles
    ▼

GPU Instanced Renderer
```

---

# Basic Setup

Create a GameObject and add:

1. `PSEmitter`
2. A `PSEmission`
3. A `PSShape`

Then add any optional `PSBehaviour` components.

For example:

```text
Particle Effect
├── PSEmitter
├── PSBurstEmission
├── PSCircleShape
├── PSRandomSpeedBeh
└── PSRandomColourBeh
```

Assign a sprite and ParticleStack-compatible material to the emitter and configure the particle settings.

---

# Emission

Emission components control **when particles are created**.

ParticleStack currently includes:

### PSBurstEmission

Creates multiple particles at once.

Useful for:

- Explosions
- Impact effects
- Enemy deaths
- Spell impacts
- Hit particles

### PSOngoingEmission

Continuously creates particles at a configurable rate.

Useful for:

- Fire
- Smoke
- Magic effects
- Trails
- Environmental particles

---

# Shapes

Shapes control:

- Where a particle spawns
- Its initial direction

ParticleStack currently includes:

### PSCircleShape

Produces particle spawn information using a circular shape.

Shapes are kept separate from emission so the same emission system can be used with completely different spawn patterns.

---

# Particle Behaviours

`PSBehaviour` allows particles to be modified either when they spawn or while they are alive.

A custom behaviour can override:

```csharp
public override void OnParticleSpawn(ref PSParticle particle)
{

}
```

and/or:

```csharp
public override void UpdateParticle(
    ref PSParticle particle,
    float deltaTime
)
{

}
```

For example:

```csharp
using UnityEngine;

public class MyParticleBehaviour : PSBehaviour
{
    public override void UpdateParticle(
        ref PSParticle particle,
        float deltaTime
    )
    {
        particle.velocity += Vector2.down * deltaTime;
    }
}
```

Add the behaviour to the same GameObject as the `PSEmitter` and ParticleStack will automatically include it in the simulation.

This makes it easy to create project-specific effects such as:

- Gravity
- Attraction
- Repulsion
- Homing
- Orbiting
- Wind
- Wobbling
- Gameplay-reactive particles

without changing ParticleStack itself.

---

# Custom Emissions

Custom emission systems can be created by inheriting from:

```csharp
PSEmission
```

An emission decides **when particles should be created**.

The base emission class handles particle creation, allowing custom emissions to simply call:

```csharp
EmitParticle();
```

This can be used to create systems such as:

- Timed emissions
- Random interval emissions
- Gameplay-triggered emissions
- Rhythm-based emissions
- Distance-based emissions

---

# Custom Shapes

Custom spawn shapes can be created by inheriting from:

```csharp
PSShape
```

and implementing:

```csharp
public override void GetSpawnData(
    out Vector2 position,
    out Vector2 direction
)
{

}
```

ParticleStack does not need to know how the shape works.

It only needs a spawn position and direction.

This makes it possible to create shapes such as:

```text
Point
Box
Cone
Line
Ring
Character Outline
Custom Path
```

or anything specific to the game using ParticleStack.

---

# Rendering

ParticleStack does not create a GameObject or SpriteRenderer for every particle.

Particles exist only as data inside the emitter.

Rendering follows roughly this process:

```text
PSParticle[]
    ↓
Particle Simulation
    ↓
Transform Matrices
+
Particle Colours
    ↓
GPU Instance Batches
    ↓
ParticleStack Shader
```

Large groups of particles sharing the same mesh and material are submitted together using GPU instancing.

This avoids the overhead of maintaining thousands of:

```text
GameObjects
Transforms
SpriteRenderers
MonoBehaviours
```

for visual particles.

---

# Particle Lifetime

Particles are stored inside a preallocated array.

When a particle dies, ParticleStack does not shift the entire collection.

Instead, the last active particle is moved into the empty position.

```text
Before:

[A] [B] [C] [D] [E]
     ↑
     dies

After:

[A] [E] [C] [D]
```

Particle order is not important, allowing removal to remain inexpensive.

Particle slots are then reused by future emissions.

---

# What ParticleStack Is For

ParticleStack is particularly aimed at:

- 2D games
- Indie games
- Pixel-art games
- Gameplay-driven particle effects
- Programmers who prefer working directly with code
- Projects that do not need the full complexity of Unity's Particle System

ParticleStack is **not** intended to replace Unity's Particle System or VFX Graph for every possible use case.

Unity's built-in systems provide significantly more functionality and mature tooling.

ParticleStack instead focuses on being:

> **Small, understandable, modular, performant, and easy to extend.**

---

# Planned Development

Future versions may explore features such as:

- Additional shapes
- Additional built-in behaviours
- Colour over lifetime
- Scale over lifetime
- Rotation over lifetime
- Drag and gravity behaviours
- Improved particle value/randomisation controls
- Better runtime controls
- Editor warnings and debugging tools
- Further rendering and simulation optimisation

Features will be added when they provide broadly useful functionality rather than simply attempting to match Unity's Particle System feature-for-feature.

---

# Status

ParticleStack is currently in active early development.

### Current release: `v0.2.0`

The API and project structure may change as the system develops.

---

## ParticleStack

**Particle effects without the giant particle system.**

---

# Author

Created by **Ethan Gerty** as a gameplay-focused animation system designed around clarity, control, and predictable frame-based behaviour.

GitHub: https://github.com/Ethan-Gerty

