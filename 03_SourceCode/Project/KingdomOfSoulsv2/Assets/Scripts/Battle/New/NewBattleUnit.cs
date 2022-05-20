using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] NewBattleHud hud;

    public Unit Unit { get; set; }

    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }

    public NewBattleHud Hud
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
