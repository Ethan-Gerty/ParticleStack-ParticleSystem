using UnityEngine;

public class PSCircleShape : PSShape
{
    [SerializeField] private Vector2 radiusRange;

    public override void GetSpawnData(out Vector2 position, out Vector2 direction)
    {
        float radius = Random.Range(radiusRange.x, radiusRange.y);

        Vector2 offset = Random.insideUnitCircle.normalized * radius;

        position = (Vector2)transform.position + offset;

        direction = offset.normalized;

        if (direction == Vector2.zero)
        {
            direction = Random.insideUnitCircle.normalized;
        }
    }
}
