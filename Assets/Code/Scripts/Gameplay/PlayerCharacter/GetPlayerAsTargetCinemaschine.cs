using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GetPlayerAsTargetCinemaschine : MonoBehaviour
{
    [SerializeField] CinemachineCamera cam;
    PolygonCollider2D coll;
    void Start()
    {
        coll = FindAnyObjectByType<TilemapCollider2D>().gameObject.GetComponent<PolygonCollider2D>();
        cam.Target.TrackingTarget = S_PlayerHealth.GetInstance().transform;
        cam.GetComponent<CinemachineConfiner2D>().BoundingShape2D = coll;
    }
}
