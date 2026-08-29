using UnityEngine;

public class PSDragBeh : PSBehaviour
{
    [SerializeField] private float drag = 1.0f;

    public override void UpdateParticle(ref PSParticle particle, float deltaTime)
    {
        particle.velocity *= Mathf.Exp(-drag * deltaTime);
    }
}