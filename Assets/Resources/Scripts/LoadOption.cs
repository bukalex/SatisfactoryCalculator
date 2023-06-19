using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadOption : MonoBehaviour
{
    public void changeOption()
    {
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            transform.parent.GetComponentsInChildren<Outline>()[i].enabled = false;
        }
        GetComponent<Outline>().enabled = true;
    }
}
