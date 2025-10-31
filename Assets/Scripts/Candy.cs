using UnityEngine;

public class Candy : MonoBehaviour
{
    public float spinSpeed = 90f;
    void Update() { transform.Rotate(0, spinSpeed * Time.deltaTime, 0); }
}
