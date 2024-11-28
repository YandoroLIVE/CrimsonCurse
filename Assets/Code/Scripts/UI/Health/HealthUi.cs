using System.Collections.Generic;
using UnityEngine;

public class LifeCrystalManager : MonoBehaviour
{
    [Header("Life Crystals")]
    public List<GameObject> lifeCrystals;

    public int currentLife;

    private void Start()
    {
        currentLife = lifeCrystals.Count;
    }

    public void Update()
    {
        if (currentLife > 0)
        {

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