using Mirror;
using UnityEngine;

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

            GameObject powerup = Instantiate(powerupPrefabs[randomIndex], transform.position, Quaternion.identity);

            NetworkServer.Spawn(powerup);
        }

        RpcSetBoxActive(false);//disable box 
    }

    //Method to reenable boxes when the round restarts.
    [Server]
    public void ResetBox()
    {
        RpcSetBoxActive(true);
    }

    [ClientRpc]
    void RpcSetBoxActive(bool state)
    {
        //Turn on or off collider and graphic based on input
        if(GetComponent<Collider>()) GetComponent<Collider>().enabled = state;
        if (transform.childCount > 0) transform.GetChild(0).gameObject.SetActive(state);
        else if (GetComponent<Renderer>()) GetComponent<Renderer>().enabled = state;
    }
}
