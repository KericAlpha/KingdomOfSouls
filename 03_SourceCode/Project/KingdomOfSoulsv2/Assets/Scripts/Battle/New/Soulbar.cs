using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soulbar : MonoBehaviour
{
    [SerializeField] GameObject souls;

    public void SetSouls(float curSouls)
    {
        souls.transform.localScale = new Vector3(curSouls/6, 1f);
    }
}
