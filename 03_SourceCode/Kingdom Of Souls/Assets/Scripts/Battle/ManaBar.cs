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
}
