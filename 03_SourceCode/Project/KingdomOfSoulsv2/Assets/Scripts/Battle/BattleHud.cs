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
    [SerializeField] List<GameObject> statusIcons;
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
            statusIcons[0].SetActive(false);
            statusIcons[1].SetActive(false);
            statusIcons[2].SetActive(false);
        }
        else
        {
            int index = 0;
            foreach(Condition status in unit.StatusL)
            {
                if(status != null)
                {
                    if(!statusIcons[index].activeSelf)
                    {
                        statusIcons[index].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(unit.StatusL[index].SpritePath);
                        statusIcons[index].SetActive(true);
                        Debug.Log(index + unit.StatusL[index].SpritePath);
                    }
                    else if(statusIcons[index].activeSelf)
                    {

                    }

                    /*if (!statusIconOne.activeSelf)
                    {
                        statusIconOne.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(unit.StatusL[0].SpritePath);
                        statusIconOne.SetActive(true);
                    }
                    else if (!statusIconTwo.activeSelf)
                    {
                        statusIconTwo.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(unit.StatusL[1].SpritePath);
                        statusIconTwo.SetActive(true);
                    }
                    else if(!statusIconThree.activeSelf)
                    {
                        statusIconThree.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(unit.StatusL[2].SpritePath);
                        statusIconThree.SetActive(true);
                    }*/
                }
                else
                {
                    statusIcons[index].SetActive(false);
                }
                index++;
            }
            //statusIconOne.SetActive(true);
            //statusIconOne.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Battle/Burn_Icon");
            //Debug.Log("test");
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
