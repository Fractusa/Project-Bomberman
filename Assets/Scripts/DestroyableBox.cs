using UnityEngine;
using Mirror;

public class DestroyableBox : NetworkBehaviour
{
    public GameObject[] powerupPrefabs; //List of possible powerups to drop
    [Range(0, 100)] public float spawnChance = 20f;

    [Server]
    public void Explode()//Called when the box is hit by an explosion
    {
        float roll = Random.Range(0, 100f);//Roll a number between 0 and 100

        if(roll <= spawnChance)//If the rolled number is 0 to 20 drop a powerup
        {
            //Randomly decide which powerup to drop
            int randomIndex = Random.Range(0, powerupPrefabs.Length);
            GameObject powerupObject = Instantiate(powerupPrefabs[randomIndex], transform.position, Quaternion.identity);

            NetworkServer.Spawn(powerupObject);
        }

        NetworkServer.Destroy(gameObject);//Destroy box
    }
}
