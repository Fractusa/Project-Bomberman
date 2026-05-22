using Mirror;
using UnityEngine;

public class NewMonoBehaviourScript : NetworkBehaviour
{
    public Powerup effectData;

    private void OnTriggerEnter(Collider other)
    {
        // Check if it is a player who collided with the prefab
        if (other.CompareTag("Player"))
        {
            //Get player stats from the player
            PlayerStats stats = other.GetComponent<PlayerStats>();

            if (stats != null)
            {
                //Add stats to the player
                stats.AddPowerup(effectData);

                //Delete powerup prefab
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
