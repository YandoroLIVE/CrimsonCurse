using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField]
    private float lifetime = 6f;
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}