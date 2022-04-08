using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Unit
{
    [SerializeField] UnitBase _unit;
    [SerializeField] int _level;

    public UnitBase UnitBase
    {
        get { return _unit; }
    }
    public int Level
    {
        get { return _level; }
    }


    public int HP { get; set; }
    public int Mana { get; set; }
    public List<Move> Moves { get; set; }

    public void Init()
    {
        HP = MaxHP;
        Mana = MaxMana;

        Moves = new List<Move>();

        // Generate Moves
        foreach(var move in UnitBase.LearnableMoves)
        {
            if(move.LevelToLearn <= Level)
            {
                Moves.Add(new Move(move.MoveBase));
            }
        }
    }

    public int Attack
    {
        get { return UnitBase.Damage * this.Level; }
    }
    public int Defense
    {
        get { return UnitBase.Defense * this.Level; }
    }
    public int MaxHP
    {
        get { return UnitBase.MaxHP * this.Level; } 
    }
    public int MaxMana
    {
        get { return UnitBase.MaxMana * this.Level; } 
    }
    public int Speed
    {
        get { return UnitBase.Speed * this.Level; }
    }

    public int Accuracy
    {
        get { return UnitBase.Accuracy * this.Level; }
    }

    public bool TakeDamage(Move move, Unit attacker)
    {
        float typemultiplier = 1F;
        float rngdamage = Random.Range(0.9F, 1F);
        float critical = 1F;
        float dmgMultiplicator = 1F;

        if (Random.value * 100F <= 5F)
        {
            critical = 2F;

            // do animation if it is a crit
        }

        if (this.UnitBase.Weakness.Contains(move.MoveBase.MoveType))
        {
            typemultiplier += 0.5F;
        }

        if (this.UnitBase.Resistance.Contains(move.MoveBase.MoveType))
        {
            typemultiplier -= 0.5F;
        }

        float damageWithoutMultiplicator = (move.MoveBase.Power * 3) * ((float)attacker.Attack / Defense);
        float multiplicator = typemultiplier * rngdamage * critical * dmgMultiplicator;
        int damage = Mathf.FloorToInt(damageWithoutMultiplicator * multiplicator);

        HP -= damage;
        attacker.Mana -= move.ManaCost;

        if(HP <= 0)
        {
            HP = 0;
            return true;
        }

        return false;
    }

    public Move GetRandomMove()
    {
        int random = Random.Range(0, Moves.Count);

        return Moves[random];
    }
}
