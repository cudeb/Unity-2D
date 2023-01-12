using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public int itemID;
    public int _count;
    public string pickUpSound;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(Input.GetKey(KeyCode.Z))
        {
            AudioManager.instance.Play(pickUpSound);
            Inventory.instance.GetAnItem(itemID, _count);
            Destroy(this.gameObject);
        }
    }
}
