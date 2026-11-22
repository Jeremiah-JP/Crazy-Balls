using UnityEngine;

//[RequireComponent(typeof(Collider2D))]
public class DrainZone : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Ball ball)) ball.Break();
    }
}
