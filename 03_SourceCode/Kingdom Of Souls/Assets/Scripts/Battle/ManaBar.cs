using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : MonoBehaviour
{
    [SerializeField] GameObject mana;

    public void SetMana(float manaNormalized)
    {
        mana.transform.localScale = new Vector3(manaNormalized, 1F);
    }

    public IEnumerator SetManaSmooth(float newMana)
    {
        float curMana = mana.transform.localScale.x;
        float changeAmt = curMana - newMana;

        while (curMana - newMana > Mathf.Epsilon)
        {
            curMana -= changeAmt * Time.deltaTime;
            mana.transform.localScale = new Vector3(curMana, 1f);
            yield return null;
        }

        mana.transform.localScale = new Vector3(newMana, 1f);
    }
}
