
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
struct HeadCrystalPoint 
{
    public float maxHealth;
    public Vector3 position;
}

public class HeadPhase : BossPhase
{
    private int crystal_Pool_Size;
    [SerializeField] private CrystalProjectileEnemy crystalProjectileEnemyPrefab;
    [SerializeField] private BossHeadCrystals headCrystalPrefab;
    [SerializeField] private List<Vector3> crystalSpawnPositons;
    [SerializeField] private List<HeadCrystalPoint> headCrystalPoints;
    private List<BossHeadCrystals> headCrystalss;
    public float crystalRespawnTime = 5f;
    public int crystalHealAmount = 10;
    public float crystalDamage = 5f;
    public float crystalSpeed = 7f;
    private List<float> crystalDownTimers = new();
    private List<int> disabledCrystals = new();
    List<CrystalProjectileEnemy> crystalObjects = new();
    private Boss owner;

    
    

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
            tmp.SetPlayer(player.rigidbody, player.health);
            tmp.SetOwner(this);
            tmp.SetOrigin(crystalSpawnPositons[i]);
            tmp.damage = crystalDamage;
            tmp.speed = crystalSpeed;
            crystalObjects.Add(tmp);
            crystalDownTimers.Add(i);
            
        }
        foreach (HeadCrystalPoint crystal in headCrystalPoints) 
        {
            BossHeadCrystals tmp = Instantiate<BossHeadCrystals>(headCrystalPrefab,crystal.position, Quaternion.identity, this.transform);
            tmp.maxHealth = crystal.maxHealth;
            tmp.Heal();
            tmp.SetOwner(this);
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

        Gizmos.color = Color.blue;
        foreach(HeadCrystalPoint crystal in headCrystalPoints) 
        {
            Gizmos.DrawWireSphere(crystal.position, 0.5f);
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
        //ResetPhase();
    }
}
