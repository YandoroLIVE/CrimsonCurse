using HeroController;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SafepointObject : MonoBehaviour
{
    private Safepoint _data;
    public static bool _Respawned;
    [SerializeField] private ParticleSystem _NotActive;
    [SerializeField] private ParticleSystem _Active;
    [SerializeField] AudioClip safepointSoundClip;

    private void Start()
    {
        Vector2 pos = transform.position;
        string currentScene = SceneManager.GetActiveScene().name;
        _data = Safepoint.DoesSafepointExist(pos, currentScene);
        if (_data == null) 
        {
            _data = new Safepoint(pos, false, currentScene);
        }

        if (_data.isActive)
        {
            SetVFXForActive();
        }
        else
        {
            SetVFXForNotActive();
        }

        if(_Respawned) 
        {
            Transform player = FindAnyObjectByType<PlayerController>().transform;
            player.position = new Vector3(_data.position.x, _data.position.y, player.position.z);
            _Respawned = false;
        }

    }

    private void SetVFXForNotActive()
    {
        _NotActive.Play();
        _Active.Stop();
        _Active.Clear();
    }

    private void SetVFXForActive()
    {
        _NotActive.Stop();
        _NotActive.Clear();
        _Active.Play();
    }

    public void ActivateSafepoint() 
    {
        if (_data.isActive == false)
        {
            AudioManager.instance.PlaySoundFXClip(safepointSoundClip, transform, .3f);
        }
        _data.SetAsCurrentSafepoint(_data.iD);
        SetVFXForActive();

        

    }

    public static void LoadCurrentSafepoint() 
    {
        Safepoint safe = Safepoint.GetCurrentSafepoint();
        SceneManager.LoadScene(safe.targetScene);
        _Respawned = true;
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player") 
        {
            if (Input.GetKey(KeyCode.Q)) 
            {
                ActivateSafepoint();           
            }            
        }
    }
}
