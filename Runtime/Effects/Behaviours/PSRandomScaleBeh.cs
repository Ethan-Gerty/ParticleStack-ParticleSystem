using UnityEngine;

public class PSRandomScaleBeh : PSBehaviour
{
    [SerializeField] private Vector2 xRange;
    [SerializeField] private Vector2 yRange;

    public override void OnParticleSpawn(ref PSParticle particle)
    {
        float xScale = Random.Range(xRange.x, xRange.y);
        float yScale = Random.Range(yRange.x, yRange.y);

        particle.scale = new Vector2(xScale, yScale);
    }
}