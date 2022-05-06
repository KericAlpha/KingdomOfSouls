using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleHud : MonoBehaviour
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

    public IEnumerator UpdateHP()
    {
        if(unit.HPChanged)
        {
            yield return hpBar.SetHPSmooth((float)unit.HP / unit.MaxHP);
            hpBar.SetHPText(unit.HP, unit.MaxHP);
            unit.HPChanged = false;
        }
    }

    public IEnumerator UpdateMana()
    {
        // Debug.Log(unit.UnitBase.Name + unit.Mana);
        yield return manaBar.SetManaSmooth((float)unit.Mana / unit.MaxMana);
        manaBar.SetManaText(unit.Mana, unit.MaxMana);
    }
}
