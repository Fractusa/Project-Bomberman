using Mirror;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public float roundEndDelay = 2.0f;
    public float restartSceneDelay = 5.0f;
    public float roundDuration = 120f;

    public int playersLeftToWin = 1;
    private bool isRoundEnding = false;



    public TextMeshProUGUI roundTimer;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI txtRedScore;
    public TextMeshProUGUI txtBlueScore;
    public TextMeshProUGUI txtGreenScore;
    public TextMeshProUGUI txtYellowScore;
    public TextMeshProUGUI txtOrangeScore;
    public TextMeshProUGUI txtPurpleScore;

    //Adding hooks that automatically calls the method on the client once the value is changed on the server, updating the clients score UI to match the server
    [SyncVar(hook = nameof(OnRedScoreChanged))] public int scoreRed = 0;
    [SyncVar(hook = nameof(OnBlueScoreChanged))] public int scoreBlue = 0;
    [SyncVar(hook = nameof(OnGreenScoreChanged))] public int scoreGreen = 0;
    [SyncVar(hook = nameof(OnYellowScoreChanged))] public int scoreYellow = 0;
    [SyncVar(hook = nameof(OnOrangeScoreChanged))] public int scoreOrange = 0;
    [SyncVar(hook = nameof(OnPurpleScoreChanged))] public int scorePurple = 0;

    [SyncVar(hook = nameof(OnTimerChanged))] private int currentRoundTime = 0;

    private Coroutine roundTimerCoroutine;

    void OnRedScoreChanged(int oldScore, int newScore) { if (txtRedScore != null) txtRedScore.text = $"Red Team: {newScore}"; }
    void OnBlueScoreChanged(int oldScore, int newScore) { if (txtBlueScore != null) txtBlueScore.text = $"Blue Team: {newScore}"; }
    void OnGreenScoreChanged(int oldScore, int newScore) { if (txtGreenScore != null) txtGreenScore.text = $"Green Team: {newScore}"; }
    void OnYellowScoreChanged(int oldScore, int newScore) { if (txtYellowScore != null) txtYellowScore.text = $"Yellow Team: {newScore}"; }
    void OnOrangeScoreChanged(int oldScore, int newScore) { if (txtOrangeScore != null) txtOrangeScore.text = $"Orange Team: {newScore}"; }
    void OnPurpleScoreChanged(int oldScore, int newScore) { if (txtPurpleScore != null) txtPurpleScore.text = $"Purple Team: {newScore}"; }

    void OnTimerChanged(int oldTime, int newTime)
    {
        if(roundTimer != null)
        {
            int minutes = newTime / 60;
            int seconds = newTime % 60;
            roundTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    [Server]
    public void CheckRemainingPlayers()
    {
        if (isRoundEnding) return;
        StartCoroutine(DelayedCheckRoutine());
    }

    [Server]
    private void StartRoundTimer()
    {
        if (roundTimerCoroutine != null) StopCoroutine(roundTimerCoroutine);

        currentRoundTime = Mathf.CeilToInt(roundDuration);
        roundTimerCoroutine = StartCoroutine(RoundTimerCoroutine());
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartRoundTimer();
    }

    [Server]
    private IEnumerator RoundTimerCoroutine()
    {
        while (currentRoundTime > 0 && !isRoundEnding)
        {
            yield return new WaitForSeconds(1f);
            
            if (isRoundEnding) yield break;

            currentRoundTime--;
        }

        if(currentRoundTime <= 0 && !isRoundEnding)
        {
            EndRoundDraw();
        }
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
        isRoundEnding = true;
        Debug.Log($"Round has ended as a draw! Everyone died!");

        RpcShowWinnerText("Round Draw", Color.white);
        StartCoroutine(RestartSceneRoutine());

    }




    void EndRound(PlayerTeam winningTeam)
    {
        int currentWinnerScore = 0;
        Color teamColor = Color.white;
        string teamName = "";

        switch (winningTeam)
        {
            case PlayerTeam.Red: scoreRed++; currentWinnerScore = scoreRed; teamColor = Color.red; teamName = "Red"; break;
            case PlayerTeam.Blue: scoreBlue++; currentWinnerScore = scoreBlue; teamColor = Color.blue; teamName = "Blue"; break;
            case PlayerTeam.Green: scoreGreen++; currentWinnerScore = scoreGreen; teamColor = Color.green; teamName = "Green"; break;
            case PlayerTeam.Yellow: scoreYellow++; currentWinnerScore = scoreYellow; teamColor = Color.yellow; teamName = "Yellow"; break;
            case PlayerTeam.Orange: scoreOrange++; currentWinnerScore = scoreOrange; teamColor = Color.orange; teamName = "Orange"; break;
            case PlayerTeam.Purple: scorePurple++; currentWinnerScore = scorePurple; teamColor = Color.purple; teamName = "Purple"; break;

        }

        RpcShowWinnerText($"{teamName} Team Won the round", teamColor);
        Debug.Log($"Round has ended! {winningTeam} won! Their score is {currentWinnerScore}");


        if(currentWinnerScore >= 3)
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

        RpcHideWinnerText();

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
        StartRoundTimer();
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

    [ClientRpc]
    void RpcShowWinnerText(string textToDisplay, Color colorToUse)
    {
        winnerText.text = textToDisplay;
        winnerText.color = colorToUse;
        winnerText.gameObject.SetActive(true);
    }

    [ClientRpc]
    void RpcHideWinnerText()
    {
        winnerText.gameObject.SetActive(false);
    }

    public void ResetAllScores()
    {
        scoreRed = 0;
        scoreBlue = 0;
        scoreGreen = 0;
        scoreYellow = 0;
        scoreOrange = 0;
        scorePurple = 0;
    }


}
