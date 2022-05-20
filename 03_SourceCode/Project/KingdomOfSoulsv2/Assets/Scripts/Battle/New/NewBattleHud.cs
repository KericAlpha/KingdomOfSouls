using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBattleHud : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] NewHPBar hpBar;
    [SerializeField] NewMPBar mpBar;
    [SerializeField] Soulbar soulbar;

    Unit unit;

    public void SetData(Unit unit)
    {
        this.unit = unit;

        nameText.text = unit.UnitBase.Name;
        levelText.text = "Lvl " + unit.Level;
        hpBar.SetHPText(unit.HP, unit.MaxHP);
        mpBar.SetMPText(unit.Mana, unit.MaxMana);
    }

    public void ShowHud()
    {
        float xAxis = this.gameObject.transform.localScale.x;
        this.transform.position = new Vector3(xAxis, -130, 0);
    }

    public void MiniHud()
    {
        float xAxis = this.gameObject.transform.localScale.x;
        this.transform.position = new Vector3(xAxis, -208, 0);
    }

    public void UpdateHP()
    {
        if (unit.HPChanged)
        {
            hpBar.SetHPText(unit.HP, unit.MaxHP);
            unit.HPChanged = false;
        }
    }

    public void UpdateMana()
    {
        mpBar.SetMPText(unit.Mana, unit.MaxMana);
    }

    public void UpdateSoulbar()
    {
        // unit soll einen soul counter haben, wird später implementiert
    }
}
