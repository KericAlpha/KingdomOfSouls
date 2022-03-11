using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject hp;

    public void SetHP(float hpNormalized)
    {
        hp.transform.localScale = new Vector3(hpNormalized, 1F);
    }

    public IEnumerator SetHPSmooth(float newHP)
    {
        float curHP = hp.transform.localScale.x;
        float changeAmt = curHP - newHP;

        while(curHP - newHP > Mathf.Epsilon)
        {
            curHP -= changeAmt * Time.deltaTime;
            hp.transform.localScale = new Vector3(curHP, 1f);
            yield return null;
        }

        hp.transform.localScale = new Vector3(newHP, 1f);
    }
}
