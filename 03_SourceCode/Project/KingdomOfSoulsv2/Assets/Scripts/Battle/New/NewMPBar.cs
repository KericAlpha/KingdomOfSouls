using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewMPBar : MonoBehaviour
{
    [SerializeField] TMP_Text mpText;

    public void SetMPText(float newMP, float maxMP)
    {
        mpText.text = newMP.ToString() + "/" + maxMP.ToString();

    }
}
