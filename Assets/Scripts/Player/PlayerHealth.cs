using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int lives = 3;
    public float invulnerabilityDuration = 1f;
    private bool isInvulnerable;

    private PlayerStats playerStats;


    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }
    public void TakeDamage()
    {

        if (isInvulnerable) return;


        lives--;
        Debug.Log($"Player lost a life! Lives left: {lives}");

        if( lives <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(invulnerabilityDuration);

        isInvulnerable = false;
    }

    public void Die()
    {
        Debug.Log("Player died");

        if(playerStats != null)
        {
            playerStats.enabled = false;
        }
        var movement = GetComponent<PlayerMovement>

        FindAnyObjectByType<GameManager>()?.CheckRemainingPlayers();
    }
}
