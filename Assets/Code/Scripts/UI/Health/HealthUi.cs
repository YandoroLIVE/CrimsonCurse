using System.Collections.Generic;
using UnityEngine;

public class HealthUi : MonoBehaviour
{
    [Header("Life Crystals")]
    public List<GameObject> lifeCrystals;

    public int currentLife;

    private void Start()
    {
        currentLife = lifeCrystals.Count;
    }

    public void GetDamage()
    {
        if (currentLife > 0)
        {
            currentLife--;
            lifeCrystals[currentLife].SetActive(false);

            if (currentLife == 0)
            {
                Debug.Log("Game Over!");
                HandleGameOver();
            }
        }
    }

    private void HandleGameOver()
    {

    }
}