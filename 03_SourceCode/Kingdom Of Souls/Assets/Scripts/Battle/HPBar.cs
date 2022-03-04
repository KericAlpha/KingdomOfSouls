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
}
