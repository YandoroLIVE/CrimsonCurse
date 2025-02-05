using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PausePhase : BossPhase
{
    [SerializeField] float pauseTime = 20;


  
    public override void OnEnable()
    {
        base.OnEnable();
        InitPhase();
    }

    private void InitPhase() 
    {
        StartPauseTimer();
    }

    private void StartPauseTimer() 
    {
        StartCoroutine(PauseTimer());
    }

    IEnumerator PauseTimer() 
    {
        yield return new WaitForSeconds(pauseTime);
        EndPhase();
    }


}
