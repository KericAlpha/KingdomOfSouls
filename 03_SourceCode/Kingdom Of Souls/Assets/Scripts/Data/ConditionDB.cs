using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>() 
    {
        {
            ConditionID.BRN,  
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burned",
                OnAfterTurn = (Unit unit) =>
                {
                    unit.UpdateHP(unit.MaxHP / 8);
                    unit.StatusChanges.Enqueue($"{unit.UnitBase.Name} is burning");
                }
            } 
        }
    };
}

public enum ConditionID
{
    NONE, BRN, PAR, FRZ
}
