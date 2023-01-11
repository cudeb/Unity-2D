using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private DatabaseManager theDatabase;
    private OrderManager theOrder;
    private AudioManager theAudio;
    private OkOrCancel theOOC;
    private Equipment theEquip;
    

    public string key_sound;
    public string enter_sound;
    public string cancel_sound;
    public string open_sound;
    public string beep_sound;

    private InventorySlot[] slots;

    private List<Item> inventoryItemList;
    private List<Item> inventoryTabList;

    public Text Description_Text;
    public string[] tabDescription;

    public Transform tf;

    public GameObject go;
    public GameObject[] selectedTabImages;
    public GameObject go_OOC;
    public GameObject prefab_Floating_Text;

    private int selectedItem;
    private int selectedTab;

    private int page;
    private int slotCount;
    private const int MAX_SLOTS_COUNT = 12;

    private bool activated;
    private bool tabActivated;
    private bool itemActivated;
    private bool stopKeyInput;
    private bool preventExec;  //GetKey 일 때 동시 실행 방지

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        theAudio = FindObjectOfType<AudioManager>();
        theOrder = FindObjectOfType<OrderManager>();
        theDatabase = FindObjectOfType<DatabaseManager>();
        theOOC = FindObjectOfType<OkOrCancel>();
        theEquip = FindObjectOfType<Equipment>();
        inventoryItemList = new List<Item>();
        inventoryTabList = new List<Item>();
        slots = tf.GetComponentsInChildren<InventorySlot>();
    }

    public List<Item> SaveItem()
    {
        return inventoryItemList;
    }

    public void LoadItem(List<Item> _itemList)
    {
        inventoryItemList = _itemList;
    }

    public void EquipToInventory(Item _item)
    {
        inventoryItemList.Add(_item);
    }

    public void GetAnItem(int _itemID, int _count = 1)
    {
        for(int i=0;i<theDatabase.itemList.Count;i++)   
        {
            if(_itemID == theDatabase.itemList[i].itemID)
            {

                var clone = Instantiate(prefab_Floating_Text, PlayerManager.instance.transform.position,Quaternion.Euler(Vector3.zero));
                clone.GetComponent<FloatingText>().text.text = theDatabase.itemList[i].itemName + " " + _count + "개 획득 +";
                clone.transform.SetParent(this.transform);
                for(int j=0;j<inventoryItemList.Count;j++)
                {
                    if(inventoryItemList[j].itemID == _itemID)
                    {
                        if (inventoryItemList[j].itemType == Item.ItemType.Use)
                        {
                            inventoryItemList[j].itemCount += _count;
                        }
                        else  //part 25 > 장비창도 추가 버그픽스
                        {
                            for (int k = 0; k < _count; k++)
                            {
                                inventoryItemList.Add(theDatabase.itemList[i]);
                            }
                        }
                        return;
                    }          
                }
                if (theDatabase.itemList[i].itemType == Item.ItemType.Use)
                {
                    inventoryItemList.Add(theDatabase.itemList[i]);
                    inventoryItemList[inventoryItemList.Count - 1].itemCount = _count;
                }
                else   //part 25 > 장비창도 추가 버그픽스
                {
                    for (int j = 0; j < _count; j++)
                    {
                        inventoryItemList.Add(theDatabase.itemList[i]);
                    }
                }
                return;
            }
        }
        Debug.LogError("데이터베이스에 해당 ID값을 가진 아이템이 존재하지 않습니다.");
    }

    public void ShowTab()
    {
        RemoveSlot();
        SelectedTab();
    }
    public void RemoveSlot()
    {
        for(int i=0;i<slots.Length;i++)
        {
            slots[i].RemoveItem();
            slots[i].gameObject.SetActive(false);
        }
    }
    public void SelectedTab()
    {
        StopAllCoroutines();
        Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
        color.a = 0f;
        for(int i=0;i<selectedTabImages.Length;i++)
        {
            selectedTabImages[i].GetComponent<Image>().color = color;
        }
        Description_Text.text = tabDescription[selectedTab];
        StartCoroutine(SelectedTabEffectCoroutine());
    }
    IEnumerator SelectedTabEffectCoroutine()
    {
        while(tabActivated)
        {
            Color color = selectedTabImages[0].GetComponent<Image>().color;
            while(color.a < 0.5f)
            {
                color.a += 0.03f;
                selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while(color.a > 0f)
            {
                color.a -= 0.03f;
                selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    public void ShowPage()
    {
        slotCount = -1;

        for (int i = page * MAX_SLOTS_COUNT; i < inventoryTabList.Count; i++)
        {
            slotCount = i - (page * MAX_SLOTS_COUNT);
            slots[slotCount].gameObject.SetActive(true);
            slots[slotCount].AddItem(inventoryTabList[i]);

            if(slotCount == MAX_SLOTS_COUNT - 1)
            {
                break;
            }
        }

    }
    public void ShowItem()
    {
        inventoryTabList.Clear();
        RemoveSlot();
        selectedItem = 0;
        page = 0;

        switch(selectedTab)
        {
            case 0:
                for (int i=0;i<inventoryItemList.Count;i++)
                {
                    if(Item.ItemType.Use == inventoryItemList[i].itemType)
                    {
                        inventoryTabList.Add(inventoryItemList[i]);
                    }
                }
                break;
            case 1:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Equip == inventoryItemList[i].itemType)
                    {
                        inventoryTabList.Add(inventoryItemList[i]);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Quest == inventoryItemList[i].itemType)
                    {
                        inventoryTabList.Add(inventoryItemList[i]);
                    }
                }
                break;
            case 3:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.ETC == inventoryItemList[i].itemType)
                    {
                        inventoryTabList.Add(inventoryItemList[i]);
                    }
                }
                break;
        }

        ShowPage();
        SelectedItem();

    }
    public void SelectedItem()
    {
        StopAllCoroutines();
        if(slotCount > -1)
        {
            Color color = slots[0].selected_Item.GetComponent<Image>().color;
            color.a = 0f;
            for(int i=0;i <= slotCount; i++)
            {
                slots[i].selected_Item.GetComponent<Image>().color = color;
            }
            StartCoroutine(SelectedItemEffectCoroutine());
            Description_Text.text = inventoryTabList[selectedItem].itemDescription;
        }
        else
        {
            Description_Text.text = "해당 타입의 아이템을 소유하고 있지 않습니다.";
        }
    }

    IEnumerator SelectedItemEffectCoroutine()
    {
        while (itemActivated)
        {
            Color color = slots[0].GetComponent<Image>().color;
            while (color.a < 0.5f)
            {
                color.a += 0.03f;
                slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f)
            {
                color.a -= 0.03f;
                slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(!stopKeyInput)
        {
            if(Input.GetKeyDown(KeyCode.I))
            {
                activated = !activated;

                if(activated)
                {
                    theAudio.Play(open_sound);
                    theOrder.NotMove();
                    go.SetActive(true);
                    selectedTab = 0;
                    tabActivated = true;
                    itemActivated = false;
                    ShowTab();
                }
                else
                {
                    theAudio.Play(cancel_sound);
                    StopAllCoroutines();
                    go.SetActive(false);
                    tabActivated = false;
                    itemActivated = false;
                    theOrder.Move();
                }
            }

            if(activated)
            {
                if(tabActivated)
                {
                    if(Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        if(selectedTab < selectedTabImages.Length - 1)
                        {
                            selectedTab++;
                        }
                        else
                        {
                            selectedTab = 0;
                        }
                        theAudio.Play(key_sound);
                        SelectedTab();
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        if (selectedTab > 0)
                        {
                            selectedTab--;
                        }
                        else
                        {
                            selectedTab = selectedTabImages.Length - 1;
                        }
                        theAudio.Play(key_sound);
                        SelectedTab();
                    }
                    else if(Input.GetKeyDown(KeyCode.Z))
                    {
                        theAudio.Play(enter_sound);
                        Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
                        color.a = 0.25f;
                        selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                        itemActivated = true;
                        tabActivated = false;
                        preventExec = true; 
                        ShowItem();
                    }
                }
                else if(itemActivated)
                {
                    if (inventoryTabList.Count > 0)
                    {
                        if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            if (selectedItem + 2 > slotCount)
                            {
                                if (page < (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT)
                                {
                                    page++;
                                }
                                else
                                {
                                    page = 0;
                                }

                                RemoveSlot();
                                ShowPage();
                                selectedItem = -2;
                            }
                            if (selectedItem < slotCount- 1)
                            {
                                selectedItem += 2;
                            }
                            else
                            {
                                selectedItem %= 2;
                            }
                            theAudio.Play(key_sound);
                            SelectedItem();
                        }
                        else if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            if (selectedItem -2 < 0)
                            {
                                if (page != 0)
                                {
                                    page--;
                                }
                                else
                                {
                                    page = (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT;
                                }

                                RemoveSlot();
                                ShowPage();
                                
                            }
                            if (selectedItem > 1)
                            {
                                selectedItem -= 2;
                            }
                            else
                            {
                                selectedItem = slotCount - selectedItem;
                            }
                            theAudio.Play(key_sound);
                            SelectedItem();
                        }
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            if (selectedItem + 1 > slotCount)
                            {
                                if (page < (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT)
                                {
                                    page++;
                                }
                                else
                                {
                                    page = 0;
                                }

                                RemoveSlot();
                                ShowPage();
                                selectedItem = -1;
                            }
                            if (selectedItem < slotCount)
                            {
                                selectedItem++;
                            }
                            else
                            {
                                selectedItem = 0;
                            }
                            theAudio.Play(key_sound);
                            SelectedItem();
                        }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            if (selectedItem - 1 < 0)
                            {
                                if (page != 0)
                                {
                                    page--;
                                }
                                else
                                {
                                    page = (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT;
                                }

                                RemoveSlot();
                                ShowPage();
                            }

                            if (selectedItem > 0)
                            {
                                selectedItem--;
                            } 
                            else
                            {
                                selectedItem = slotCount;
                            }
                            theAudio.Play(key_sound);
                            SelectedItem();
                        }
                        else if (Input.GetKeyDown(KeyCode.Z)&& !preventExec)
                        {
                            if (selectedTab == 0)
                            {
                                StartCoroutine(OOCCoroutine("사용","취소"));
                            }
                            else if (selectedTab == 1)
                            { 
                                StartCoroutine(OOCCoroutine("장착","취소"));
                            }
                            else
                            {
                                theAudio.Play(beep_sound);
                            }
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        theAudio.Play(cancel_sound);
                        StopAllCoroutines();
                        itemActivated = false;
                        tabActivated = true;
                        ShowTab();
                    }
                }
                
                if(Input.GetKeyUp(KeyCode.Z))
                {
                    preventExec = false;
                }
            }
        }
    }

    IEnumerator OOCCoroutine(string _up, string _down)
    {
        theAudio.Play(enter_sound);
        stopKeyInput = true;

        go_OOC.SetActive(true);
        theOOC.ShowTwoChoice(_up, _down);
        yield return new WaitUntil(() => !theOOC.activated);
        if (theOOC.GetResult())
        {
            for (int i = 0; i < inventoryItemList.Count; i++)
            {
                if (inventoryItemList[i].itemID == inventoryTabList[selectedItem].itemID)
                {
                    if (selectedTab == 0)
                    {
                        theDatabase.UseItem(inventoryItemList[i].itemID);
                        if (inventoryItemList[i].itemCount > 1)
                        {
                            inventoryItemList[i].itemCount--;
                        }
                        else
                        {
                            inventoryItemList.RemoveAt(i);
                        }
                        //theAudio.Play()
                        ShowItem();
                        break;
                    }
                    else if(selectedTab == 1)
                    {
                        theEquip.EquipItem(inventoryItemList[i]);
                        inventoryItemList.RemoveAt(i);
                        ShowItem();
                        break;
                    }
                }
            }
        }

        stopKeyInput = false;
        go_OOC.SetActive(false);
    }
}
