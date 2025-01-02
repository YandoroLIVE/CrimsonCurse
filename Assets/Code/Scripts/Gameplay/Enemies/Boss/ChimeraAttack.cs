using UnityEngine;

public class ChimeraAttack : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string animatorShowFaceTriggerName;
    [SerializeField] private string animatorAttackTriggerName;
    public void ShowFace() 
    {
        animator.SetTrigger(animatorShowFaceTriggerName);
        //Play eyes appear anmiaton
    }

    

    public void Attack() 
    {
        //attack animation
        animator.SetTrigger(animatorAttackTriggerName);
    }
}
