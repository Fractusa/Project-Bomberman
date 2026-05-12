using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float destructionDelay = 0.5f;
    void Start()
    {
        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player was hit by the flame");
        }
    }

}
