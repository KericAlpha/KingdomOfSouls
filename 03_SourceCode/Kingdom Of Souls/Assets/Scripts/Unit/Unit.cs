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

            // MoveEffect(move, attacker);
            float dmgMultiplicator = MoveDMGMultiplicator(move, attacker);


            float damageWithoutMultiplicator = (move.MoveBase.Power * 3) * ((float)attacker.Attack / Defense);
            float multiplicator = typemultiplier * rngdamage * critical * dmgMultiplicator;
            int damage = Mathf.FloorToInt(damageWithoutMultiplicator * multiplicator);

            // Debug.Log(damage);
            // Debug.Log("Before " + attacker.UnitBase.Name + attacker.UnitBase.Name);

            HP = HP - damage;
            attacker.Mana -= move.ManaCost;

            // Debug.Log(Mana);

            if (HP <= 0)
            {
                HP = 0;
                return true;
            }

            return false;
        }

        else
        {

        }



        return false;
    }

    public Move GetRandomMove()
    {
        int random = Random.Range(0, Moves.Count);
        return Moves[random];
    }

    public void MoveEffect(Move move, Unit attacker)
    {
        switch (move.MoveBase.MoveEffect)
        {
            case global::MoveEffect.None:
                break;

            case global::MoveEffect.Random_Effect:
                break;

            case global::MoveEffect.Bleed:
                break;

            case global::MoveEffect.Burn:
                break;

            case global::MoveEffect.Freeze:
                break;

            case global::MoveEffect.Paralyze:
                break;

            case global::MoveEffect.B_All:
                break;

            case global::MoveEffect.B_Accuracy:
                break;

            case global::MoveEffect.B_DEF:
                break;

            case global::MoveEffect.B_DMG:
                break;

            case global::MoveEffect.B_SPEED:
                break;

            case global::MoveEffect.DB_All:
                break;

            case global::MoveEffect.S_E_Accuracy:
                break;

            case global::MoveEffect.S_E_DEF:
                break;

            case global::MoveEffect.S_E_DMG:
                break;

            case global::MoveEffect.S_E_SPEED:
                break;

            case global::MoveEffect.Focus:
                break;

            case global::MoveEffect.Block:
                break;

            case global::MoveEffect.Heal:
                break;
        }
    }

    public float MoveDMGMultiplicator(Move move, Unit attacker)
    {
        if(move.MoveBase.Multiplicator > 1F)
        {
            if(move.MoveBase.Name.Equals("Final Blow") && this.HP/this.MaxHP <= 0.5F)
            {
                Debug.Log("final blow works");
                return 1.2F;
            }
            if(move.MoveBase.Name.Equals("Primal Strike") && this.HP == this.MaxHP)
            {
                Debug.Log("primal strike works");
                return 1.5F;
            }
        }

        return 1F;
    }
}
