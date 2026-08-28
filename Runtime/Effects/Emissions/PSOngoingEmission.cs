using UnityEngine;

public class PSOngoingEmission : PSEmission
{
    [SerializeField] private float spawnRate;
    private float spawnTime;
    private float spawnTimer;

    private void OnEnable()
    {
        spawnTime = 1.0f / spawnRate;
        spawnTimer = spawnTime;
    }

    private void Update()
    {
        if (spawnRate <= 0) return;

        if (spawnTimer > 0)
            spawnTimer -= Time.deltaTime;
        else
        {
            spawnTimer = spawnTime;
            EmitParticle();
        }
    }
}
