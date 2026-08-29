using UnityEngine;
using UnityEngine.Rendering;

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

    private const int MAX_INSTANCES_PER_BATCH = 1023;

    private Mesh particleMesh;
    private Material runtimeMaterial;
    private MaterialPropertyBlock propertyBlock;

    private readonly Matrix4x4[] instanceMatrices = new Matrix4x4[MAX_INSTANCES_PER_BATCH];

    private readonly Vector4[] instanceColours = new Vector4[MAX_INSTANCES_PER_BATCH];




    private void Awake()
    {
        particles = new PSParticle[maxParticles];

        GetBehaviours();

        CreateParticleMesh();

        propertyBlock = new MaterialPropertyBlock();

        runtimeMaterial = new Material(material);
        runtimeMaterial.enableInstancing = true;
        runtimeMaterial.mainTexture = sprite.texture;
    }

    private void Update()
    {
        UpdateParticles(Time.deltaTime);
    }

    private void LateUpdate()
    {
        RenderParticles();
    }

    private void OnDestroy()
    {
        if (particleMesh != null)
            Destroy(particleMesh);

        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);
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




    private void CreateParticleMesh()
    {
        particleMesh = new Mesh();
        particleMesh.name = "ParticleStack Particle Mesh";

        Vector2[] spriteVertices = sprite.vertices;
        Vector3[] vertices = new Vector3[spriteVertices.Length];

        for (int i = 0; i < spriteVertices.Length; i++)
        {
            vertices[i] = spriteVertices[i];
        }

        ushort[] spriteTriangles = sprite.triangles;
        int[] triangles = new int[spriteTriangles.Length];

        for (int i = 0; i < spriteTriangles.Length; i++)
        {
            triangles[i] = spriteTriangles[i];
        }

        particleMesh.vertices = vertices;
        particleMesh.uv = sprite.uv;
        particleMesh.triangles = triangles;

        particleMesh.RecalculateBounds();
    }

    private void RenderParticles()
    {
        int particleIndex = 0;

        while (particleIndex < activeParticleCount)
        {
            int batchCount = Mathf.Min(
                MAX_INSTANCES_PER_BATCH,
                activeParticleCount - particleIndex
            );

            for (int i = 0; i < batchCount; i++)
            {
                ref PSParticle particle =
                    ref particles[particleIndex + i];

                Vector3 position = new Vector3(
                    particle.position.x,
                    particle.position.y,
                    transform.position.z
                );

                Quaternion rotation =
                    Quaternion.Euler(
                        0f,
                        0f,
                        particle.zRotation
                    );

                Vector3 scale = new Vector3(
                    particle.scale.x,
                    particle.scale.y,
                    1f
                );

                instanceMatrices[i] = Matrix4x4.TRS(
                    position,
                    rotation,
                    scale
                );

                instanceColours[i] = particle.colour;
            }

            propertyBlock.Clear();

            propertyBlock.SetVectorArray(
                "_PSColour",
                instanceColours
            );

            Graphics.DrawMeshInstanced(
                particleMesh,
                0,
                runtimeMaterial,
                instanceMatrices,
                batchCount,
                propertyBlock,
                ShadowCastingMode.Off,
                false,
                gameObject.layer
            );

            particleIndex += batchCount;
        }
    }
}