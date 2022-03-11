using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Move/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] MoveType moveType;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int manaCost;

    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public MoveType MoveType
    {
        get { return moveType; }
    }
    public int Power
    {
        get { return power; }
    }
    public int Accuracy
    {
        get { return accuracy; }
    }
    public int ManaCost
    {
        get { return manaCost; }
    }
}

public enum MoveType
{
    Test,
    Physical,
    Electric,
    Fire,
    Ice,
    Light,
    Dark,
    Status
}

public enum MoveEffect
{
    Freeze20,
    Freeze30,
    Freeze50,
    Freeze100,
    Burn20,
    Burn30,
    Burn50,
    Burn100,
    Para20,
    Para30,
    Para50,
    Para100
}
