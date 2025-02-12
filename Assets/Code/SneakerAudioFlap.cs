using UnityEngine;

public class SneakerAudioFlap : MonoBehaviour
{
    [SerializeField] AudioClip flap;

    void PlayFlap()
    {
        AudioManager.instance.PlaySoundFXClip(flap, transform, 1f);
    }
}
