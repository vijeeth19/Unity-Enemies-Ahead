using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Inventory : MonoBehaviour
{
    public Animator anim;
    public Transform slotParent;
    public GameObject slotPrefab;
    public int totalItems = 28;
    ItemDetails[] items;
    public int[] gameObjectIndex;
    
    public Image hotbarSlotOne, hotbarSlotTwo, hotbarSlotThree, hotbarSlotFour;
    public Transform hotbarParent;
    public Sprite selectedSlot, blankSprite;
    public Color defaultBGSlot;
    public ItemDetails[] hotbarItems; ItemDetails selectedItem;
    public ItemDetails normalBullet, hotbarSelectedItem;
    public int hbSelectedIndex;
    public bool isInventoryOpen = false;
    public IngameUI igui;
    
    public Button nextWave;
    public TextMeshProUGUI nextWaveDesc;
    public Image healthBG, healthIcon, healthBar;
    public ShopUI shop;
    public WallSpawner ws;
    public EnemySpawner es;

    void Start()
    {
        anim.SetBool("openInventory", false);
        FindObjectOfType<ShopUI>().ItemBought += this.OnItemBought;
        items = new ItemDetails[totalItems];
        gameObjectIndex = new int[totalItems];
        hotbarItems = new ItemDetails[4];

        items[28-1] = Instantiate(normalBullet);
        gameObjectIndex[28-1] = -1;
        hotbarItems[0] = items[28-1];
        hotbarSelectedItem = hotbarItems[0];
        hbSelectedIndex = 0;
    }
    IEnumerator waveDescCoroutine;
    public void OpenBetweenWaves(){
        if(PlayerPrefs.GetInt("DoneTut", 0) == 1){
            nextWave.enabled = true;
            nextWave.transform.GetComponent<Image>().enabled = true;
            nextWave.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = true;
        }
        nextWaveDesc.enabled = true;
        healthBG.enabled = false;
        healthIcon.enabled = false;
        healthBar.enabled = false;
        waveDescCoroutine = TextAnim();
        StartCoroutine(waveDescCoroutine);
        ws.touchShiftUp = 90;
    }
    IEnumerator TextAnim(){
        while(true){
            nextWaveDesc.fontSize = 15f + Mathf.PingPong(Time.time*6, 6);
            yield return null;
        }
    }

    public void CloseBetweenWaves(){
        nextWave.enabled = false;
        nextWave.transform.GetComponent<Image>().enabled = false;
        nextWave.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = false;
        nextWaveDesc.enabled = false;
        OnCloseInventory();
        shop.OnShopClosed();
        healthBG.enabled = true;
        healthIcon.enabled = true;
        healthBar.enabled = true;
        StopCoroutine(waveDescCoroutine);
        ws.touchShiftUp = 35;
    }

    public void OnItemBought(object source, ItemDetails item){

        //If Item is not already owned
        if(items[item.id-1] == null){
            //Add the item to list
            items[item.id-1] = Instantiate(item);
            gameObjectIndex[item.id-1] = slotParent.childCount;

            //Instantiate Inventory slot
            GameObject slotInstance = Instantiate(slotPrefab, slotParent);
            slotInstance.GetComponent<Button>().onClick.AddListener(delegate {OnInventoryItemPressed(items[item.id-1]);});

            //GameObject Component Configuration
            slotInstance.transform.GetChild(0).GetComponent<Image>().sprite = item.image;
            slotInstance.transform.GetChild(0).GetComponent<Image>().preserveAspect = true;   
            slotInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.numberOfItems.ToString();

        }else{
            //Modify the item
            UpdateItem(item.id, item.numberOfItems);
        }

        /*        
        if(item.type == ItemDetails.ItemType.Shoot){
            if(hotbarItems[0] == null){
                selectedItem = items[item.id - 1];
                PutItemIntoHotbar(0, hotbarParent.GetChild(0).transform);
            }
            else if(hotbarItems[1] == null){
                selectedItem = items[item.id - 1];
                PutItemIntoHotbar(1, hotbarParent.GetChild(1).transform);
            }
        }
        else{
            if(hotbarItems[2] == null){
                selectedItem = items[item.id - 1];
                PutItemIntoHotbar(2, hotbarParent.GetChild(2).transform);
            }
            else if(hotbarItems[3] == null){
                selectedItem = items[item.id - 1];
                PutItemIntoHotbar(3, hotbarParent.GetChild(3).transform);
            }
        }
        */
    }

    public void UpdateItem(int id, int changeInNum){
        Debug.Log("Update id: " + id);
        GameObject go;
        items[id-1].numberOfItems += changeInNum;


        if(gameObjectIndex[id-1] != -1){
            go = slotParent.GetChild(gameObjectIndex[id-1]).gameObject;
            go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (items[id-1].numberOfItems).ToString();
        }
        else{
            int hbindex = -1;
            for(int i = 0; i < 4; i++){
                if(hotbarItems[i] != null){
                    if(hotbarItems[i].id == id){
                        hbindex = i;
                        break;
                    }
                }
            }
            go = hotbarParent.GetChild(hbindex).gameObject;
            go.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = (items[id-1].numberOfItems).ToString();

            if(items[id-1].numberOfItems <= 0){
                if(items[id-1].type == ItemDetails.ItemType.Shoot){
                    if(gameObjectIndex[28-1] != -1){//if normal bullet is in the inventory
                        selectedItem = items[28-1]; //select the normal bullet
                        PutItemIntoHotbar(hbindex, go.transform); //move normal bullet into to-be-deleted item's hotbar slot
                        DeleteItem(id, slotParent.GetChild(gameObjectIndex[id-1]).gameObject); //delete the "to-be-deleted" item
                    }
                    else {//if normal bullet is in the hotbar
                        int hbi = -1;
                        for(int i=0;i<4;i++){
                            if(hotbarItems[i].id == 28){
                                hbi = i;
                                break;
                            }
                        }
                        hotbarParent.GetChild(hbSelectedIndex).transform.GetChild(0).GetComponent<Image>().sprite = normalBullet.image;
                        hotbarParent.GetChild(hbSelectedIndex).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";

                        hotbarParent.GetChild(hbi).transform.GetChild(0).GetComponent<Image>().sprite = blankSprite;
                        
                        items[hotbarItems[hbSelectedIndex].id-1] = null;
                        Debug.Log("Nulled items index: " + (hotbarItems[hbSelectedIndex].id-1));
                        hotbarItems[hbSelectedIndex] = hotbarItems[hbi];
                        hotbarSelectedItem = hotbarItems[hbSelectedIndex];
                        hotbarItems[hbi] = null;
                        
                    }
                }
                else{
                    hotbarParent.GetChild(hbSelectedIndex).transform.GetChild(0).GetComponent<Image>().sprite = blankSprite;
                    hotbarParent.GetChild(hbSelectedIndex).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
                    items[hotbarItems[hbSelectedIndex].id-1] = null;
                    hotbarItems[hbSelectedIndex] = null;

                    OnHotBarItemPressed(0);
                }
                
            }
        }
    }

    public void DeleteItem(int id, GameObject go){
        Debug.Log("deleting id: " + id + " gameObjectIndex: " + gameObjectIndex[id -1]);
        items[id-1] = null; //Deletes the item
        if(gameObjectIndex[id-1] != -1){
            Debug.Log("remove inv");
            RemoveFromInventory(id, go);
        }
        else{
            go.transform.GetChild(0).GetComponent<Image>().color = new Color(1f,1f,1f,0f);
            go.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    public void OnOpenInventory(){
        isInventoryOpen = true;
        anim.SetBool("openInventory", true);
        selectedItem = null;
    }

    public void OnCloseInventory(){
        StartCoroutine("CloseInventory");
        anim.SetBool("openInventory", false);
        DeselectInventoryItem();
    }
    IEnumerator CloseInventory(){
        yield return new WaitForSeconds(0.1f);
        isInventoryOpen = false;
    }

    public void IsHotbarVisible(bool cond){
        hotbarSlotOne.enabled = cond;
        hotbarSlotOne.gameObject.GetComponent<Button>().enabled = cond;
        hotbarSlotOne.transform.GetChild(0).GetComponent<Image>().enabled = cond;
        hotbarSlotOne.transform.GetChild(1).GetComponent<Image>().enabled = cond;
        hotbarSlotOne.transform.GetChild(2).GetComponent<TextMeshProUGUI>().enabled = cond;

        hotbarSlotTwo.enabled = cond;
        hotbarSlotTwo.gameObject.GetComponent<Button>().enabled = cond;
        hotbarSlotTwo.transform.GetChild(0).GetComponent<Image>().enabled = cond;
        hotbarSlotTwo.transform.GetChild(1).GetComponent<Image>().enabled = cond;
        hotbarSlotTwo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().enabled = cond;

        hotbarSlotThree.enabled = cond;
        hotbarSlotThree.gameObject.GetComponent<Button>().enabled = cond;
        hotbarSlotThree.transform.GetChild(0).GetComponent<Image>().enabled = cond;
        hotbarSlotThree.transform.GetChild(1).GetComponent<Image>().enabled = cond;
        hotbarSlotThree.transform.GetChild(2).GetComponent<TextMeshProUGUI>().enabled = cond;

        hotbarSlotFour.enabled = cond;
        hotbarSlotFour.gameObject.GetComponent<Button>().enabled = cond;
        hotbarSlotFour.transform.GetChild(0).GetComponent<Image>().enabled = cond;
        hotbarSlotFour.transform.GetChild(1).GetComponent<Image>().enabled = cond;
        hotbarSlotFour.transform.GetChild(2).GetComponent<TextMeshProUGUI>().enabled = cond;
    }

    public void OnInventoryItemPressed(ItemDetails item){
        if(item.type == ItemDetails.ItemType.Shoot) OnWiggleShootHotbar();
        else OnWiggleBuildHotbar();
        DeselectInventoryItem();
        selectedItem = item;
        Debug.Log(item.id + "-x-" +gameObjectIndex[item.id-1]);
        slotParent.GetChild(gameObjectIndex[item.id-1]).GetComponent<Image>().sprite = selectedSlot;
        slotParent.GetChild(gameObjectIndex[item.id-1]).GetComponent<Image>().color = new Color(0f,0f,0f,1f);
        
    }

    public void DeselectInventoryItem(){
        if(selectedItem != null){
            slotParent.GetChild(gameObjectIndex[selectedItem.id-1]).GetComponent<Image>().sprite = null;
            slotParent.GetChild(gameObjectIndex[selectedItem.id-1]).GetComponent<Image>().color = defaultBGSlot;
        }
    }

    public void OnHotBarItemPressed(int index){
        if(selectedItem != null && isInventoryOpen == true){
            if(((index == 0 || index == 1) && selectedItem.type == ItemDetails.ItemType.Shoot) || ((index == 2 || index == 3) && selectedItem.type == ItemDetails.ItemType.Build) ){
                PutItemIntoHotbar(index, hotbarParent.GetChild(index).transform);

                DeselectInventoryItem();
            }
        }
        if(!isInventoryOpen){
            if(index != hbSelectedIndex & hotbarItems[index] != null){
                hotbarParent.GetChild(hbSelectedIndex).transform.GetChild(1).GetComponent<Image>().color = new Color(0f,0f,0f,0f);
                hotbarParent.GetChild(index).transform.GetChild(1).GetComponent<Image>().color = new Color(0f,0f,0f,1f);
                hbSelectedIndex = index;
                hotbarSelectedItem = hotbarItems[index];
                if(hotbarSelectedItem.type == ItemDetails.ItemType.Shoot) igui.ShootMode();
                else if(hotbarSelectedItem.id == 14) igui.BuildWall();
                else igui.BuildTurret();
            }
        }
    }
    
    //This function only removes the item from the inventory, but doesn't delete it
    public void RemoveFromInventory(int id, GameObject go){
        for(int i = 0; i < totalItems; i++){
            if(gameObjectIndex[i] > gameObjectIndex[id-1]){
                gameObjectIndex[i]--;
            }
        }
        Destroy(go);
        gameObjectIndex[id-1] = -1;
    }

    public void PutItemIntoHotbar(int hotbarIndex, Transform hotbarSlotTranform){

        DeselectInventoryItem();
        if(hotbarItems[hotbarIndex] == null){

            hotbarItems[hotbarIndex] = selectedItem; //ItemDetails object references
            if(hbSelectedIndex == hotbarIndex) hotbarSelectedItem = selectedItem;

            //Change the HB appearance
            hotbarSlotTranform.GetChild(0).GetComponent<Image>().sprite = selectedItem.image;
            hotbarSlotTranform.GetChild(0).GetComponent<Image>().color = new Color(1f,1f,1f,1f);
            hotbarSlotTranform.GetChild(2).GetComponent<TextMeshProUGUI>().text = selectedItem.numberOfItems.ToString();
            if(selectedItem.id == 28) hotbarSlotTranform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";

            RemoveFromInventory(selectedItem.id, slotParent.GetChild(gameObjectIndex[selectedItem.id-1]).gameObject); //Remove inv slot
            selectedItem = null;
        }
        else{
            GameObject invSlotObj = slotParent.GetChild(gameObjectIndex[selectedItem.id - 1]).gameObject;
            invSlotObj.GetComponent<Button>().onClick.RemoveAllListeners();
            Debug.Log("Added listener id: " + items[hotbarItems[hotbarIndex].id - 1].id);
            ItemDetails tempID =  items[hotbarItems[hotbarIndex].id - 1];
            invSlotObj.GetComponent<Button>().onClick.AddListener(delegate {OnInventoryItemPressed(tempID);});
            invSlotObj.transform.GetChild(0).GetComponent<Image>().sprite = hotbarItems[hotbarIndex].image;
            invSlotObj.transform.GetChild(0).GetComponent<Image>().preserveAspect = true;   
            invSlotObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = hotbarItems[hotbarIndex].numberOfItems.ToString();
            if(tempID.id == 28) invSlotObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";

            GameObject hbSlotObj = hotbarParent.GetChild(hotbarIndex).gameObject;
            hbSlotObj.transform.GetChild(0).GetComponent<Image>().sprite = selectedItem.image;
            hbSlotObj.transform.GetChild(0).GetComponent<Image>().color = new Color(1f,1f,1f,1f);
            hbSlotObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = (selectedItem.numberOfItems).ToString();
            if(selectedItem.id == 28) hbSlotObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";

            int temp = gameObjectIndex[hotbarItems[hotbarIndex].id - 1];
            gameObjectIndex[hotbarItems[hotbarIndex].id - 1] = gameObjectIndex[selectedItem.id - 1];
            gameObjectIndex[selectedItem.id - 1] = temp;
            
            hotbarItems[hotbarIndex] = selectedItem;
            selectedItem = null;
            if(hotbarIndex == hbSelectedIndex) hotbarSelectedItem = hotbarItems[hotbarIndex];
        }
        
    }

    //mode 0 -> shooting, mode 1 -> building
    bool wigglingShoot = false; bool wigglingBuild = false;
    Coroutine wiggleHBOne, wiggleHBTwo, wiggleHBThree, wiggleHBFour;
    public void OnWiggleShootHotbar(){
        OnStopWiggleBuildHotbar();
        if(!wigglingShoot){
            wiggleHBOne = StartCoroutine(WiggleImage(hotbarSlotOne, 5f, 1f, 0));
            wiggleHBTwo = StartCoroutine(WiggleImage(hotbarSlotTwo, 5f, 1f, 0));
        }
    }
    public void OnWiggleBuildHotbar(){
        OnStopWiggleShootHotbar();
        if(!wigglingBuild){
            wiggleHBThree = StartCoroutine(WiggleImage(hotbarSlotThree, 5f, 1f, 1));
            wiggleHBFour = StartCoroutine(WiggleImage(hotbarSlotFour, 5f, 1f, 1));
        }
    }
    public void OnStopWiggleShootHotbar(){
        if(wigglingShoot){
            Debug.Log("Stop shoot wiggle");
            StopCoroutine(wiggleHBOne);
            StopCoroutine(wiggleHBTwo);
            hotbarSlotOne.rectTransform.rotation = Quaternion.EulerAngles(0f,0f,0f);
            hotbarSlotTwo.rectTransform.rotation = Quaternion.EulerAngles(0f,0f,0f);
            wigglingShoot = false;
        }
    }
    public void OnStopWiggleBuildHotbar(){
        if(wigglingBuild){
            StopCoroutine(wiggleHBThree);
            StopCoroutine(wiggleHBFour);
            hotbarSlotThree.rectTransform.rotation = Quaternion.EulerAngles(0f,0f,0f);
            hotbarSlotFour.rectTransform.rotation = Quaternion.EulerAngles(0f,0f,0f);
            wigglingBuild = false;
        }
    }
    IEnumerator WiggleImage(Image img, float rotAngleRadius, float increment, int mode){
        if(mode == 0) wigglingShoot = true;
        else wigglingBuild = true;

        int moveDirection = 1; float rotZ;
        while(true){
            rotZ = (img.rectTransform.rotation.eulerAngles.z <= 180f) ? img.rectTransform.rotation.eulerAngles.z: -360f + img.rectTransform.rotation.eulerAngles.z;
            if(rotZ > rotAngleRadius) moveDirection = -1;
            if(rotZ < -rotAngleRadius) moveDirection = 1;

            if(moveDirection == 1){
                img.rectTransform.Rotate(new Vector3(0f, 0f, increment));
            }
            else if(moveDirection == -1){
                img.rectTransform.Rotate(new Vector3(0f, 0f, -increment));
            }
            yield return null;
        }

    }

    

    void Update()
    {   

    }
}
