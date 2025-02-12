using UnityEngine;
using UnityEngine.Audio;
public class MixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("MasterVolume", level);
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("SoundEffectsVolume", level);
    }
    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", level);
    }
}
