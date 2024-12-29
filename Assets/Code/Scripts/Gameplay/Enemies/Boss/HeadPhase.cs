using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class HeadPhase : MonoBehaviour
{
    [SerializeField] private int crystal_Pool_Size = 6;
    [SerializeField] private List<Transform> crystalSpawnPositons; 
    List<CrystalProjectileEnemy> crystalObjects;

}
