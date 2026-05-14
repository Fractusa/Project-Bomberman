using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float roundEndDelay = 2.0f;
    public int playersLeftToWin = 1;

    private bool isRoundEnding = false;

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
    void EndRound(PlayerTeam winningTeam)
    {
        Debug.Log($"Round has ended! {winningTeam} won!");
    }
    void EndRoundDraw()
    {
        Debug.Log($"Round has ended as a draw! Everyone died!");
    }
}
