using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject hp;
    [SerializeField] TMP_Text hpText;

    public void SetHP(float hpNormalized)
    {
        hp.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHPSmooth(float newHP)
    {
        float curHP = hp.transform.localScale.x;
        float changeAmt = curHP - newHP;

        while (curHP - newHP > Mathf.Epsilon)
        {
            curHP -= changeAmt * Time.deltaTime;
            hp.transform.localScale = new Vector3(curHP, 1f);
            yield return null;
        }

        hp.transform.localScale = new Vector3(newHP, 1f);
    }

    public void SetHPText(float newHP, float maxHP)
    {
        hpText.text = newHP.ToString() + "/" + maxHP.ToString();

    }
}
