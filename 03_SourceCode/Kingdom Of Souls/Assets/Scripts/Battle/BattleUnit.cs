using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] UnitBase uBase;
    [SerializeField] int level;

    public Unit Unit { get; set; }

    public void Setup()
    {
        Unit = new Unit(uBase, level);
        GetComponent<Image>().sprite = Unit.UnitBase.Sprite;
    }
}
