using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ExitTrigger : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            FindObjectOfType<GameManager>().TryExit(other.transform.position);
    }
}
