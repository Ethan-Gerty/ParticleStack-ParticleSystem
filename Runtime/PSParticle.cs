using UnityEngine;

public struct PSParticle
{
    public Vector2 position;
    public Vector2 velocity;

    public float zRotation;
    public float angularVelocity;

    public Vector2 scale;
    public Color colour;

    public float lifeTime;
    public float age;

    public bool isAlive => age < lifeTime;
}