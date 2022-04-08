using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManaBar : MonoBehaviour
{
    [SerializeField] GameObject mana;
    [SerializeField] TMP_Text manaText;

    public void SetMana(float manaNormalized)
    {
        mana.transform.localScale = new Vector3(manaNormalized, 1f);
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
    public void SetManaText(float newMana, float maxMana)
    {
        manaText.text = newMana.ToString() + "/" + maxMana.ToString();
    }
}

