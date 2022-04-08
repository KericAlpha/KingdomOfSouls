using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    [SerializeField] List<Unit> units;

    public List<Unit> Units
    {
        get { return units; }
    }

    private void Start()
    {
        foreach (var unit in units)
        {
            unit.Init();
        }
    }

    public Unit GetNotFaintedUnit()
    {
        foreach(var unit in units)
        {
            if(unit.HP > 0)
            {
                return unit;
            }
        }

        return null;
    }
}
