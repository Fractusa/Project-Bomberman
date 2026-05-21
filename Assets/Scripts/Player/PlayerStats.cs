using UnityEngine;


public enum PlayerTeam { Red, Green, Blue, Yellow}

public class PlayerStats : MonoBehaviour
{
    public PlayerTeam myTeam;
    public MeshRenderer playerRenderer; //put the players 3d model here, to allow it to be rendered in it's teams color


    void Start()
    {
        ApplyTeamColor();
    }


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
            Vector3 spawnPos = new Vector3(transform.position.x, -0.5f, transform.position.z);

           
                GameObject newBomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

                activeBombs++;
                newBomb.GetComponent<Bomb>().Setup(explosionRange, this);

        }
    }

    void ApplyTeamColor()
    {
        if (playerRenderer == null) return;

        //Change color on the player models material based on their team color
        switch (myTeam)
        {
            case PlayerTeam.Red:
                playerRenderer.material.color = Color.red; break;
            case PlayerTeam.Green:
                playerRenderer.material.color = Color.green; break;
            case PlayerTeam.Blue:
                playerRenderer.material.color = Color.blue; break;
            case PlayerTeam.Yellow:
                playerRenderer.material.color = Color.yellow; break;
        }
    }
}
