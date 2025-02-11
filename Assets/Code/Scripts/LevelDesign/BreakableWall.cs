using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour, IHurtable
{
    private const int BREAKABLE_WALL_AMOUNT = 5;
    private static List<bool> brokenWalls = new List<bool>(BREAKABLE_WALL_AMOUNT);
    [SerializeField, Range(0, BREAKABLE_WALL_AMOUNT - 1)] int wallID;
    [SerializeField] private float health = 200;
    [SerializeField] private ParticleSystem damageVFX;
    [SerializeField] AudioClip breakSFX;
    [SerializeField] float volumeBreak;
    [SerializeField] AudioClip hitSFX;
    [SerializeField] float volumeHit;
    void Start()
    {
        if (brokenWalls.Count == 0)
        {
            for (int i = 0; i < BREAKABLE_WALL_AMOUNT; ++i)
            {
                brokenWalls.Add(false);
            }
        }

        else if (brokenWalls[wallID] == true) 
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Hurt(float damage) 
    {
        health -= damage;
        if (health <= 0)
        {
            brokenWalls[wallID] = true;
            damageVFX.transform.SetParent(null);
            this.gameObject.SetActive(false);
            AudioManager.instance?.PlaySoundFXClip(breakSFX, transform, volumeBreak);
        }
        else
        {
            damageVFX?.Play();
            AudioManager.instance?.PlaySoundFXClip(hitSFX, transform, volumeHit);
        }
        
        
    }

}
