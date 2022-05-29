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

    bool showing = false;
    bool mini = false;

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
        if(!showing)
        {
            float xAxis = this.transform.position.x;
            float yAxis = this.transform.position.y;
            float ny = yAxis + 2f;
            
            this.transform.position = new Vector3(xAxis, ny);
            showing = true;
            mini = false;
        }
        
    }

    public void MiniHud()
    {
        if(!mini)
        {
            float xAxis = this.transform.position.x;
            float yAxis = this.transform.position.y;
            float ny = yAxis - 2f;

            this.transform.position = new Vector3(xAxis, ny, 0);
            mini = true;
            showing = false;
        }
        
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
