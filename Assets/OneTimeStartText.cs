using UnityEngine;

public class OneTimeStartText : MonoBehaviour
{
    [SerializeField] DialogSystem dialog;
    private static bool displayed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!displayed) 
        {
            displayed = true;
            dialog.BecomeActive();
        }
    }
}
