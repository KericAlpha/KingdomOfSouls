using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] ManaBar manaBar;

    public void SetData(Unit unit)
    {
        nameText.text = unit.UnitBase.Name;
        levelText.text = "Lvl " + unit.Level;
        hpBar.SetHP((float)unit.HP / unit.MaxHP);
        manaBar.SetMana((float)unit.Mana / unit.MaxMana);
    }
}
