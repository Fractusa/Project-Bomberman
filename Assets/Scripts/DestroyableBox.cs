using UnityEngine;

public class DestroyableBox : MonoBehaviour
{
    public GameObject[] powerupPrefabs; //List of possible powerups to drop
    [Range(0, 100)] public float spawnChance = 20f;
    public void Explode()//Called when the box is hit by an explosion
    {
        float roll = Random.Range(0, 100f);//Roll a number between 0 and 100

        if(roll <= spawnChance)//If the rolled number is 0 to 20 drop a powerup
        {
            //Randomly decide which powerup to drop
            int randomIndex = Random.Range(0, powerupPrefabs.Length);
            Instantiate(powerupPrefabs[randomIndex], transform.position, Quaternion.identity);
        }

        Destroy(gameObject);//Destroy box
    }
}
