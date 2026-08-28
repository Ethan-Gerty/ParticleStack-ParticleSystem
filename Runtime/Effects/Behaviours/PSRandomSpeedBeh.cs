using UnityEngine;

public class PSRandomSpeedBeh : PSBehaviour
{
    [SerializeField] private Vector2 range;

    public override void OnParticleSpawn(ref PSParticle particle)
    {
        float speed = Random.Range(range.x, range.y);

        particle.velocity /= emitter.particleSpeed;
        particle.velocity *= speed;
    }
}