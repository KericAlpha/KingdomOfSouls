using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TMP_Text messageText;

    PartyMemberUI[] pMemberSlots;

    List<Unit> units;

    public void Init()
    {
        pMemberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Unit> units)
    {
        this.units = units; 

        for(int i = 0; i < pMemberSlots.Length; i++)
        {
            if(i < units.Count)
            {
                pMemberSlots[i].SetData(units[i]);
            }
            else
            {
                pMemberSlots[i].gameObject.SetActive(false);
            }
        }

        messageText.text = "Select who should get switched.";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for(int i = 0; i < units.Count; i++)
        {
            if(i == selectedMember)
            {
                pMemberSlots[i].SetSelected(true);
            }
            else
            {
                pMemberSlots[i].SetSelected(false);
            }
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
