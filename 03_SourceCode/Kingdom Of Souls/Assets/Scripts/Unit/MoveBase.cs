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
    [SerializeField] MoveEffects moveEffects;
    [SerializeField] MoveTarget target;


    [SerializeField] bool aoe;
    [SerializeField] int charge;

    [SerializeField] MoveEffect effect;
    [SerializeField] float chance;
    [SerializeField] int statusLength;

    [SerializeField] float dmgMultiplicator;
    [SerializeField] DMGMultiplicatorCondition dmgMultiplicatorCondition;
    [SerializeField] float hpConditionValue;

    [SerializeField] bool heal;
    [SerializeField] float healMultiplier;
    
    [SerializeField] bool buff;
    [SerializeField] bool debuff;
    [SerializeField] bool status;
    


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
    public MoveEffects Effects
    {
        get { return moveEffects; }
    }
    public MoveTarget Target
    {
        get { return target; }
    }



    public MoveEffect MoveEffect
    {
        get { return effect; }
    }
    public float Chance
    {
        get { return chance; }
    }
    public float DMGMultiplicator
    {
        get { return dmgMultiplicator; }
    }
    public int Charge
    {
        get { return charge; }
    }
    public bool AOE
    {
        get { return aoe; }
    }
    public int StatusLength
    {
        get { return statusLength; }
    }
    public DMGMultiplicatorCondition DMGMultiplicatorCondition
    {
        get { return dmgMultiplicatorCondition; }
    }
    public float HPConditionValue
    {
        get { return hpConditionValue; }
    }
    public bool Heal
    {
        get { return heal; }
    }
    public float HealMultiplier
    {
        get { return healMultiplier; }
    }
    public bool Buff
    {
        get { return buff; }
    }
    public bool Debuff
    {
        get { return debuff; }
    }
    public bool Status
    {
        get { return status; }
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

public enum MoveTarget
{
    Enemy, Self
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

public enum DMGMultiplicatorCondition
{
    None,
    AboveEnemyHP,
    BelowEnemyHP,
    AbovePlayerHP,
    BelowPlayerHP,
}
