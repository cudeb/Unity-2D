using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLight : MonoBehaviour
{
    public GameObject go;

    private bool flag =false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!flag)
        {
            flag = true;
            go.SetActive(true);
        }
    }
}
