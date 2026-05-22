using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class PlayerBombPlacer : NetworkBehaviour
{
    [Header("Bomb settings")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private KeyCode placeBombKey = KeyCode.Space;

    private PlayerStats stats;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if(!isLocalPlayer)
            return;

        if(Input.GetKeyDown(placeBombKey))
        {
            CmdPlaceBomb();
        }
    }
    
    //Method for clients to call to attempt to place a bomb in game
    [Command]
    private void CmdPlaceBomb()
    {   
        if(stats.activeBombs < stats.maxBombs)
        {
            Vector3 spawnPos = GetBombSpawnPosition();

            if(!CanPlaceBombAt(spawnPos))
                return;

            GameObject newBomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

            Bomb bomb = newBomb.GetComponent<Bomb>();

            if(bomb != null)
            {
                bomb.Setup(stats.bombRange, this);
            }

            stats.RegisterBombPlaced();

            NetworkServer.Spawn(newBomb);
        }
    }

    //Server method for getting bomb spawn position
    [Server]
    private Vector3 GetBombSpawnPosition()
    {
        Vector3 spawnPos = new Vector3(
                transform.position.x,
                -0.5f,
                transform.position.z
        );

        return spawnPos;
    }

    //Server method for checking if bomb can be placed
    [Server]
    private bool CanPlaceBombAt(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.3f);

        //Checks if a bomb is already placed around the bomb
        foreach(Collider collider in colliders)
        {
            if(collider.GetComponent<Bomb>() != null)
                return false;
        }

        return true;
    }

    [Server]
    public void OnBombExploded()
    {
        stats.RegisterBombRemoved();
    }
}
