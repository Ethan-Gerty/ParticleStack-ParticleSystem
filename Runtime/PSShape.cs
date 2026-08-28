using UnityEngine;

[RequireComponent(typeof(PSEmitter))]
public abstract class PSShape : MonoBehaviour
{
    public abstract void GetSpawnData(
        out Vector2 position,
        out Vector2 direction
    );
}