using System.Collections.Generic;
using UnityEngine;

public class PSRandomColourBeh : PSBehaviour
{
    [SerializeField] private List<Color> colourList;

    public override void OnParticleSpawn(ref PSParticle particle)
    {
        int i = Random.Range(0, colourList.Count);

        particle.colour = colourList[i];
    }
}