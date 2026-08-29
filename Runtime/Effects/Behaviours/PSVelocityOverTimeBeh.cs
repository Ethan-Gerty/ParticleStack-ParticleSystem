using UnityEngine;

public class PSVelocityOverLifetimeBeh : PSBehaviour
{
    [SerializeField] private Vector2 velocityTo;

    private Vector2 startVelocity;

    public override void OnParticleSpawn(ref PSParticle particle)
    {
        startVelocity = particle.velocity;
    }

    public override void UpdateParticle(ref PSParticle particle, float deltaTime)
    {
        float percentage = particle.age / particle.lifeTime;

        particle.velocity = Vector2.Lerp(startVelocity, velocityTo, percentage);
    }
}