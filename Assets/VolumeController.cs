using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{

    public AudioMixer audioMixer;

    public Slider volumeSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
{
    float value;
    if (audioMixer.GetFloat("MasterVolume", out value))
    {
        volumeSlider.value = value;
    }
    else
    {
        volumeSlider.value = -20; // default volume if not set
        audioMixer.SetFloat("MasterVolume", -20);
    }
    volumeSlider.onValueChanged.AddListener(SetVolume);
}


    // Update is called once per frame

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }
}
