using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit/Create new Unit")]

public class UnitBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite sprite;

    [SerializeField] UnitType unitType;

    [SerializeField] int damage;
    [SerializeField] int maxHP;
    [SerializeField] int currentHP;
    [SerializeField] int maxMana;
    [SerializeField] int currentMana;
    [SerializeField] int defense;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] List<MoveType> resistance;
    [SerializeField] List<MoveType> weakness;

    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public Sprite Sprite
    {
        get { return sprite; }
    }
    public UnitType UnitType
    {
        get { return unitType; }
    }
    public int Damage
    {
        get { return damage; }
    }
    public int MaxHP
    {
        get { return maxHP; }
    }
    public int CurrentHP
    {
        get { return currentHP; }
    }
    public int MaxMana
    {
        get { return maxMana; }
    }
    public int CurrentMana
    {
        get { return currentMana; }
    }
    public int Defense
    {
        get { return defense; }
    }
    public int Speed
    {
        get { return speed; }
    }
    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }
    public List<MoveType> Resistance
    {
        get { return resistance; }
    }
    public List <MoveType> Weakness
    {
        get { return weakness; }
    }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int levelToLearn;

    public MoveBase MoveBase
    {
        get { return moveBase; }
    }
    public int LevelToLearn
    {
        get { return levelToLearn; }
    }
}


public enum UnitType
{
    Test,
    Warrior, 
    Assassin, 
    Magician, 
    Healer, 
    Supporter, 
    Undead
}

public enum Stat
{
    Attack,
    Defense,
    Speed
}

