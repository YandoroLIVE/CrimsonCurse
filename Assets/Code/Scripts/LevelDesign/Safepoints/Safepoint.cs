using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Safepoint
{
    private static int IdCounter = 0;
    private static List<Safepoint> _safepoints = new();
    private static int _CurrentSafepointID;

    public Vector2 position;
    public bool isActive;
    public string targetScene;
    public int iD;

    public Safepoint(Vector2 pos, bool status, string currentScene)
    {
        position = pos;
        isActive = status;
        targetScene = currentScene;
        iD = IdCounter;
        _safepoints.Add(this);
        IdCounter++;
    }

    public static Safepoint DoesSafepointExist(Vector2 pos, string currentScene) 
    {
        Safepoint safepoint = null;

        foreach (Safepoint point in _safepoints) 
        {
            if(point.position == pos && point.targetScene == currentScene) 
            {
                safepoint = point; 
                break;
            }        
        }
        return safepoint;
    }

    public static Safepoint GetCurrentSafepoint() 
    {
        if (_CurrentSafepointID >= _safepoints.Count) 
        {
            return null;
        }
        return _safepoints[_CurrentSafepointID]; 
    }

    public Safepoint GetSpecificSafepoint(int id)
    {
        return _safepoints[id];
    }

    public void SetAsCurrentSafepoint(int id) 
    {
        Safepoint safepoint = _safepoints[_CurrentSafepointID];
        safepoint.isActive = false;
        safepoint = _safepoints[id];
        safepoint.isActive = true;
        _CurrentSafepointID = id;
    }
}
