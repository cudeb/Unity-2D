using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestText : MonoBehaviour
{
    private OrderManager theOrder;
    private DialogueManager theDM;

    public bool flag;
    public string[] texts;
    // Start is called before the first frame update
    void Start()
    {
        theOrder = FindObjectOfType<OrderManager>();
        theDM = FindObjectOfType<DialogueManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!flag)
        {
            StartCoroutine(ACoroutine());
        }
    }

    IEnumerator ACoroutine()
    {
        flag = true;
        theOrder.NotMove();
        theDM.ShowText(texts);
        yield return new WaitUntil(() => !theDM.talking);
        theOrder.Move();
    }

}
