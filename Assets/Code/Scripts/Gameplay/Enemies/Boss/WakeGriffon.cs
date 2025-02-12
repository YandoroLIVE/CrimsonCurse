using UnityEngine;

public class WakeGriffon : MonoBehaviour
{
    [SerializeField] DialogSystem dialogSystem;
    [SerializeField] int dialogIdToActOn;
    [SerializeField] GameObject objectToSetInactive;
    [SerializeField] GameObject objectToSetActive;


    private void Awake()
    {
        if (dialogSystem == null)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        if(dialogSystem.GetCurrentDialogID() == dialogIdToActOn) 
        {
            objectToSetActive?.SetActive(true);
            objectToSetInactive?.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}
