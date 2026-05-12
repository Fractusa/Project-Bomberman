using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public GameObject bombPrefab;
    public int explosionRange = 2;
    public int maxBombs = 1;
    public int activeBombs = 0;

    public float moveSpeed = 5f;
    public void AddPowerup(PowerupEffect effect)
    {
        explosionRange += effect.extraRange;
        maxBombs += effect.maxBombs;
        moveSpeed += effect.moveSpeed;

        Debug.Log($"Powerup picked up! stats: Range: {explosionRange}, Max bombs: {maxBombs}, Movement speed: {moveSpeed}");
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryPlaceBomb();
        }
    }


    void TryPlaceBomb()
    {
        if(activeBombs < maxBombs)
        {
            Vector3 spawnPos = new Vector3(
                Mathf.Round(transform.position.x),
                -0.5f,
                Mathf.Round(transform.position.z)
            );
        

            GameObject newBomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

            activeBombs++;
            newBomb.GetComponent<Bomb>().Setup(explosionRange, this);
        }
    }
}
