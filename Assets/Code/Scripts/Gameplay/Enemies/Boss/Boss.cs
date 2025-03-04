using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;



public class Boss : MonoBehaviour
{
    bool isBeaten = false;
    [SerializeField] string winSceneName = "WinScene";
    private int currentPhaseCounter = 0;
    [SerializeField] private List<BossPhase> phaseTransitions;
    (Rigidbody2D rigidbody, S_PlayerHealth health) player;
    //[SerializeField] HealthPickup healthCrystalPrefab;
    public void Awake()
    {
        player.health = FindAnyObjectByType<S_PlayerHealth>();
        player.rigidbody = player.health.GetComponent<Rigidbody2D>();
        foreach (Transform child in transform) 
        {
            child.gameObject.SetActive(false);

        }
        //HealthPickup healthCrystal = Instantiate(healthCrystalPrefab);
        //healthCrystal.gameObject.SetActive(false);
        foreach (BossPhase phase in phaseTransitions) 
        {
            phase.player.health = player.health;
            phase.player.rigidbody = player.rigidbody;
            //phase.healthCrystal = healthCrystal;
            phase.ResetPhase();
        }
        phaseTransitions.First().gameObject.SetActive(true);
        phaseTransitions[currentPhaseCounter].ResetPhase();
    }

    public void Update()
    {
        if (!isBeaten && phaseTransitions[currentPhaseCounter].IsFinished()) 
        {
            InitNextPhase();
        }
    }

    public void InitNextPhase() 
    {
        currentPhaseCounter++;
        if (currentPhaseCounter >= phaseTransitions.Count) 
        {
            //Boss is defeated
            Debug.Log("Boss is Defeated");
            SceneManager.LoadScene(winSceneName);
            isBeaten = true;
        }
        else 
        { 
            phaseTransitions[currentPhaseCounter].gameObject.SetActive(true);
            phaseTransitions[currentPhaseCounter].ResetPhase();
        }
    }


}
