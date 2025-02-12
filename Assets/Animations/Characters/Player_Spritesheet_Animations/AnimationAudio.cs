using HeroController;
using UnityEngine;
using UnityEngine.Audio;
public class AnimationAudio : MonoBehaviour
{

    bool helper = false;
    [SerializeField] AudioClip[] forrestSFX;
    [SerializeField] AudioClip[] caveSFX;

    void Start()
    {
        if(S_PlayerHealth.GetInstance() != null)
        {
            helper = S_PlayerHealth.GetInstance().GetComponent<PlayerController>().forrestLevel;
        }
        
    }

    void AudioPlay()
    {
        if (helper)
        {
            ForrestEvent();
        }
        else
        {
            CaveEvent();
        }
    }
   

    void ForrestEvent()
    {
        AudioManager.instance?.PlayRandomSoundFXClip(forrestSFX, transform, 1f);
    }

    void CaveEvent()
    {
        AudioManager.instance?.PlayRandomSoundFXClip(caveSFX, transform, 1f);
    }
}
