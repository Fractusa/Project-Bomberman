using UnityEngine;
using Mirror;
using System.Collections;

public class Bomb : NetworkBehaviour
{
    public float fuseTime = 3f;
    public GameObject explosionPrefab;
    public LayerMask levelLayerMask;
    private int bombRange;
    private PlayerBombPlacer creator;

    public void Setup(int range, PlayerBombPlacer bombCreator)
    {
        bombRange = range;
        creator = bombCreator;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        StartCoroutine(FuseRoutine());
    }

    [Server]
    private IEnumerator FuseRoutine()
    {
        yield return new WaitForSeconds(fuseTime);

        Explode();
    }

    [Server]
    void Explode()
    {
        GameObject explosionObject = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(explosionObject);

        SpawnExplosionDirection(Vector3.forward);
        SpawnExplosionDirection(Vector3.back);
        SpawnExplosionDirection(Vector3.left);
        SpawnExplosionDirection(Vector3.right);

        if (creator != null) 
            creator.OnBombExploded();

        NetworkServer.Destroy(gameObject);
    }

    [Server]
    private void SpawnExplosionDirection(Vector3 direction)
    {
        for (int i = 1; i <= bombRange; i++)
        {
            Vector3 spawnPos = transform.position + direction * i;

            //Check if a wall or box is hit before creating the explosion
            Quaternion rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);

            bool hasHit = Physics.SphereCast(transform.position, 0.3f, direction, out RaycastHit hit, i, levelLayerMask);

            if(hasHit)
            {
                if(hit.collider.CompareTag("Box"))
                {
                    DestroyableBox box = hit.collider.GetComponent<DestroyableBox>();
                    box.Explode();
                    
                    SpawnExplosion(spawnPos, rotation);
                    break;
                }

                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {
                    break;
                }
            }
            
            SpawnExplosion(spawnPos, rotation);
        }
    }

    [Server]
    private void SpawnExplosion(Vector3 position, Quaternion rotation)
    {
        GameObject explosionObject = Instantiate(explosionPrefab, position, rotation);
        NetworkServer.Spawn(explosionObject);
    }
}
