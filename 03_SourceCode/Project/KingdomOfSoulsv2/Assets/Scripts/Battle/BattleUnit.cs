using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    public Unit Unit { get; set; }

    public void Setup(Unit unit)
    {
        Unit = unit;
        GetComponent<Image>().sprite = Unit.UnitBase.Sprite;
    }
}
