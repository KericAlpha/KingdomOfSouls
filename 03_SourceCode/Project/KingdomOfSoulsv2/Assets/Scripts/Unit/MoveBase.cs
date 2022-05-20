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
    [SerializeField] bool alwaysHits;
    [SerializeField] int manaCost;
    [SerializeField] MoveCategory moveCategory;
    [SerializeField] MoveEffects moveEffects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    [SerializeField] MoveTarget target;

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
    public bool AlwaysHits
    {
        get { return alwaysHits; }
    }
    public int ManaCost
    {
        get { return manaCost; }
    }
    public MoveCategory MoveCategory
    {
        get { return moveCategory; }
    }
    public MoveEffects MoveEffects
    {
        get { return moveEffects; }
    }
    public List<SecondaryEffects> SecondaryEffects
    {
        get { return secondaryEffects; }
    }
    public MoveTarget Target
    {
        get { return target; }
    }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
    public ConditionID Status
    {
        get { return status; }
    }
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance
    {
        get { return chance; }
    }
    public MoveTarget Target
    {
        get { return target; }
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}



public enum MoveType
{
    None,
    Physical,
    Electric,
    Fire,
    Ice,
    Light,
    Dark,
    Status
}

public enum MoveCategory
{
    Attack, Status
}

public enum MoveTarget
{
    Enemy, Self
}
