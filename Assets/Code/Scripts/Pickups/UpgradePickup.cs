using HeroController;
using NUnit.Framework;
using UnityEngine;

public abstract class UpgradePickup : MonoBehaviour
{
    
    
    void OnTriggerEnter2D(Collider2D other)
    {
        OnPickup();    
    }


    public abstract void OnPickup();
}
