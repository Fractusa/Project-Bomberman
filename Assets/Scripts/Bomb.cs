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

        SpawnExplosionDirection(Vector3.forward);
        SpawnExplosionDirection(Vector3.back);
        SpawnExplosionDirection(Vector3.left);
        SpawnExplosionDirection(Vector3.right);


        if (owner != null) owner.activeBombs--;

        Destroy(gameObject);
    }


    void SpawnExplosionDirection(Vector3 direction)
    {
        for (int i = 1; i <= bombRange; i++)
        {
            Vector3 spawnPos = transform.position + direction * i;
            RaycastHit hit;

            //Check if a wall or box is hit before creating the explosion
            Quaternion rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);

            //Makes the SphereCast radius 0.3f, to easier hit targets. 
            if (!Physics.SphereCast(transform.position, 0.3f, direction, out hit, i, levelLayerMask))
            {
                Instantiate(explosionPrefab, spawnPos, rotation);
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
