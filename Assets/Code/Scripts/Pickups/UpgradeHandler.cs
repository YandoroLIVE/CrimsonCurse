using HeroController;
using NUnit.Framework;
using UnityEngine;

public class UpgradeHandler : MonoBehaviour
{
    private PlayerController _Player;
    private static UpgradeHandler _Instance;
    private bool _PickedUpDash = false;
    private bool _PickedUpWalljump = false;
    private bool _PickedUpLongrange = false;

    public static UpgradeHandler GetInstance() 
    {
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
        }
        else if (_Instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        UpdateStatus();
    }

    public void UpdateStatus()
    {
        if (_Player == null) 
        {
            _Player = FindFirstObjectByType<PlayerController>();
        }
        _Player.pickedUpDash = _PickedUpDash;
        _Player.hasWallJump = _PickedUpWalljump;

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
