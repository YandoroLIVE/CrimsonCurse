using System.Collections.Generic;
using UnityEngine;

public class DashEffect : MonoBehaviour
{
    private const int MAX_ALLOWED_SPECTRES = 20;
    private struct DashObject 
    {
        public GameObject gameObject;
        public SpriteRenderer renderer;
        public Material material;
    }
    [SerializeField] GameObject dashSpectrePrefab;
    [SerializeField] Material dashSpectreMaterial;
    [SerializeField] float spawnRate;
    float lastTime = -50000;
    List<DashObject> spectres = new List<DashObject>();
    int currentSpectreID = 0;


    public void Loop(Sprite currentSprite , Transform transform) 
    {
        if(Time.time >= lastTime + spawnRate) 
        {
            lastTime = Time.time;
            SpawnSpectre(currentSprite, transform);
        }
    }

    public void ResetCounter() 
    {
        currentSpectreID = 0;
    }

    private void SpawnSpectre(Sprite spriteToShow, Transform transform)
    {
        if (currentSpectreID < spectres.Count) 
        {
            DashObject tmp = spectres[currentSpectreID];
            InitDashobject(spriteToShow, tmp, transform);
            tmp.material.SetFloat("_T",Time.time);
            //spawn Spectre
        }

        else if(currentSpectreID >= MAX_ALLOWED_SPECTRES) 
        {
            ResetCounter();
            SpawnSpectre(spriteToShow, transform);
        }

        else
        {
            DashObject tmp = new DashObject();
            tmp.gameObject = Instantiate(dashSpectrePrefab);
            tmp.material = new Material(dashSpectreMaterial);
            tmp.renderer = tmp.gameObject.GetComponent<SpriteRenderer>();
            InitDashobject(spriteToShow, tmp, transform);
            spectres.Add(tmp);
        }
        currentSpectreID++;
    }

    private void InitDashobject(Sprite spriteToShow, DashObject objectToInit, Transform transform)
    {
        objectToInit.renderer.sprite = spriteToShow;
        objectToInit.gameObject.transform.position = transform.position;
        objectToInit.gameObject.transform.localScale = transform.localScale;
        objectToInit.gameObject.transform.localScale = new Vector3( objectToInit.gameObject.transform.localScale.x * Mathf.Sign(this.transform.localScale.x), 
                                                                    objectToInit.gameObject.transform.localScale.y, 
                                                                    objectToInit.gameObject.transform.localScale.z);
        objectToInit.material.SetFloat("_T", Time.time);
        objectToInit.renderer.material = objectToInit.material;
    }
}
