using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] ManaBar manaBar;
    [SerializeField] GameObject statusIconOne;
    [SerializeField] GameObject statusIconTwo;
    [SerializeField] GameObject statusIconThree;
    [SerializeField] Sprite burnIcon;
    [SerializeField] Sprite freezeIcon;
    [SerializeField] Sprite sleepIcon;


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

    public void SetStatusIcon()
    {
        if(unit.StatusL.Count == 0)
        {
            statusIconOne.SetActive(false);
            statusIconTwo.SetActive(false);
            statusIconThree.SetActive(false);
        }
        else
        {
            foreach(Condition status in unit.StatusL)
            {
                if(status != null)
                {
                    if(!statusIconOne.activeSelf)
                    {

                    }
                    else if (!statusIconTwo.activeSelf)
                    {

                    }
                    else if(!statusIconThree.activeSelf)
                    {

                    }
                }
            }
            statusIconOne.SetActive(true);
            statusIconOne.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Battle/Burn_Icon");
            Debug.Log("test");
            Debug.Log(unit.StatusL.Count);
        }
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
