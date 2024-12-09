using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private Vector3 lastCameraPosition; // Speichert die Position der Kamera beim letzten Frame
    [SerializeField] private float parallaxFactor = 0.5f; // Bestimmt die Stärke des Parallax-Effekts
    private Transform cameraTransform; // Referenz auf die Kamera

    private void Start()
    {
        // Findet die aktive Cinemachine-Kamera
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    private void Update()
    {
        // Berechnet die Bewegung der Kamera seit dem letzten Frame
        Vector3 cameraMovement = cameraTransform.position - lastCameraPosition;

        // Bewegt die Ebene in Abhängigkeit von der Kamerabewegung
        transform.position += new Vector3(cameraMovement.x * parallaxFactor, cameraMovement.y * parallaxFactor, 0);

        // Aktualisiert die letzte Kamera-Position
        lastCameraPosition = cameraTransform.position;
    }
}