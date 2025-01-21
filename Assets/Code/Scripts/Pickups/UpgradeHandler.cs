using HeroController;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeHandler : MonoBehaviour
{
    bool inMenu = false;
    private PlayerControllerNew _Player;
    private static UpgradeHandler _Instance;
    private bool _PickedUpDash = false;
    [SerializeField] private GameObject _DashIcon;
    private bool _PickedUpWalljump = false;
    [SerializeField] private GameObject _WalljumpIcon;
    private bool _PickedUpLongrange = false;
    [SerializeField] private GameObject _LongrangeIcon;


    public static UpgradeHandler GetInstance() 
    {
        if(_Instance == null) 
        {
            Debug.Log("No Upgrade Handler in scene");
            return null;
        }
        return _Instance;
    }

    public bool HasDash() 
    {
        return _Instance._PickedUpDash;
    }
    public bool HasWalljump() 
    {
        return _Instance._PickedUpWalljump;
    }
    public bool HasLongrange() 
    {
        return _Instance._PickedUpLongrange;
    }

    public static void ActivateDash() 
    {
        _Instance._PickedUpDash = true;
        _Instance.UpdateStatus();
    }
    public static void ActivateWallJump() 
    {
        _Instance._PickedUpWalljump = true;
        _Instance.UpdateStatus();
    }
    public static void ActivateLongrange() 
    {
        _Instance._PickedUpLongrange = true;
        _Instance.UpdateStatus();
    }
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(this);
            SceneManager.activeSceneChanged += ResetValues;

            UpdateStatus();
        }
        else if (_Instance != this)
        {
            Destroy(this.gameObject);
        }
        
    }

    private void ResetValues(Scene current, Scene next)
    {
        if (next.buildIndex == 0)
        {
            _PickedUpDash = false;
            _PickedUpLongrange = false;
            _PickedUpWalljump = false;
        }
        UpdateStatus();
    }

    public void UpdateStatus()
    {
        if (_Player == null) 
        {
            _Player = FindFirstObjectByType<PlayerControllerNew>();
            if (_Player == null)
            {
                Debug.Log("Player not found");
                return;
            }
        }
        
        _Player.pickedUpDash = _PickedUpDash;
        _DashIcon.SetActive(_PickedUpDash);
        _Player.hasWallJump = _PickedUpWalljump;
        _WalljumpIcon.SetActive(_PickedUpWalljump);
        _LongrangeIcon.SetActive(_PickedUpLongrange);

        if (!_PickedUpLongrange)
        {
            _Player.GetComponent<S_PlayerLongRangeAttack>().enabled = false;
        }
        else 
        {
            _Player.GetComponent<S_PlayerLongRangeAttack>().enabled = true;
        }
        //_Player.Longrange = _PickedUpLongrange
    }
}
