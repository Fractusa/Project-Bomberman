using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void CheckRemainingPlayers()
    {
        PlayerHealth[] players = FindObjectsByType<PlayerHealth>();

        int aliveCount = 0;
        foreach (var player in players)
        {
            if (player.lives > 0)
            {
                aliveCount++;
            }
        }

        if(aliveCount <= 1)
        {
            EndRound();
        }
    }

    void EndRound()
    {
        Debug.Log("Round has ended");
    }
}
