using UnityEngine;

public class PSBoxShape : PSShape
{
    [SerializeField] private Vector2 boxSize;

    public override void GetSpawnData(out Vector2 position, out Vector2 direction)
    {
        int i = Random.Range(0, 4);
        float randomPos;

        switch (i)
        {
            case 0:
                randomPos = Random.Range(-(boxSize.x / 2), (boxSize.x / 2));
                position = new Vector2(randomPos, boxSize.y/2);
                direction = transform.up;
                break;
            case 1:
                randomPos = Random.Range(-(boxSize.x / 2), (boxSize.x / 2));
                position = new Vector2(randomPos, -(boxSize.y / 2));
                direction = -transform.up;
                break;
            case 2:
                randomPos = Random.Range(-(boxSize.y / 2), (boxSize.y / 2));
                position = new Vector2(boxSize.x / 2, randomPos);
                direction = transform.right;
                break;
            case 3:
                randomPos = Random.Range(-(boxSize.y / 2), (boxSize.y / 2));
                position = new Vector2(-(boxSize.x / 2), randomPos);
                direction = -transform.right;
                break;
            default:
                position = Vector2.zero;
                direction = transform.up;
                break;
        }
    }
}