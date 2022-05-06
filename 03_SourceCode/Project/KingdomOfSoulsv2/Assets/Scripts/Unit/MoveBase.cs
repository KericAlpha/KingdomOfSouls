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
    [SerializeField] MoveCategory moveCategory;
    [SerializeField] MoveEffects moveEffects;
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
