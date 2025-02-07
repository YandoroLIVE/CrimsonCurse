using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
public class HitStop : MonoBehaviour
{
    bool waiting;
    public GameObject hitStunImage;
    private Volume postProcessVolume;
    private ColorAdjustments colorAdjustments;


    public void Stop(float duration)
    {
        if (waiting)
            return;
        Time.timeScale = 0.0f;
        StartCoroutine(Wait(duration));
    }

    IEnumerator Wait (float duration)
    {
        waiting = true;
        hitStunImage.SetActive(true);
        colorAdjustments.postExposure.value = -2;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        waiting = false;
        colorAdjustments.postExposure.value = .24f;
        hitStunImage.SetActive(false);
    }

    void Start()
    {
        postProcessVolume = FindObjectOfType<Volume>(); // Holt das Post-Processing-Volume aus der Szene
        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out colorAdjustments))
        {
            Debug.Log("PostExposure gefunden!");
        }
    }

}
