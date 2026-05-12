using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float destructionDelay = 0.5f;
    void Start()
    {
        Destroy(gameObject, destructionDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player was hit by the flame");
        }
    }

}
