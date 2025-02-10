using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MinimapRoomManager : MonoBehaviour
{
    public Image[] roomOverlays; // Alle Raum-Overlays (UI-Vierecke)
    public Color defaultColor = new Color(1, 1, 1, 0); // Unsichtbar
    public Color highlightColor = new Color(0, 1, 0, 0.5f); // Grün mit 50% Transparenz

    void Start()
    {
        HighlightRoomByScene();
    }
    void Update() {
        HighlightRoomByScene();
    }

    void HighlightRoomByScene()
    {
        // Alle Räume auf Standardfarbe setzen
        foreach (Image room in roomOverlays)
        {
            room.color = defaultColor;
        }

        // Szene anhand des Index oder Namens ermitteln
        int sceneIndex = SceneManager.GetActiveScene().buildIndex - 1;

        // Nur den aktuellen Raum hervorheben, falls er existiert
        if (sceneIndex >= 0 && sceneIndex < roomOverlays.Length)
        {
            roomOverlays[sceneIndex].color = highlightColor;
        }
    }
}
