using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<GameManager>().OnCandyCollected();
            Destroy(gameObject);
        }
    }
}
