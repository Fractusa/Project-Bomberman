using Mirror;
using UnityEngine;


public enum PlayerTeam { Red, Green, Blue, Yellow, Purple, Orange}

public class PlayerStats : NetworkBehaviour
{
    public PlayerTeam myTeam;
    public PlayerColorChoice colorChoice = PlayerColorChoice.Red;


    public GameObject bombPrefab;
    public int explosionRange = 2;
    public int maxBombs = 1;

    [SyncVar]public int activeBombs = 0;

    public float moveSpeed = 5f;

    void Start()
    {
        ApplyTeamColor();
    }

    public void AddPowerup(PowerupEffect effect)
    {
        explosionRange += effect.extraRange;
        maxBombs += effect.maxBombs;
        moveSpeed += effect.moveSpeed;

        //Update the player movement speed
        if(GetComponent<Player>()) GetComponent<Player>().moveSpeed = moveSpeed;

        Debug.Log($"Powerup picked up! stats: Range: {explosionRange}, Max bombs: {maxBombs}, Movement speed: {moveSpeed}");
    }


    void Update()
    {
        if (isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryPlaceBomb();
        }
    }


    public void TryPlaceBomb()
    {
        if (isLocalPlayer)
        {
            CmdTryPlaceBomb();
        }
    }

    [Command]
    void CmdTryPlaceBomb()
    {
        if(activeBombs < maxBombs)
        {
            Vector3 spawnPos = new Vector3(transform.position.x, -0.5f, transform.position.z);

           
                GameObject newBomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

                activeBombs++;
                newBomb.GetComponent<Bomb>().Setup(explosionRange, this);

            NetworkServer.Spawn(newBomb);

        }
    }

    void ApplyTeamColor()
    {
        colorChoice = GetComponent<PlayerColor>().colorChoice;

        //Change color on the player models material based on their team color
        switch (colorChoice)
        {
            case PlayerColorChoice.Red: myTeam = PlayerTeam.Red; break;
            case PlayerColorChoice.Green: myTeam = PlayerTeam.Green; break;
            case PlayerColorChoice.Blue: myTeam = PlayerTeam.Blue; break;
            case PlayerColorChoice.Yellow: myTeam = PlayerTeam.Yellow; break;
            case PlayerColorChoice.Purple: myTeam = PlayerTeam.Purple; break;
            case PlayerColorChoice.Orange: myTeam = PlayerTeam.Orange; break;
        }
    }
}
