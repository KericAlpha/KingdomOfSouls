using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewHPBar : MonoBehaviour
{
    [SerializeField] TMP_Text hpText;

    public void SetHPText(float newHP, float maxHP)
    {
        hpText.text = newHP.ToString() + "/" + maxHP.ToString();

    }
}
