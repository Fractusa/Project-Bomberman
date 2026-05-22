using Mirror;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public float roundEndDelay = 2.0f;
    public float restartSceneDelay = 5.0f;

    public int playersLeftToWin = 1;
    private bool isRoundEnding = false;

    [SyncVar] public int scoreRed = 0;
    [SyncVar] public int scoreBlue = 0;
    [SyncVar] public int scoreGreen = 0;
    [SyncVar] public int scoreYellow = 0;
    [SyncVar] public int scoreOrange = 0;
    [SyncVar] public int scorePurple = 0;


    [Server]
    public void CheckRemainingPlayers()
    {
        if (isRoundEnding) return;
        StartCoroutine(DelayedCheckRoutine());
    }

    private IEnumerator DelayedCheckRoutine()
    {
        isRoundEnding = true;
        yield return new WaitForSeconds(roundEndDelay);

        //Find all alive players based on them still having lives left.
        List<PlayerHealth> alivePlayers = new List<PlayerHealth>();
        foreach (PlayerHealth p in FindObjectsByType<PlayerHealth>())
        {
            if (p.lives > 0) alivePlayers.Add(p);
        }

        if(alivePlayers.Count == playersLeftToWin)
        {
            //Find the team of the last player alive.
            PlayerStats winnerStats = alivePlayers[0].GetComponent<PlayerStats>();
            if(winnerStats != null)
            {
                EndRound(winnerStats.myTeam);
            }
        }
        else if(alivePlayers.Count == 0)
        {
            EndRoundDraw();
        }
        else
        {
            isRoundEnding = false;
        }
    }

    void EndRoundDraw()
    {
        Debug.Log($"Round has ended as a draw! Everyone died!");
        StartCoroutine(RestartSceneRoutine());
    }




    void EndRound(PlayerTeam winningTeam)
    {
        int currentWinnerSCore = 0;

        switch (winningTeam)
        {
            case PlayerTeam.Red: scoreRed++; currentWinnerSCore = scoreRed; break;
            case PlayerTeam.Blue: scoreBlue++; currentWinnerSCore = scoreBlue; break;
            case PlayerTeam.Green: scoreGreen++; currentWinnerSCore = scoreGreen; break;
            case PlayerTeam.Yellow: scoreYellow++; currentWinnerSCore = scoreYellow; break;
            case PlayerTeam.Orange: scoreOrange++; currentWinnerSCore = scoreOrange; break;
            case PlayerTeam.Purple: scorePurple++; currentWinnerSCore = scorePurple; break;

        }
        Debug.Log($"Round has ended! {winningTeam} won! Their score is {currentWinnerSCore}");


        if(currentWinnerSCore >= 3)
        {
            EndMatch(winningTeam);
        }
        else
        {
            StartCoroutine(RestartSceneRoutine());
        }

    }

    IEnumerator RestartSceneRoutine()
    {
        Debug.Log($"Restarting the round in {restartSceneDelay}");
        yield return new WaitForSeconds(restartSceneDelay);


        //Once round ends remove all bombs, explosions and powerups
        foreach (GameObject bomb in GameObject.FindGameObjectsWithTag("Bomb")) Destroy(bomb);
        foreach (GameObject explosion in GameObject.FindGameObjectsWithTag("Explosion")) Destroy(explosion);
        foreach (GameObject powerup in GameObject.FindGameObjectsWithTag("Powerup")) Destroy(powerup);

        //Find all boxes and set them active
        DestroyableBox[] allboxes = FindObjectsByType<DestroyableBox>(FindObjectsInactive.Include);
        foreach(DestroyableBox box in allboxes)
        {
            box.ResetBox();
        }

        //Find all players and reset them and place them in the spawn locations
        NetworkStartPosition[] spawns = FindObjectsByType<NetworkStartPosition>();
        PlayerHealth[] allPlayers = FindObjectsByType<PlayerHealth>();

        for (int i = 0; i < allPlayers.Length; i++)
        {
            Vector3 spawnPos = (i < spawns.Length) ? spawns[i].transform.position : Vector3.zero;
            allPlayers[i].ResetPlayer(spawnPos);
        }

        isRoundEnding = false;
    }

    void EndMatch(PlayerTeam matchWinner)
    {
        Debug.Log($"Match has ended! {matchWinner} won 3 rounds");

        ResetAllScores();

        //Load lobby scene
        if(NetworkManager.singleton is NetworkRoomManager roomManager)
        {
            roomManager.ServerChangeScene(roomManager.RoomScene);
        }
    }


    private void ResetAllScores()
    {
        scoreRed = 0;
        scoreBlue = 0;
        scoreGreen = 0;
        scoreYellow = 0;
        scoreOrange = 0;
        scorePurple = 0;
    }


}
