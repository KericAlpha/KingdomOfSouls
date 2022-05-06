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
                OnAfterTurn = (Unit unit) =>
                {
                    // Burn tick damage
                    unit.UpdateHP(unit.HP/8);
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
                OnBeforeMove = (Unit unit) =>
                {
                    if(Random.Range(1,11) >= 4)
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
                OnBeforeMove = (Unit unit) =>
                {
                    if(Random.Range(1,11) >= 4)
                    {
                        // Breaks freeze
                        unit.StatusChanges.Enqueue($"{unit.UnitBase.Name} is not frozen anymore");
                        unit.CureStatus();
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
                OnBeforeMove = (Unit unit) =>
                {
                    if(Random.Range(1,11) >= 2)
                    {
                        // Breaks freeze
                        unit.StatusChanges.Enqueue($"{unit.UnitBase.Name} woke up");
                        unit.CureStatus();
                        return true;
                    }

                    // Still frozen
                    unit.StatusChanges.Enqueue($"{unit.UnitBase.Name} has sweet dreams");
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