using Mirror;
using System.Collections;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
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
        if (!isServer) return;
        if (isInvulnerable || lives <= 0) return;


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

    [Server]
    public void Die()
    {
        Debug.Log("Player died");

        FindAnyObjectByType<GameManager>()?.CheckRemainingPlayers();

        RpcOnDeath();
    }

    [ClientRpc]
    void RpcOnDeath()
    {
        isInvulnerable = true;
        if (playerStats != null) playerStats.enabled = false;

        //disable key player components, to prevent moving/hitboxes while dead
        if(GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
        if(GetComponent<CharacterController>()) GetComponent<CharacterController>().enabled = false;
        if (GetComponent<Player>()) GetComponent<Player>().enabled = false;

        //Hide the player graphic
        if(transform.childCount > 0) transform.GetChild(0).gameObject.SetActive(false);
    }

    [Server]
    public void ResetPlayer(Vector3 spawnPosition)
    {
        lives = 3;
        isInvulnerable = false;

        transform.position = spawnPosition;

        RpcOnReset(spawnPosition);
    }

    [ClientRpc]
    void RpcOnReset(Vector3 spawnPosition)
    {

        CharacterController cc = GetComponent<CharacterController>();
        
        if(cc != null) cc.enabled = false;

        transform.position = spawnPosition;
        if(playerStats != null) playerStats.enabled = true;

        //enable key player components, to enable moving/hitboxes again
        if (cc != null) cc.enabled = true;
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = true;
        if (GetComponent<CharacterController>()) GetComponent<CharacterController>().enabled = true;
        if (GetComponent<Player>()) GetComponent<Player>().enabled = true;

        if (transform.childCount > 0) transform.GetChild(0).gameObject.SetActive(true);

        //Reset playerstats
        if (playerStats != null)
        {
            playerStats.activeBombs = 0;
            playerStats.maxBombs = 1;
            playerStats.explosionRange = 2;
        }
    }
}
