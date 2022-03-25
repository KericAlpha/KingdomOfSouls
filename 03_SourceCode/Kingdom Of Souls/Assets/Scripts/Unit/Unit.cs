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
        private set { _unit = value; }
    }
    public int Level{ 
        get { return _level; } 
        private set { _level = value; }
    }

    public int HP { get; set; }
    public int Mana { get; set; }
    public List<Move> Moves { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }


    public Unit(UnitBase uBase, int level)
    {
        _unit = uBase;
        _level = level;

        //Generate Moves
        Moves = new List<Move>();
        foreach (var move in UnitBase.LearnableMoves)
        {
            if (move.LevelToLearn <= Level)
            {
                Moves.Add(new Move(move.MoveBase));
            }
        }

        CalculateStats();

        HP = MaxHP;
        Mana = MaxMana;

        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.Speed, 0}
        };
    }

    /*public void Init()
    {
        
    }*/

   

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, UnitBase.Damage * this.Level);
        Stats.Add(Stat.Defense, UnitBase.Defense * this.Level);
        Stats.Add(Stat.Speed, UnitBase.Speed * this.Level);

        MaxHP = UnitBase.MaxHP * this.Level;
        MaxMana = UnitBase.MaxMana * this.Level;
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        // Apply Stat Boost
        int boost = StatBoosts[stat];
        var boostValues = new float[] {1F, 1.5F, 2F, 2.5F, 3F };

        if(boost >= 0)
        {
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        }
        else
        {
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
        }

        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach(var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -4, 4);

            Debug.Log("stat boost work");
        }
    }

    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }
    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }
    public int MaxHP
    {
        get; private set;
    }
    public int MaxMana
    {
        get; private set;
    }
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public MoveType hasWall = MoveType.None;

    public int focus = 0;

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

            

            MoveEffect(move, attacker);
            float dmgMultiplicator = MoveDMGMultiplicator(move, attacker);


            float damageWithoutMultiplicator = (move.MoveBase.Power * 3) * ((float)attacker.Attack / Defense);
            float multiplicator = typemultiplier * rngdamage * critical * dmgMultiplicator;
            int damage = Mathf.FloorToInt(damageWithoutMultiplicator * multiplicator);

            // Debug.Log(damage);
            // Debug.Log("Before " + attacker.UnitBase.Name + attacker.UnitBase.Name);

            if(Random.value * 100F <= move.MoveBase.Accuracy)
            {
                if(hasWall == move.MoveBase.MoveType)
                {
                    hasWall = MoveType.None;
                }
                else
                {
                    HP = HP - damage;
                }
            }

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

    public bool LowerMana(Move move)
    {
        // return true if enough mana for move, false if not enough mana, all in all lower mana if move is possible to use

        if(this.Mana >= move.ManaCost)
        {
            this.Mana -= move.ManaCost;
            return true;
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
                focus += 4;
                break;

            case global::MoveEffect.Block:
                attacker.hasWall = move.MoveBase.MoveType;
                break;

            case global::MoveEffect.Heal:
                break;
        }
    }

    public float MoveDMGMultiplicator(Move move, Unit attacker)
    {
        bool meetsCondition = false;

        if (move.MoveBase.DMGMultiplicatorCondition != DMGMultiplicatorCondition.None)
        {
            if (this.HP / this.MaxHP <= move.MoveBase.HPConditionValue/100F && move.MoveBase.DMGMultiplicatorCondition == DMGMultiplicatorCondition.BelowEnemyHP)
            {
                meetsCondition = true;
            }

            else if (this.HP / this.MaxHP >= move.MoveBase.HPConditionValue/100F && move.MoveBase.DMGMultiplicatorCondition == DMGMultiplicatorCondition.AboveEnemyHP)
            {
                meetsCondition = true;
            } 
        }

        if (meetsCondition)
        {
            return move.MoveBase.DMGMultiplicator;
        }

        return 1F;
    }
}
