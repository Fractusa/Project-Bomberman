using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float roundEndDelay = 2.0f;
    public float restartSceneDelay = 5.0f;

    public int playersLeftToWin = 1;
    private bool isRoundEnding = false;

    public static int scoreRed = 0;
    public static int scoreBlue = 0;
    public static int scoreGreen = 0;
    public static int scoreYellow = 0;
    public static int scoreOrange = 0;
    public static int scorePurple = 0;


    public void CheckRemainingPlayers()
    {
        if (isRoundEnding) return;

        StartCoroutine(DelayedCheckRoutine());
    }

    private IEnumerator DelayedCheckRoutine()
    {
        isRoundEnding = true;

        yield return new WaitForSeconds(roundEndDelay);

        PlayerHealth[] activePlayers = FindObjectsByType<PlayerHealth>();

        if (activePlayers.Length == playersLeftToWin)
        {
            PlayerStats winnerStats = activePlayers[0].GetComponent<PlayerStats>();

            if (winnerStats != null)
            {
                EndRound(winnerStats.myTeam);
            }
        }
        else if (activePlayers.Length == 0)
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
            case PlayerTeam.Red:
                scoreRed++;
                currentWinnerSCore = scoreRed;
                break;
            case PlayerTeam.Blue:
                scoreBlue++;
                currentWinnerSCore = scoreBlue;
                break;
            case PlayerTeam.Green:
                scoreGreen++;
                currentWinnerSCore = scoreGreen;
                break;
            case PlayerTeam.Yellow:
                scoreYellow++;
                currentWinnerSCore = scoreYellow;
                break;
            case PlayerTeam.Orange:
                scoreOrange++;
                currentWinnerSCore = scoreOrange;
                break;
            case PlayerTeam.Purple:
                scorePurple++;
                currentWinnerSCore = scorePurple;
                break;

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


        isRoundEnding = false;

        if(NetworkManager.singleton is NetworkManager roomManager)
        {
            Debug.Log($"Mirror reload");

            string currentSceneName = SceneManager.GetActiveScene().name;

            roomManager.ServerChangeScene(currentSceneName);
        }
        else
        {
            Debug.Log($"Non-Mirror reload");
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

    }

    void EndMatch(PlayerTeam matchWinner)
    {
        Debug.Log($"Match has ended! {matchWinner} won 3 rounds");

        ResetAllScores();

        //Load lobby scene
        SceneManager.LoadScene("LobbyScene");
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
