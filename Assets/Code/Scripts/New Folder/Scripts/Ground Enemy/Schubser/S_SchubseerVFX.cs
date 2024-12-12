using UnityEngine;

public class S_SchubseerVFX : MonoBehaviour
{
    public GameObject capsule; // Dein Capsule-Objekt
    public GameObject vfx; // Dein VFX-Objekt

    private void Update()
    {
        // Überprüfe, ob die Capsule deaktiviert ist
        if (!capsule.activeSelf)
        {
            // Wenn die Capsule deaktiviert ist, deaktiviere auch die VFX
            vfx.SetActive(false);
        }
    }
}