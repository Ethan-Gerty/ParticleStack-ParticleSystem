# ParticleStack

**ParticleStack is a lightweight, code-first 2D particle system for Unity built around simple, modular components and easily programmable particle behaviour.**

ParticleStack is designed as a smaller, more understandable alternative to Unity's built-in Particle System for projects that do not need a huge VFX toolset. Instead of hiding particle behaviour behind a large inspector, ParticleStack keeps the system close to code and lets effects be assembled from focused components.

## Features

- Lightweight particle data stored as `PSParticle` structs
- Preallocated particle pool with reusable `SpriteRenderer` objects
- Burst and ongoing emission
- Circle-based emission shape
- Modular particle behaviours
- Simple API for creating custom behaviours, emissions, and shapes
- Configurable lifetime, speed, scale, colour, rotation, sprite, material, and sorting
- No `Rigidbody2D` required for basic particle movement

### Included Components

**Core**
- `PSEmitter` — owns, updates, removes, and renders particles
- `PSParticle` — stores the data for an individual particle
- `PSBehaviour` — base class for custom particle behaviours
- `PSEmission` — base class for custom emission types
- `PSShape` — base class for custom emission shapes

**Emissions**
- `PSBurstEmission` — emits a configurable burst of particles
- `PSOngoingEmission` — continuously emits particles at a configurable rate

**Shapes**
- `PSCircleShape` — emits particles around a configurable circular radius range

**Behaviours**
- `PSRandomColourBeh`
- `PSRandomLifetimeBeh`
- `PSRandomScaleBeh`
- `PSRandomSpeedBeh`

## Basic Setup

1. Add `PSEmitter` to a GameObject.
2. Assign a sprite and configure the particle settings on the emitter.
3. Add a `PSShape`, such as `PSCircleShape`.
4. Add an emission component:
   - `PSBurstEmission` for bursts
   - `PSOngoingEmission` for continuous emission
5. Add any optional `PSBehaviour` components to modify particles when they spawn or while they are alive.

A basic effect might look like:

```text
Particle Effect
├── PSEmitter
├── PSCircleShape
├── PSBurstEmission
├── PSRandomScaleBeh
└── PSRandomColourBeh
```

## Custom Particle Behaviours

ParticleStack is intended to make custom particle logic easy to write.

Create a component that inherits from `PSBehaviour` and override either `OnParticleSpawn` or `UpdateParticle`.

```csharp
using UnityEngine;

public class MyParticleBehaviour : PSBehaviour
{
    public override void OnParticleSpawn(ref PSParticle particle)
    {
        // Modify the particle when it is created.
    }

    public override void UpdateParticle(ref PSParticle particle, float deltaTime)
    {
        // Modify the particle while it is alive.
    }
}
```

The particle is passed by reference, so its data can be modified directly.

Available particle data currently includes:

```csharp
particle.position;
particle.velocity;
particle.zRotation;
particle.angularVelocity;
particle.scale;
particle.colour;
particle.lifeTime;
particle.age;
particle.isAlive;
```

Add the custom behaviour to the same GameObject as the `PSEmitter` and ParticleStack will automatically include it in the simulation.

## Custom Emissions

To define a new way of deciding **when** particles are emitted, inherit from `PSEmission`.

Your emission can call:

```csharp
EmitParticle();
```

whenever it wants to create a particle. ParticleStack handles creating the particle from the current emitter settings and selected shape.

This can be used to create things such as timed emissions, gameplay-triggered emissions, rhythm-based emissions, or any project-specific system.

## Custom Shapes

To define **where** particles spawn and the direction they initially travel, inherit from `PSShape` and implement:

```csharp
public override void GetSpawnData(
    out Vector2 position,
    out Vector2 direction
)
```

This keeps shape logic separate from emission timing and particle behaviour.

## Design Philosophy

ParticleStack focuses on:

- **Code-first control** — particle behaviour should remain easy to understand and modify in C#.
- **Small components** — each script should have one clear responsibility.
- **Extensibility** — project-specific particle behaviour should be easy to add without modifying ParticleStack itself.
- **Minimal hidden behaviour** — the path from emission to simulation to rendering should remain straightforward.
- **Only use what you need** — effects are built by adding the behaviours, emissions, and shapes they actually use.

ParticleStack is not intended to reproduce every feature of Unity's built-in Particle System. Its goal is to provide a simpler foundation for lightweight 2D effects while leaving unusual or game-specific particle behaviour in the programmer's hands.

## Current Status

ParticleStack is currently an early-stage project and its API and feature set may change as development continues.

# Author

Created by **Ethan Gerty** as a code-focused particle system designed around clarity, control, and simplistic particle behaviour.

GitHub: https://github.com/Ethan-Gerty
