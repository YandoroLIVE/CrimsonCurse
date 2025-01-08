using UnityEditor;
using UnityEngine;

public class IsPlayerInTrigger : MonoBehaviour
{
    bool doesPlayerOverlap = false;
    Collider2D player;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        doesPlayerOverlap = true;
        player = collision;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        doesPlayerOverlap = false;
    }

    public bool IsPlayerInBox() 
    {
        return doesPlayerOverlap;
    }

    public Collider2D GetPlayer() 
    {
        return player;
    }
}
