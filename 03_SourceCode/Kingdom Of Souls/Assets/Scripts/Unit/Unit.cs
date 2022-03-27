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
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public Condition Status { get; private set; }
    public bool ChangeHP { get; set; }


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

        ResetStatBoosts();
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
        Stats.Add(Stat.Accuracy, 100);

        MaxHP = UnitBase.MaxHP * this.Level;
        MaxMana = UnitBase.MaxMana * this.Level;
    }

    void ResetStatBoosts()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0}
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        
        if(stat == Stat.Accuracy)
        {
            // Increase or Lower Accuracy
            statVal += (10 * StatBoosts[stat]);
        }

        else
        {
            // Apply Stat Boost
            int boost = StatBoosts[stat];
            var boostValues = new float[] { 1F, 1.5F, 2F, 2.5F, 3F };

            if (boost >= 0)
            {
                statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
            }
            else
            {
                statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
            }
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

            if(boost > 0)
            {
                StatusChanges.Enqueue($"{UnitBase.Name}'s {stat} increased.");
            }
            else
            {
                StatusChanges.Enqueue($"{UnitBase.Name}'s {stat} decreased.");
            }

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

    public int Accuracy 
    {
        get { return GetStat(Stat.Accuracy); }
    }

    public MoveType hasWall = MoveType.None;

    public int focus = 0;

    public bool TakeDamage(Move move, Unit attacker)
    {

        float typemultiplier = 1F;
        float rngdamage = Random.Range(0.9F, 1F);
        float critical = 1F;

        // Critical Hit Check
        if (Random.value * 100F <= 5F)
        {
            critical = 2F;

            // do animation if it is a crit
        }

        // If Defender is weak to the move -> deal more damage
        if (this.UnitBase.Weakness.Contains(move.MoveBase.MoveType))
        {
            typemultiplier += 0.5F;
        }

        // If Defender is strong to the move -> deal less damage
        if (this.UnitBase.Resistance.Contains(move.MoveBase.MoveType))
        {
            typemultiplier -= 0.5F;
        }

        // Checks if move is Focus/Wall/Heal
        MoveEffect(move, attacker);

        // Implies a Move DMG Multiplicator 
        float dmgMultiplicator = MoveDMGMultiplicator(move, attacker);


        // Calculates the Damage
        float damageWithoutMultiplicator = (move.MoveBase.Power * 3) * ((float)attacker.Attack / Defense);
        float multiplicator = typemultiplier * rngdamage * critical * dmgMultiplicator;
        int damage = Mathf.FloorToInt(damageWithoutMultiplicator * multiplicator);

        // Debug.Log(damage);
        // Debug.Log("Before " + attacker.UnitBase.Name + attacker.UnitBase.Name);

        // Debug.Log(attacker.UnitBase.Name + " Accuracy: "  + move.MoveBase.Accuracy * (attacker.Accuracy / 100F));

        // Calculates if the move hits
        if(Random.value * 100F <= move.MoveBase.Accuracy * (attacker.Accuracy / 100F))
        {
            // If the Defender has a Wall of the Moves Type -> deal no Damage
            if(hasWall == move.MoveBase.MoveType)
            {
                hasWall = MoveType.None;
            }
            else
            {
                UpdateHP(damage);
            }
        }

        attacker.LowerMana(move);

        // Debug.Log(Mana);

        //Checks if Defender has more than 0 HP
        if (HP <= 0)
        {
            // Defender is dead -> return true
            return true;
        }

        // Defender is alive -> return false
        return false;
    }

    public bool HasEnoughMana(Move move)
    {
        return this.Mana >= move.ManaCost;
    }

    public bool LowerMana(Move move)
    {
        // Return true if enough Mana for Move, false if not enough Mana, all in all lower Mana if Move is possible to use

        if(HasEnoughMana(move))
        {
            this.Mana -= move.ManaCost;
            return true;
        }

        return false;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        ChangeHP = true;
    }

    public void SetStatus(ConditionID conditionID)
    {
        Status = ConditionDB.Conditions[conditionID];
        StatusChanges.Enqueue($"{UnitBase.Name} {Status.StartMessage}");
    }

    public Move GetRandomMove()
    {
        int random = Random.Range(0, Moves.Count);
        return Moves[random];
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver()
    {
        ResetStatBoosts();
    }

    public void MoveEffect(Move move, Unit attacker)
    {
        switch (move.MoveBase.MoveEffect)
        {
            case global::MoveEffect.None:
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

        // If the Move has a DMG Multiplicator Condition

        if (move.MoveBase.DMGMultiplicatorCondition != DMGMultiplicatorCondition.None)
        {
            // Checks if the Condition is met

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
            // Returns the Multiplicator if Condition is met

            return move.MoveBase.DMGMultiplicator;
        }

        // Returns 1F as a Multiplicator if no Condition is met

        return 1F;
    }
}
