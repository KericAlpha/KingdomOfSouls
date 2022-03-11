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
    [SerializeField] bool AOE;

    [SerializeField] MoveEffect effect;
    [SerializeField] float chance;
    [SerializeField] bool all;
    


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
    None,
    Random_Effect,
    Freeze,
    Burn,
    Paralyze,
    Bleed,
    S_E_Accuracy,
    S_E_DMG,
    S_E_DEF,
    S_E_SPEED,
    B_Accuracy,
    B_DMG,
    B_DEF,
    B_SPEED,
    B_All,
    DB_All,
    Focus,
    Block,
    Heal,
}
