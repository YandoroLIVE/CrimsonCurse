using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    public abstract bool Execute();
}

public class Selector : Node
{
    private List<Node> _children;

    public Selector(List<Node> children)
    {
        _children = children;
    }

    public override bool Execute()
    {
        foreach (var child in _children)
        {
            if (child.Execute())
                return true; // Erfolgreich, Abbruch
        }
        return false; // Kein Kind erfolgreich
    }
}

public class Sequence : Node
{
    private List<Node> _children;

    public Sequence(List<Node> children)
    {
        _children = children;
    }

    public override bool Execute()
    {
        foreach (var child in _children)
        {
            if (!child.Execute())
                return false; // Fehlgeschlagen, Abbruch
        }
        return true; // Alle Kinder erfolgreich
    }
}

public class Task : Node
{
    private Func<bool> _action;

    public Task(Func<bool> action)
    {
        _action = action;
    }

    public override bool Execute()
    {
        return _action();
    }
}

public class S_FFBehaviourTree : MonoBehaviour
{
    private Node _tree;

    void Start()
    {
        // Beispiel-Baumaufbau
        _tree = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new Task(IsPlayerInRange),
                new Task(AttackPlayer)
            }),
            new Task(Patrol)
        });
    }

    void Update()
    {
        _tree.Execute();
    }

    private bool IsPlayerInRange()
    {
        // Beispielbedingung: Spieler in Reichweite?
        float distance = Vector3.Distance(transform.position, Player.Instance.transform.position);
        Debug.Log("Is Player In Range: " + (distance < 5f));
        return distance < 5f;
    }

    private bool AttackPlayer()
    {
        // Beispielaktion: Angriff
        Debug.Log("Attacking Player!");
        return true; // Erfolg
    }

    private bool Patrol()
    {
        // Beispielaktion: Patrouillieren
        Debug.Log("Patrolling...");
        return true; // Erfolg
    }
}

// Beispiel für die Player-Klasse (zum Testen)
public class Player : MonoBehaviour
{
    public static Player Instance;

    void Awake()
    {
        Instance = this;
    }
}
