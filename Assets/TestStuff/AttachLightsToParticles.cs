using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using System.Collections.Generic;

public class AttachLightsToParticles : MonoBehaviour
{
    public ParticleSystem particleSystem; // Referenz zum Partikelsystem
    public GameObject lightPrefab;        // Prefab des 2D-Lichts

    private ParticleSystem.Particle[] particles;
    private List<GameObject> activeLights = new List<GameObject>();

    void Start()
    {
        if (particleSystem == null || lightPrefab == null)
        {
            Debug.LogError("ParticleSystem oder LightPrefab ist nicht zugewiesen!");
            return;
        }

        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
    }

    void Update()
    {
        if (particles == null || particleSystem == null)
        {
            Debug.LogError("ParticleSystem ist nicht initialisiert!");
            return;
        }

        // Hole alle aktiven Partikel
        int particleCount = particleSystem.GetParticles(particles);

        // Stelle sicher, dass genug Lichter vorhanden sind
        while (activeLights.Count < particleCount)
        {
            GameObject lightObj = Instantiate(lightPrefab, Vector3.zero, Quaternion.identity);
            lightObj.transform.SetParent(transform); // Optional: Lichter als Kind-Objekte des Systems
            activeLights.Add(lightObj);
        }

        // Update Positionen der Lichter
        for (int i = 0; i < activeLights.Count; i++)
        {
            if (i < particleCount)
            {
                activeLights[i].SetActive(true);
                activeLights[i].transform.position = particles[i].position;
            }
            else
            {
                activeLights[i].SetActive(false);
            }
        }
    }
}