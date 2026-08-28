using UnityEngine;
using UnityEngine.Rendering;

public class PSBurstEmission : PSEmission
{
    [SerializeField] private int count = 10;

    private bool hasStarted = false;

    private void Start()
    {
        Burst();
        hasStarted = true;
    }

    private void OnEnable()
    {
        if (!hasStarted) return;

        Burst();
    }


    public void Burst()
    {
        for (int i = 0; i < count; i++)
        {
            EmitParticle();
        }
    }
}
