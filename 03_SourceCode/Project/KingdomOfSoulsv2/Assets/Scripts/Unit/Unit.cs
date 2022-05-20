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
    public Move CurrentMove { get; set; }

    public Dictionary<Stat, int> Stats { get; private set; }

    public Dictionary<Stat, int> StatBoosts { get; private set; }

    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public Condition Status { get; private set; }
    public List<Condition> StatusL { get; private set; } = new List<Condition>();
    public bool HPChanged { get; set; }
    public int StatusCureChance { get; set; } = 0;

    public void Init()
    {
        Moves = new List<Move>();

        // Generate Moves
        foreach(var move in UnitBase.LearnableMoves)
        {
            if(move.LevelToLearn <= Level)
            {
                Moves.Add(new Move(move.MoveBase));
            }
        }

        CalculateStats();

        HP = MaxHP;
        Mana = MaxMana;

        ResetStatBoosts();
    }

    public void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, UnitBase.Damage * this.Level);
        Stats.Add(Stat.Defense, UnitBase.Defense * this.Level);
        Stats.Add(Stat.Speed, UnitBase.Speed * this.Level);
        Stats.Add(Stat.Accuracy, 100);

        MaxHP = UnitBase.MaxHP * this.Level;
        MaxMana = UnitBase.MaxMana * this.Level;
    }

    void ResetStatBoosts()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0 },
            {Stat.Defense, 0 },
            {Stat.Speed, 0 },
            {Stat.Accuracy, 0 }
        };
    }

    public int GetStat(Stat stat)
    {
        int statValue = Stats[stat];

        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f };

        if(boost >= 0)
        {
            statValue = Mathf.FloorToInt(statValue * boostValues[boost]);
        }
        else
        {
            statValue = Mathf.FloorToInt(statValue / boostValues[-boost]);
        }

        return statValue;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach(var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -5, 5);

            if(boost > 0)
            {
                StatusChanges.Enqueue($"{UnitBase.Name}'s {stat} increased");
            }
            else
            {
                StatusChanges.Enqueue($"{UnitBase.Name}'s {stat} decreased");
            }
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

    public int Accuracy
    {
        get { return GetStat(Stat.Accuracy); }
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

        UpdateHP(damage);
        if(HP <= 0)
        {
            return true;
        }

        // als return vielleicht eine klasse mitgeben, die werte wie fainted, critical und iseffective hat, damit wir später beim animieren wissen, ob es vielleicht doch ein crit war

        return false;
    }

    public void SetStatus(ConditionID conditionID)
    {
        // Status = ConditionsDB.conditions[conditionID];
        StatusL.Add(ConditionsDB.conditions[conditionID]);

        StatusChanges.Enqueue($"{UnitBase.Name} {StatusL[StatusL.Count-1].StartMessage}");
    }

    public void CureStatus(Condition condition)
    {
        StatusCureChance = 0;
        StatusL.Remove(condition);
        // Status = null;
    }

    public void DecreaseMana(int manaCost)
    {
        Mana -= manaCost;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        HPChanged = true;
    }

    public Move GetRandomMove()
    {
        int random = Random.Range(0, Moves.Count);

        return Moves[random];
    }

    public void OnAfterTurn()
    {
        foreach(Condition status in StatusL)
        {
            status?.OnAfterTurn?.Invoke(this);
        }
        // Status?.OnAfterTurn?.Invoke(this);
    }

    public bool OnBeforeMove()
    {
        foreach(Condition status in StatusL)
        {
            if (status?.OnBeforeMove != null)
            {
                return status.OnBeforeMove(this);
            }
        }
        return true;
    }

    public void OnBattleOver()
    {
        ResetStatBoosts();
    }
}
