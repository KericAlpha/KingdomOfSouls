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
        yield return hpBar.SetHPSmooth((float)unit.HP / unit.MaxHP);
        hpBar.SetHPText(unit.HP, unit.MaxHP);
    }

    public IEnumerator UpdateMana()
    {
        // Debug.Log(unit.UnitBase.Name + unit.Mana);
        yield return manaBar.SetManaSmooth((float)unit.Mana / unit.MaxMana);
        manaBar.SetManaText(unit.Mana, unit.MaxMana);
    }
}
