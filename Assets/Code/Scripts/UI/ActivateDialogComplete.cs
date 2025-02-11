using UnityEngine;

public class ActivateDialogComplete : MonoBehaviour
{
    [SerializeField] DialogSystem dialogSystem;
    [SerializeField] GameObject objectToActivate;

    private void Update()
    {
        if(!dialogSystem.dialogCompleted && !dialogSystem.gameObject.activeInHierarchy) 
        {
            dialogSystem.BecomeActive();
        }
        if (dialogSystem.dialogCompleted) 
        {
            objectToActivate.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}
