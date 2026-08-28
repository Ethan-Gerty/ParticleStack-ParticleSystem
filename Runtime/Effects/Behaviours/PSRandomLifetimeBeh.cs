using UnityEngine;

public class PSRandomLifetimeBeh : PSBehaviour
{
    [SerializeField] private Vector2 range;

    public override void OnParticleSpawn(ref PSParticle particle)
    {
        float lifetime = Random.Range(range.x, range.y);

        particle.lifeTime = lifetime;
    }
}