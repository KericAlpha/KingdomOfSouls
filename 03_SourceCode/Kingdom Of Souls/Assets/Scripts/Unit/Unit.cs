using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public UnitBase UnitBase { get; set; }
    public int Level { get; set; }

    public int HP { get; set; }
    public int Mana { get; set; }

    public List<Move> Moves { get; set; }

    public Unit(UnitBase uBase, int level)
    {
        this.UnitBase = uBase;
        this.Level = level;
        HP = MaxHP;
        Mana = MaxMana;

        //Generate Moves
        Moves = new List<Move>();
        foreach(var move in uBase.LearnableMoves)
        {
            if(move.LevelToLearn <= level)
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

    public bool TakeDamage(Move move, Unit attacker)
    {
        if(move.MoveBase.MoveType != MoveType.Status)
        {
            float typemultiplier = 1F;
            float rngdamage = Random.Range(0.9F, 1F);
            float critical = 1F;

            if (Random.value * 100F <= 5F)
            {
                critical = 2F;
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
            float multiplicator = typemultiplier * rngdamage * critical;

            int damage = Mathf.FloorToInt(damageWithoutMultiplicator * multiplicator);

            // Debug.Log(damage);
            Debug.Log(Mana);

            HP = HP - damage;
            Mana = Mana - move.ManaCost;

            Debug.Log(Mana);

            if (HP <= 0)
            {
                HP = 0;
                return true;
            }

            return false;
        }



        return false;
    }

    public Move GetRandomMove()
    {
        int random = Random.Range(0, Moves.Count);
        return Moves[random];
    }
}
