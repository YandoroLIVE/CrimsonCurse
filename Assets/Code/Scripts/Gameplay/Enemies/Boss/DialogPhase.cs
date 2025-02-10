using UnityEngine;

[System.Serializable]
public class DialogPhase : BossPhase
{
    [SerializeField] DialogSystem dialog;

    // Update is called once per frame
    void Update()
    {
        if (dialog.dialogCompleted) 
        {
            EndPhase();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        dialog.blockMovement = true;
    }
}
