using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public MoveBase MoveBase { get; set; }
    public int ManaCost { get; set; }

    public Move(MoveBase moveBase)
    {
        MoveBase = moveBase;
        ManaCost = moveBase.ManaCost; 
    }

}
