using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static Dictionary<ConditionID, Condition> conditions { get; set; } = new Dictionary<ConditionID, Condition>
    {
        {   
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burned",
                SpritePath = "Battle/Burn_Icon",
                OnAfterTurn = (Unit unit) =>
                {
                    // Burn tick damage
                    unit.UpdateHP(unit.MaxHP/8);
                    unit.StatusChanges.Enqueue($"{unit.UnitBase.Name} is burning");
                }
            }
        },

        {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralyzed",
                StartMessage = "has been paralyzed",
                SpritePath = "Battle/Paralyzed_Icon",
                OnBeforeMove = (Unit unit) =>
                {
                    if(Random.Range(1,11) >= 5)
                    {
                        // Will not perform move
                        unit.StatusChanges.Enqueue($"{unit.UnitBase.Name} is paralyzed and can't move");
                        return false;
                    }

                    // Will perform move
                    return true;
                }

            }
        },

        {
            ConditionID.frz,
            new Condition()
            {
                Name = "Freeze",
                StartMessage = "has been frozen",
                SpritePath = "Battle/Freeze_Icon",
                OnBeforeMove = (Unit unit) =>
                {
                    if(Random.Range(1,11) >= 5)
                    {
                        // Breaks freeze
                        unit.StatusChanges.Enqueue($"{unit.UnitBase.Name} is not frozen anymore");
                        unit.CureStatus(ConditionsDB.conditions[ConditionID.frz]);
                        return true;
                    }

                    // Still frozen
                    return false;
                }

            }
        },

        {
            ConditionID.slp,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "is sleeping",
                SpritePath = "Battle/Sleep_Icon",
                OnBeforeMove = (Unit unit) =>
                {
                    if(Random.Range(1,11) <= 2 + unit.StatusCureChance)
                    {
                        // Wakes up
                        unit.StatusChanges.Enqueue($"{unit.UnitBase.Name} woke up");
                        unit.CureStatus(ConditionsDB.conditions[ConditionID.slp]);
                        return true;
                    }

                    // Still sleeping
                    unit.StatusCureChance = Mathf.Clamp(unit.StatusCureChance + 2, 0, 10);
                    unit.StatusChanges.Enqueue($"{unit.UnitBase.Name} is having sweet dreams");
                    return false;
                }

            }
        }
    };

}

public enum ConditionID
{
    none, brn, par, frz, slp
}