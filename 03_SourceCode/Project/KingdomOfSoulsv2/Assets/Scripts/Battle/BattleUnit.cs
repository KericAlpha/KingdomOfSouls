using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public Unit Unit { get; set; }

    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }

    public BattleHud Hud
    {
        get { return hud; }
    }

    public void Setup(Unit unit)
    {
        Unit = unit;
        GetComponent<Image>().sprite = Unit.UnitBase.Sprite;

        hud.SetData(unit);
    }
}
