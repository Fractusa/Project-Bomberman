using Mirror;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;


public enum PlayerTeam { Red, Green, Blue, Yellow, Purple, Orange}

public class PlayerStats : NetworkBehaviour
{
    public PlayerTeam myTeam;
    public PlayerColorChoice colorChoice = PlayerColorChoice.Red;


    public List<GameObject> uiBombs;

    [SyncVar] public int bombRange = 2;
    [SyncVar(hook = nameof(OnBombCountChanged))] 
    public int maxBombs = 1;
    [SyncVar(hook = nameof(OnBombCountChanged))] 
    public int activeBombs = 0;
    [SyncVar] public float moveSpeed = 5f;
    [SyncVar] public int playerLives = 3;

    void Start()
    {
        ApplyTeamColor();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        GameObject container = GameObject.Find("BombContainer");
        if(container != null)
        {
            uiBombs = new List<GameObject>();
            foreach (Transform child in container.transform)
            {
                uiBombs.Add(child.gameObject);
            }
        }
    }





    [Server]
    public void AddPowerup(PowerupEffect effect)
    {
        ApplyPowerupLogic(effect);
    }


    void OnBombCountChanged(int oldVal, int newVal)
    {
        if (!isLocalPlayer) return;

        UpdateBombUI();
    }

    public void ApplyPowerupLogic(PowerupEffect effect)
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



    public void UpdateBombUI()
    {
        if (uiBombs == null || uiBombs.Count == 0) return;


        int availableBombs = maxBombs - activeBombs;

        for (int i = 0; i < uiBombs.Count; i++)
        {
            if(i < maxBombs)
            {
                uiBombs[i].SetActive(i < availableBombs);
            }
            else
            {
                uiBombs[i].SetActive(false);
            }
        }
    }
}
