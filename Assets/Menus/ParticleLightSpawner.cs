using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering.Universal; // Wichtig f�r 2D Light

public class ParticleLightSpawner : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public Light2D lightPrefab; // Hier kann ein 2D-Licht als Prefab zugewiesen werden
    private List<Light2D> activeLights = new List<Light2D>();

    private void Start()
    {
        if (particleSystem == null)
        {
            particleSystem = GetComponent<ParticleSystem>();
        }
        // Falls kein Partikelsystem vorhanden ist, geben wir eine Warnung aus
        if (particleSystem == null)
        {
            Debug.LogWarning("Kein Partikelsystem zugewiesen!");
            enabled = false;
            return;
        }
    }

    private void LateUpdate()
    {
        if (particleSystem == null) return;

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
        particleSystem.GetParticles(particles);

        while (activeLights.Count < particles.Length)
        {
            Light2D newLight = Instantiate(lightPrefab, transform);
            activeLights.Add(newLight);
        }

        for (int i = 0; i < activeLights.Count; i++)
        {
            if (i < particles.Length)
            {
                Vector3 particleWorldPos = particleSystem.transform.TransformPoint(particles[i].position); // Konvertiere lokale in Weltposition
                activeLights[i].transform.position = particleWorldPos;
                activeLights[i].enabled = true;
            }
            else
            {
                activeLights[i].enabled = false;
            }
        }
    }

}