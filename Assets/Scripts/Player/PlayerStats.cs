using Mirror;
using UnityEngine;

public enum PlayerTeam { Red, Green, Blue, Yellow}

public class PlayerStats : NetworkBehaviour
{
    public PlayerTeam myTeam;
    public MeshRenderer playerRenderer; //put the players 3d model here, to allow it to be rendered in it's teams color

    void Start()
    {
        ApplyTeamColor();
    }

    [SyncVar] public int bombRange = 2;
    [SyncVar] public int maxBombs = 1;
    [SyncVar] public int activeBombs = 0;
    [SyncVar] public float moveSpeed = 5f;
    [SyncVar] public int playerLives = 3;

    [Server]
    public void AddPowerup(Powerup effect)
    {
        bombRange += effect.extraRange;
        maxBombs += effect.maxBombs;
        moveSpeed += effect.moveSpeed;

        Debug.Log($"Powerup picked up! stats: Range: {bombRange}, Max bombs: {maxBombs}, Movement speed: {moveSpeed}");
    }
    
    [Server]
    public void RegisterBombPlaced()
    {
        activeBombs++;
    }

    [Server]
    public void RegisterBombRemoved()
    {
        activeBombs = Mathf.Max(0, activeBombs -1);
    }

    [Server]
    public void ResetRoundStats()
    {
        bombRange = 2;
        maxBombs = 1;
        activeBombs = 0;
        moveSpeed = 5f;
        playerLives = 3;
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
