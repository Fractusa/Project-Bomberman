using UnityEngine;

public class Bomb : MonoBehaviour
{
    private int bombRange;
    private PlayerStats owner;
    public float fuseTime = 3f;
    public GameObject explosionPrefab;
    public LayerMask levelLayerMask;

    public void Setup(int range, PlayerStats creator)
    {
        bombRange = range;
        owner = creator;

        Invoke("Explode", fuseTime);

    }


    void Explode()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        SpawnExplosionDirection(Vector2.up);
        SpawnExplosionDirection(Vector2.down);
        SpawnExplosionDirection(Vector2.left);
        SpawnExplosionDirection(Vector2.right);


        if (owner != null) owner.activeBombs--;

        Destroy(gameObject);
    }


    void SpawnExplosionDirection(Vector2 direction)
    {
        for (int i = 1; i <= bombRange; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + direction * i;

            //Check if a wall or box is hit before creating the explosion
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, i, levelLayerMask);

            if (!hit)
            {
                Instantiate(explosionPrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                //If a box is his explode the box
                if (hit.collider.CompareTag("Box"))
                {
                    hit.collider.GetComponent<DestroyableBox>()?.Explode();
                }
                //Stop the explosion from continuing through the box
                break;
            }
        }  
    }
}
