using Unity.Cinemachine;
using UnityEngine;

public class GetPlayerAsTargetCinemaschine : MonoBehaviour
{
    [SerializeField] CinemachineCamera cam;
    void Start()
    {
        cam.Target.TrackingTarget = S_PlayerHealth.GetInstance().transform;
    }
}
