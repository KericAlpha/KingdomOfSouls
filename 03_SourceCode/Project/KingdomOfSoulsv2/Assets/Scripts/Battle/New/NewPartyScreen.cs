using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPartyScreen : MonoBehaviour
{
    NewPMUI[] pMemberSlots;

    List<Unit> units;
    bool setInBattleBool = true;

    public void Init()
    {
        pMemberSlots = GetComponentsInChildren<NewPMUI>();
    }

    public int getIndexOfUnit(int pmIndex)
    {
        int counter = 0;

        for(int i = 0; i < units.Count; i++)
        {
            //Debug.Log(units[i].UnitBase.name + ": " + units[i].isInBattle);
            if(units[i].isInBattle == false)
            {
                if(counter == pmIndex)
                {
                    return i;
                }
                counter++;
            }
        }

        // uh ohh
        //Debug.Log("ah hell naw");
        return -1;
    }

    public void SetPartyData(List<Unit> units)
    {
        this.units = units;

        // useless for now 21.5.22
        if(setInBattleBool)
        {
            for (int i = 0; i < units.Count; i++)
            {
                if (i <= 3)
                {
                    units[i].isInBattle = true;
                }
                else
                {
                    units[i].isInBattle = false;
                }
            }
        }

        setInBattleBool = false;

        int counter = 0;

        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].isInBattle == false)
            {
                pMemberSlots[counter].gameObject.SetActive(true);
                pMemberSlots[counter].SetData(units[i]);
                counter++;
            }
            else
            {
                pMemberSlots[counter].gameObject.SetActive(false);
            }
        }

        while(counter < 3)
        {
            pMemberSlots[counter].gameObject.SetActive(false);
            counter++;
        }
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == selectedMember)
            {
                pMemberSlots[i].SetSelected(true);
            }
            else
            {
                pMemberSlots[i].SetSelected(false);
            }
        }
    }
}
