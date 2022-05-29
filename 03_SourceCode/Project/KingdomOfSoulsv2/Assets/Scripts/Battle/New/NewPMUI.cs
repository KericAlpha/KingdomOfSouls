using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewPMUI : MonoBehaviour
{
    [SerializeField] TMP_Text name;
    [SerializeField] TMP_Text hp;
    [SerializeField] TMP_Text mp;

    Unit unit;

    public void SetData(Unit unit)
    {
        this.unit = unit;

        name.text = unit.UnitBase.Name;
        hp.text = unit.HP.ToString();
        mp.text = unit.Mana.ToString();
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            name.color = Color.cyan;
        }
        else
        {
            name.color = Color.white;
        }
    }
}
