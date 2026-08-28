using UnityEngine;
using UnityEngine.U2D;

public class PSEmitter : MonoBehaviour
{
    // Emission Variables
    [field: Header("Emitter Settings")]
    [field: SerializeField] public int maxParticles { get; private set; }

    [Header("Particle Settings")]
    [SerializeField] public float lifetime;
    [SerializeField] public float particleSpeed;
    [SerializeField] public Vector2 particleScale;
    [SerializeField] public Color particleColour;

    [SerializeField] public float startRotation = 0f;
    [SerializeField] public float startAngularVelocity = 0f;

    public PSParticle[] particles { get; private set; }
    private PSBehaviour[] particleBehaviours;

    public int activeParticleCount { get; private set; }

    // Rendering Variables
    [Header("Rendering Settings")]
    [SerializeField] public Sprite sprite;
    [SerializeField] public Material material;

    [SerializeField] private string sortingLayerName = "Default";
    [SerializeField] private int orderInLayer = 0;

    private SpriteRenderer[] renderers;
    private int previousActiveCount;

    private Transform rendererParent;




    private void Awake()
    {
        particles = new PSParticle[maxParticles];

        CreateRendererPool();
        RenderParticles();
        GetBehaviours();
    }

    private void Update()
    {
        UpdateParticles(Time.deltaTime);
    }

    private void LateUpdate()
    {
        RenderParticles();
    }




    private void GetBehaviours()
    {
        particleBehaviours = GetComponents<PSBehaviour>();
    }

    public void Emit(PSParticle newParticle)
    {
        if (activeParticleCount >= maxParticles)
            return;

        ref PSParticle particle = ref particles[activeParticleCount];

        particle = newParticle;

        for (int i = 0; i < particleBehaviours.Length; i++)
        {
            if (particleBehaviours[i] == null)
                continue;

            particleBehaviours[i].OnParticleSpawn(ref particle);
        }

        activeParticleCount++;
    }

    private void UpdateParticles(float deltaTime)
    {
        int i = 0;

        while (i < activeParticleCount)
        {
            ref PSParticle particle = ref particles[i];

            particle.age += deltaTime;

            for (int b = 0; b < particleBehaviours.Length; b++)
            {
                if (particleBehaviours[b] == null)
                    continue;

                particleBehaviours[b].UpdateParticle(
                    ref particle,
                    deltaTime
                );
            }

            if (!particle.isAlive)
            {
                RemoveParticle(i);
                continue;
            }

            particle.position += particle.velocity * deltaTime;
            particle.zRotation += particle.angularVelocity * deltaTime;

            i++;
        }
    }

    private void RemoveParticle(int index)
    {
        int lastParticle = activeParticleCount - 1;

        particles[index] = particles[lastParticle];

        activeParticleCount--;
    }




    private void CreateRendererPool()
    {
        renderers = new SpriteRenderer[maxParticles];

        GameObject parentObject = new GameObject("Particle Renderers");
        rendererParent = parentObject.transform;
        rendererParent.SetParent(transform, false);

        for (int i = 0; i < renderers.Length; i++)
        {
            GameObject particleObject = new GameObject($"Particle {i}");
            particleObject.transform.SetParent(rendererParent, false);

            SpriteRenderer spriteRenderer =
                particleObject.AddComponent<SpriteRenderer>();

            spriteRenderer.sprite = sprite;

            if (material != null)
            {
                spriteRenderer.sharedMaterial = material;
            }

            spriteRenderer.sortingLayerName = sortingLayerName;
            spriteRenderer.sortingOrder = orderInLayer;

            spriteRenderer.enabled = false;

            renderers[i] = spriteRenderer;
        }
    }


    private void RenderParticles()
    {
        int activeCount = activeParticleCount;

        for (int i = 0; i < activeCount; i++)
        {
            ref PSParticle particle = ref particles[i];

            SpriteRenderer spriteRenderer = renderers[i];
            Transform particleTransform = spriteRenderer.transform;

            if (!spriteRenderer.enabled)
            {
                spriteRenderer.enabled = true;
            }

            particleTransform.position = new Vector3(
                particle.position.x,
                particle.position.y,
                transform.position.z
            );

            particleTransform.rotation =
                Quaternion.Euler(0f, 0f, particle.zRotation);

            particleTransform.localScale = new Vector3(
                particle.scale.x,
                particle.scale.y,
                1f
            );

            spriteRenderer.color = particle.colour;
        }

        for (int i = activeCount; i < previousActiveCount; i++)
        {
            renderers[i].enabled = false;
        }

        previousActiveCount = activeCount;
    }
}