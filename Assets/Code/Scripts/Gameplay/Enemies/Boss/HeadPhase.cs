
using UnityEngine;
using System.Collections.Generic;

public class HeadPhase : BossPhase
{
    private int crystal_Pool_Size;
    [SerializeField] private CrystalProjectileEnemy crystalProjectileEnemyPrefab;
    [SerializeField] private List<Vector3> crystalSpawnPositons;
    public float crystalRespawnTime = 5f;
    public float crystalDamage = 5f;
    private List<float> crystalDownTimers;
    private List<int> disabledCrystals;
    List<CrystalProjectileEnemy> crystalObjects;


    private void Awake()
    {
        crystal_Pool_Size = crystalSpawnPositons.Count;
        crystalDownTimers = new List<float>(crystal_Pool_Size);
        Debug.Log(crystalDownTimers.Capacity + " | " + crystal_Pool_Size);
        crystalObjects = new List<CrystalProjectileEnemy>(crystal_Pool_Size);
        disabledCrystals = new List<int>(crystal_Pool_Size);
        for(int i = 0; i < crystal_Pool_Size; ++i) 
        {
            CrystalProjectileEnemy tmp = Instantiate<CrystalProjectileEnemy>(crystalProjectileEnemyPrefab, crystalSpawnPositons[i], Quaternion.identity,this.transform);
            tmp.SetOwner(this);
            tmp.SetOrigin(crystalSpawnPositons[i]);
            tmp.damage = crystalDamage;
            crystalObjects.Add(tmp);
            crystalDownTimers.Add(i);
            
        }
    }


    public void CrystalDestroyed(CrystalProjectileEnemy crystalObject) 
    {
        int tmp = crystalObjects.IndexOf(crystalObject);
        disabledCrystals.Add(tmp);
    
    }


    private void UpdateCrystalTimers() 
    {
        List<int> aboutToBeRemoved = new List<int>();
        foreach (int i in disabledCrystals) 
        {
            crystalDownTimers[i] += Time.deltaTime;
            if(crystalDownTimers[i] >= crystalRespawnTime) 
            {
                aboutToBeRemoved.Add(i);
                crystalObjects[i].gameObject.SetActive(true);
                crystalDownTimers[i] = 0f;
            }
        }
        foreach (int j in aboutToBeRemoved) 
        {
            disabledCrystals.Remove(j);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach(Vector3 t in crystalSpawnPositons) 
        { 
            Gizmos.DrawWireSphere(t, 0.5f);
        }
    }

    public void Update()
    {
        if (disabledCrystals.Count > 0)
        {
            UpdateCrystalTimers();
        }
    }


    public override void ResetPhase()
    {
        base.ResetPhase();
        disabledCrystals.Clear();
        for (int i = 0; i < crystalDownTimers.Count; i++)
        {
            crystalDownTimers[i] = 0;
        }
        foreach (CrystalProjectileEnemy crystal in crystalObjects)
        {
            crystal.gameObject.SetActive(true);
        }
    }
    public override void OnEnable()
    {
        base.OnEnable();
        ResetPhase();
    }
}
