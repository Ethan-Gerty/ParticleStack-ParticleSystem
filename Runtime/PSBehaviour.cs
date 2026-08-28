using UnityEngine;

[RequireComponent(typeof(PSEmitter))]
public abstract class PSBehaviour : MonoBehaviour
{
    protected PSEmitter emitter;
    protected PSShape shape;

    private void OnEnable()
    {
        emitter = gameObject.GetComponent<PSEmitter>();
        shape = gameObject.GetComponent<PSShape>();
    }

    public virtual void OnParticleSpawn(ref PSParticle particle) { }

    public virtual void UpdateParticle(ref PSParticle particle, float deltaTime) { }
}