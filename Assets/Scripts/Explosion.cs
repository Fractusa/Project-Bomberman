using UnityEngine;
using Mirror;
using System.Collections;

public class Explosion : NetworkBehaviour
{
    public float destructionDelay = 0.5f;
    
    public override void OnStartServer()
    {
        base.OnStartServer();

        StartCoroutine(DestroyAfterDelay());
    }

    [Server]
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destructionDelay);

        NetworkServer.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if(health != null)
            {
                health.TakeDamage();
            }
            
        }
    }

}
