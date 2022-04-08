using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] ManaBar manaBar;

    Unit unit;

    public void SetData(Unit unit)
    {
        this.unit = unit;

        nameText.text = unit.UnitBase.Name;
        levelText.text = "Lvl " + unit.Level;
        hpBar.SetHP((float)unit.HP / unit.MaxHP);
        hpBar.SetHPText(unit.HP, unit.MaxHP);
        manaBar.SetMana((float)unit.Mana / unit.MaxMana);
        manaBar.SetManaText(unit.Mana, unit.MaxMana);
    }

    public void SetSelected(bool selected)
    {
        if(selected)
        {
            nameText.color = Color.red;
        }
        else
        {
            nameText.color = Color.white;
        }
    }
}
