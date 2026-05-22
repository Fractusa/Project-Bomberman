using UnityEngine;
using Mirror;
public class Bomb : NetworkBehaviour
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


    [Server]
    void Explode()
    {
        RpcSpawnVisualExplosions();
        

        CheckExplosionDirection(Vector3.forward);
        CheckExplosionDirection(Vector3.back);
        CheckExplosionDirection(Vector3.left);
        CheckExplosionDirection(Vector3.right);


        if (owner != null) owner.activeBombs--;

        Destroy(gameObject);
    }

    [ClientRpc]
    void RpcSpawnVisualExplosions()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.Euler(90, 0, 0));


        SpawnVisualDirection(Vector3.forward);
        SpawnVisualDirection(Vector3.back);
        SpawnVisualDirection(Vector3.left);
        SpawnVisualDirection(Vector3.right);
    }

    void SpawnVisualDirection(Vector3 direction)
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
                break;
            }
        }
    }

    void CheckExplosionDirection(Vector3 direction)
    {
        for (int i = 1; i <= bombRange; i++)
        {
                RaycastHit hit;

                //Makes the SphereCast radius 0.3f, to easier hit targets. 
                if (Physics.SphereCast(transform.position, 0.3f, direction, out hit, i, levelLayerMask))
                {
                    //If a box is his explode the box
                    if (hit.collider.CompareTag("Box"))
                    {
                        if (NetworkServer.active)
                        {
                            hit.collider.GetComponent<DestroyableBox>()?.Explode();
                        }
                    }

                    break;
                }
            
        }
    }
}
