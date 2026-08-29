using UnityEngine;

public class PSColourOverTimeBeh : PSBehaviour
{
    [SerializeField] private Color colourTo;

    private Color startColour;

    public override void OnParticleSpawn(ref PSParticle particle)
    {
        startColour = particle.colour;
    }

    public override void UpdateParticle(ref PSParticle particle, float deltaTime)
    {
        float percentage = particle.age / particle.lifeTime;

        particle.colour = Color.Lerp(startColour, colourTo, percentage);
    }
}
