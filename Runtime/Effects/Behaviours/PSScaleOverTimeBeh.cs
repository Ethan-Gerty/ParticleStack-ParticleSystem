using UnityEngine;

public class PSScaleOverTimeBeh : PSBehaviour
{
    [SerializeField] private Vector2 scaleTo;

    private Vector2 startScale; 

    public override void OnParticleSpawn(ref PSParticle particle)
    {
        startScale = particle.scale;
    }

    public override void UpdateParticle(ref PSParticle particle, float deltaTime)
    {
        float percentage = particle.age / particle.lifeTime;

        particle.scale = Vector2.Lerp(startScale, scaleTo, percentage);
    }
}