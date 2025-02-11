using UnityEngine;

public class ActivateDialogComplete : MonoBehaviour
{
    [SerializeField] DialogSystem dialogSystem;
    [SerializeField] GameObject objectToActivate;

    private void Update()
    {
        if(!dialogSystem.dialogCompleted && !dialogSystem.isActiveAndEnabled) 
        {
            dialogSystem.gameObject.SetActive(true);
        }
        if (dialogSystem.dialogCompleted) 
        {
            objectToActivate.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}
