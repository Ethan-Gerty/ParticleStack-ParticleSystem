using UnityEngine;

public class PSGravityBeh : PSBehaviour
{
    [SerializeField] private float gravityForce;

    public override void UpdateParticle(ref PSParticle particle, float deltaTime)
    {
        particle.velocity = new Vector2(particle.velocity.x, particle.velocity.y - gravityForce * deltaTime);
    }
}
