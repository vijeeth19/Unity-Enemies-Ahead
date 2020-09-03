using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITutorial : MonoBehaviour
{
    public enum State {NotStarted, ClickShop, ClickBulletMenu, ClickTurretMenu, ClickBrickwall, ClickBuy, CloseShop, ClickInventory, ClickInventoryItem, ClickHotbar, CloseInventory, ExplainHotbar, ClickWallFromHB, PlaceWall, ClickBullet, ClickNextWave};
    public State state;
    public bool startTut = false;
    
    public Animator waveAnimator;
    //public EnemySpawner es;
    public GameObject handImagePrefab;
    GameObject handImageInstance;
    public Sprite[] handSprites;

    float arrowShiftDown = 200f/2560f * Screen.height;
    void Start()
    {
        state = State.NotStarted;
        if(PlayerPrefs.GetInt("DoneTut", 0) == 0){
            BlockAllButtons();
            StartCoroutine(WaitTillHandAnim());
        }
        Debug.Log(arrowShiftDown + " is arrow shift down");
    }

    AnimatorStateInfo currInfo;
    IEnumerator WaitTillHandAnim(){
        currInfo = waveAnimator.GetCurrentAnimatorStateInfo(0);
        while(!currInfo.IsName("PostAnim")){
            currInfo = waveAnimator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }
        Debug.Log("Lets start animation!");
        handImageInstance = Instantiate(handImagePrefab, transform);
        coroutine = HandAnimation();
        StartCoroutine(coroutine);
    }
    IEnumerator HandAnimation(){
        int i = 0;
        Image img = handImageInstance.GetComponent<Image>();
        StartCoroutine(StopHandAnimation());
        while(true){
            img.sprite = handSprites[i%5];
            i++;
            yield return new WaitForSeconds(0.15f);
        }
        
    }
    IEnumerator StopHandAnimation(){
        while(!Input.GetMouseButton(0)){
            yield return null;
        }
        StopCoroutine(coroutine);
        coroutine = null;
        Destroy(handImageInstance);
    }

    public void StateMachine(){
        switch(state){
            case State.NotStarted:
                state = (startTut) ? State.ClickShop : State.NotStarted;
                break;
            case State.ClickShop:
                break;
            case State.ClickBulletMenu:
                break;
            case State.ClickTurretMenu:
                break;
            default: break;
        }
        switch(state){
            case State.NotStarted:
                break;
            case State.ClickShop:
                buttons[0].interactable = true;
                ToClickShop();
                break;
            case State.ClickBulletMenu:
                buttons[0].interactable = false;
                buttons[37].interactable = true;
                ToClickBulletMenu();
                break;
            case State.ClickTurretMenu:
                buttons[37].interactable = false;
                buttons[38].interactable = true;
                ToClickTurretMenu();
                break;
            case State.ClickBrickwall:
                buttons[38].interactable = false;
                buttons[7].interactable = true;
                ToClickBrickWall();
                break;
            case State.ClickBuy:
                buttons[7].interactable = false;
                ToClickBuy();
                break;
            case State.CloseShop:
                buttons[39].interactable = true;
                ToCloseShop();
                break;
            case State.ClickInventory:
                buttons[39].interactable = false;
                buttons[1].interactable = true;
                ToClickInventory();
                break;
            case State.ClickInventoryItem:
                buttons[1].interactable = false;
                ToClickInventoryItem();
                break;
            case State.ClickHotbar:
                buttons[5].interactable = true;
                ToClickHotbar();
                break;
            case State.CloseInventory:
                buttons[5].interactable = false;
                buttons[41].interactable = true;
                ToCloseInventory();
                break;
            case State.ExplainHotbar:
                buttons[41].interactable = false;
                OnExplainHotbar();
                break;
            case State.ClickWallFromHB:
                buttons[5].interactable = true;
                ToClickWallFromHB();
                break;
            case State.PlaceWall:
            buttons[5].interactable = false;
                ToPlaceWall();
                break;
            case State.ClickBullet:
                buttons[3].interactable = true;
                ToClickBullet();
                break;
            case State.ClickNextWave:
                buttons[3].interactable = false;
                ToClickNextWave();
                break;
            default: break;
        }
    }
    IEnumerator coroutine;
    public GameObject arrow, arrowInstance;
    public RectTransform shopSpawnTransform;
    public void ToClickShop(){
        if(coroutine == null && state == State.ClickShop){
            coroutine = ToClickShopEnumerator();
            StartCoroutine(coroutine);
        }
    }
    IEnumerator ToClickShopEnumerator(){
        arrowInstance = Instantiate(arrow, shopSpawnTransform.position - Vector3.up*arrowShiftDown, arrow.transform.rotation, transform);
        Vector3 origArrowPos = arrowInstance.transform.position;
        while(true){
            arrowInstance.transform.position = origArrowPos - Vector3.up * Mathf.PingPong(100*Time.time, 50f);
            yield return null;
        }
    }
    public GameObject tutPanel, tutPanelInstance;
    public void OnShopClicked(){
        if(state == State.ClickShop){
            StartCoroutine(OnShopClickedEnumerator());
        }
    }
    IEnumerator OnShopClickedEnumerator(){
        Destroy(arrowInstance);
        StopCoroutine(coroutine);
        
        yield return new WaitForSeconds(0.2f);
        tutPanelInstance = Instantiate(tutPanel, transform);
        tutPanelInstance.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Shop";
        tutPanelInstance.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "This is the shop. You can buy bullets and defenses using the money you make from killing enemies!\n\nThe top left of the screen shows how much money you have.";
        tutPanelInstance.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {state = State.ClickBulletMenu; Destroy(tutPanelInstance);coroutine = null;});
        yield return null;
    }
    public RectTransform bulletMenuTransform;
    public void ToClickBulletMenu(){
        if(coroutine == null && state == State.ClickBulletMenu){
            coroutine = ToClickBulletMenuEnumertor();
            StartCoroutine(coroutine);
        }
    }
    IEnumerator ToClickBulletMenuEnumertor(){
        yield return new WaitForSeconds(0.4f);
        arrowInstance = Instantiate(arrow, bulletMenuTransform.position - Vector3.up*arrowShiftDown, arrow.transform.rotation, transform);
        Vector3 origArrowPos = arrowInstance.transform.position;
        while(true){
            arrowInstance.transform.position = origArrowPos - Vector3.up * Mathf.PingPong(100*Time.time, 50f);
            yield return null;
        }
    }
    public void OnBulletMenuClicked(){
        if(state == State.ClickBulletMenu){
            StartCoroutine(OnBulletMenuClickedEnumerator());
        }
    }
    IEnumerator OnBulletMenuClickedEnumerator(){
        Destroy(arrowInstance);
        StopCoroutine(coroutine);
        yield return new WaitForSeconds(0.2f);
        tutPanelInstance = Instantiate(tutPanel, transform);
        tutPanelInstance.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Bullet Shop";
        tutPanelInstance.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Welcome to the bullet shop!\nYou can buy different bullets like Ice bullets, Fire bullets, Lightning bullets, and Explosive Bullets";
        tutPanelInstance.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {state = State.ClickTurretMenu; Destroy(tutPanelInstance);coroutine = null;});
        yield return null;
    }
    
    public RectTransform turretMenuTransform;
    public void ToClickTurretMenu(){
        if(coroutine == null && state == State.ClickTurretMenu){
            coroutine = ToClickTurretMenuEnumerator();
            StartCoroutine(coroutine);
        }
    }
    IEnumerator ToClickTurretMenuEnumerator(){
        yield return new WaitForSeconds(0.4f);
        arrowInstance = Instantiate(arrow, turretMenuTransform.position - Vector3.up*arrowShiftDown, arrow.transform.rotation, transform);
        Vector3 origArrowPos = arrowInstance.transform.position;
        while(true){
            arrowInstance.transform.position = origArrowPos - Vector3.up * Mathf.PingPong(100*Time.time, 50f);
            yield return null;
        }
    }
    public void OnTurretMenuClick(){
        if(state == State.ClickTurretMenu){
            StartCoroutine(OnTurretMenuClickEnumerator());
        }
    }
    IEnumerator OnTurretMenuClickEnumerator(){
        Destroy(arrowInstance);
        StopCoroutine(coroutine);
        yield return new WaitForSeconds(0.2f);
        tutPanelInstance = Instantiate(tutPanel, transform);
        tutPanelInstance.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Turret Shop";
        tutPanelInstance.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Welcome to the Turret shop!\nYou can buy walls and different turrets.\nLets start by first buying brick walls";
        tutPanelInstance.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {state = State.ClickBrickwall; Destroy(tutPanelInstance);coroutine = null;});
        yield return null;
    }

    public RectTransform brickWallTransform;
    public void ToClickBrickWall(){
        if(coroutine == null && state == State.ClickBrickwall){
            coroutine = ToClickBrickWallEnumerator();
            StartCoroutine(coroutine);
        }
    }
    IEnumerator ToClickBrickWallEnumerator(){
        yield return new WaitForSeconds(0.4f);
        arrowInstance = Instantiate(arrow, brickWallTransform.position - Vector3.up*arrowShiftDown, arrow.transform.rotation, transform);
        Vector3 origArrowPos = arrowInstance.transform.position;
        while(true){
            arrowInstance.transform.position = origArrowPos - Vector3.up * Mathf.PingPong(100*Time.time, 50f);
            yield return null;
        }
    }
    public void OnClickBrickWall(){
        if(state == State.ClickBrickwall){
            StartCoroutine(OnClickBrickWallEnumerator());
        }
    }
    IEnumerator OnClickBrickWallEnumerator(){
        Destroy(arrowInstance);
        StopCoroutine(coroutine);
        yield return new WaitForSeconds(0.2f);
        state = State.ClickBuy;
        coroutine = null;
    }

    public Transform buyButtonTransform;
    public void ToClickBuy(){
        if(coroutine == null && state == State.ClickBuy){
            coroutine = ToClickBuyEnumerator();
            StartCoroutine(coroutine);
        }
    }
    IEnumerator ToClickBuyEnumerator(){
        yield return new WaitForSeconds(0.4f);
        arrowInstance = Instantiate(arrow, buyButtonTransform.position - Vector3.up*arrowShiftDown, arrow.transform.rotation, transform);
        Vector3 origArrowPos = arrowInstance.transform.position;
        while(true){
            arrowInstance.transform.position = origArrowPos - Vector3.up * Mathf.PingPong(100*Time.time, 50f);
            yield return null;
        }
    }
    public void OnClickBuy(){
        if(state == State.ClickBuy){
            StartCoroutine(OnClickBuyEnumerator());
        }
    }
    IEnumerator OnClickBuyEnumerator(){
        Destroy(arrowInstance);
        StopCoroutine(coroutine);
        yield return new WaitForSeconds(0.2f);
        tutPanelInstance = Instantiate(tutPanel, transform);
        tutPanelInstance.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Item Bought!";
        tutPanelInstance.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "After buying an item from the shop, you can find it in your inventory!\n\nLets head over to the inventory now!";
        tutPanelInstance.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {state = State.CloseShop; Destroy(tutPanelInstance);coroutine = null;});
        yield return null;
    }

    public RectTransform shopCloseButtonTransform;
    public void ToCloseShop(){
        if(coroutine == null && state == State.CloseShop){
            coroutine = ToCloseShopEnumerator();
            StartCoroutine(coroutine);
        }
    }
    IEnumerator ToCloseShopEnumerator(){
        yield return new WaitForSeconds(0.2f);
        arrowInstance = Instantiate(arrow, shopCloseButtonTransform.position - Vector3.up*arrowShiftDown, arrow.transform.rotation, transform);
        Vector3 origArrowPos = arrowInstance.transform.position;
        while(true){
            arrowInstance.transform.position = origArrowPos - Vector3.up * Mathf.PingPong(100*Time.time, 50f);
            yield return null;
        }
    }
    public void OnCloseShop(){
        if(state == State.CloseShop){
            StartCoroutine(OnCloseShopEnumerator());
        }
    }
    IEnumerator OnCloseShopEnumerator(){
        Destroy(arrowInstance);
        StopCoroutine(coroutine);
        yield return new WaitForSeconds(0.2f);
        state = State.ClickInventory;
        coroutine = null;
    }

    public RectTransform inventoryTransform;
    public void ToClickInventory(){
        if(coroutine == null && state == State.ClickInventory){
            coroutine = ToClickInventoryEnumerator();
            StartCoroutine(coroutine);
        }
    }
    IEnumerator ToClickInventoryEnumerator(){
        yield return new WaitForSeconds(0.2f);
        arrowInstance = Instantiate(arrow, inventoryTransform.position - Vector3.up*arrowShiftDown, arrow.transform.rotation, transform);
        Vector3 origArrowPos = arrowInstance.transform.position;
        while(true){
            arrowInstance.transform.position = origArrowPos - Vector3.up * Mathf.PingPong(100*Time.time, 50f);
            yield return null;
        }
    }
    public void OnClickInventory(){
        if(state == State.ClickInventory){
            StartCoroutine(OnClickInventoryEnumerator());
        }
    }
    IEnumerator OnClickInventoryEnumerator(){
        Destroy(arrowInstance);
        StopCoroutine(coroutine);
        yield return new WaitForSeconds(0.4f);
        tutPanelInstance = Instantiate(tutPanel, transform);
        tutPanelInstance.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Inventory";
        tutPanelInstance.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "All the items you own can be seen here.";
        tutPanelInstance.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {state = State.ClickInventoryItem; Destroy(tutPanelInstance);coroutine = null;});
        yield return null;
    }

    public Transform inventoryItemParentTransform;
    public void ToClickInventoryItem(){
        if(coroutine == null && state == State.ClickInventoryItem){
            coroutine = ToClickInventoryItemEnumerator();
            StartCoroutine(coroutine);
        }
    }
    IEnumerator ToClickInventoryItemEnumerator(){
        yield return new WaitForSeconds(0.2f);
        arrowInstance = Instantiate(arrow, inventoryItemParentTransform.GetChild(0).transform.position - Vector3.up*arrowShiftDown, arrow.transform.rotation, transform);
        inventoryItemParentTransform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate {OnClickInventoryItem();});
        Vector3 origArrowPos = arrowInstance.transform.position;
        while(true){
            arrowInstance.transform.position = origArrowPos - Vector3.up * Mathf.PingPong(100*Time.time, 50f);
            yield return null;
        }
    }
    public void OnClickInventoryItem(){
        if(state == State.ClickInventoryItem){
            StartCoroutine(OnClickInventoryItemEnumerator());
        }
    }
    IEnumerator OnClickInventoryItemEnumerator(){
        Destroy(arrowInstance);
        StopCoroutine(coroutine);
        yield return new WaitForSeconds(0.1f);
        tutPanelInstance = Instantiate(tutPanel, transform);
        tutPanelInstance.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Item Selected";
        tutPanelInstance.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "You have selected the brick wall item from your inventory. Now you can place them into your hotbar!";
        tutPanelInstance.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {state = State.ClickHotbar; Destroy(tutPanelInstance);coroutine = null;});
        yield return null;
    }

    public RectTransform hotbarTransform;
    public void ToClickHotbar(){
        if(coroutine == null && state == State.ClickHotbar){
            coroutine = ToClickHotbarEnumerator();
            StartCoroutine(coroutine);
        }
    }
    IEnumerator ToClickHotbarEnumerator(){
        yield return new WaitForSeconds(0.2f);
        arrowInstance = Instantiate(arrow, hotbarTransform.position - Vector3.up*arrowShiftDown, arrow.transform.rotation, transform);
        Vector3 origArrowPos = arrowInstance.transform.position;
        while(true){
            arrowInstance.transform.position = origArrowPos - Vector3.up * Mathf.PingPong(100*Time.time, 50f);
            yield return null;
        }
    }
    public void OnClickHotbar(){
        if(state == State.ClickHotbar){
            StartCoroutine(OnClickHotbarEnumerator());
        }
    }
    IEnumerator OnClickHotbarEnumerator(){
        Destroy(arrowInstance);
        StopCoroutine(coroutine);
        yield return new WaitForSeconds(0.1f);
        tutPanelInstance = Instantiate(tutPanel, transform);
        tutPanelInstance.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Item Placed";
        tutPanelInstance.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "You have placed the brick wall item into your hotbar! Any items placed in the hotbar can be selected and used during the game.\nThe 2 red hotbar boxes are for bullets and the 2 blue boxes are for turrets and walls.";
        tutPanelInstance.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {state = State.CloseInventory; Destroy(tutPanelInstance);coroutine = null;});
        yield return null;
    }

    public RectTransform inventoryCloseTransform;
    public void ToCloseInventory(){
        if(coroutine == null && state == State.CloseInventory){
            coroutine = ToCloseInventoryEnumerator();
            StartCoroutine(coroutine);
        }
    }
    IEnumerator ToCloseInventoryEnumerator(){
        yield return new WaitForSeconds(0.2f);
        arrowInstance = Instantiate(arrow, inventoryCloseTransform.position - Vector3.up*arrowShiftDown, arrow.transform.rotation, transform);
        Vector3 origArrowPos = arrowInstance.transform.position;
        while(true){
            arrowInstance.transform.position = origArrowPos - Vector3.up * Mathf.PingPong(100*Time.time, 50f);
            yield return null;
        }
    }
    public void OnCloseInventory(){
        if(state == State.CloseInventory){
            StartCoroutine(OnCloseInventoryEnumerator());
        }
    }
    IEnumerator OnCloseInventoryEnumerator(){
        Destroy(arrowInstance);
        StopCoroutine(coroutine);
        yield return new WaitForSeconds(0.2f);
        state = State.ExplainHotbar;
        coroutine = null;
    }
    
    public void OnExplainHotbar(){
        if(coroutine == null && state == State.ExplainHotbar){
            coroutine = OnExplainHotbarEnumerator();
            StartCoroutine(coroutine);
        }
    }
    IEnumerator OnExplainHotbarEnumerator(){
        yield return new WaitForSeconds(0.2f);
        tutPanelInstance = Instantiate(tutPanel, transform);
        tutPanelInstance.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Hotbar";
        tutPanelInstance.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "The four boxes on top is your hotbar. On clicking one of the boxes, you can equip the item it contains.\nLets equip the brick wall! ";
        tutPanelInstance.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {state = State.ClickWallFromHB; Destroy(tutPanelInstance);coroutine = null;});
    }

    public void ToClickWallFromHB(){
        if(coroutine == null && state == State.ClickWallFromHB){
            coroutine = ToClickWallFromHBEnumerator();
            StartCoroutine(coroutine);
        }
    }
    IEnumerator ToClickWallFromHBEnumerator(){
        yield return new WaitForSeconds(0.2f);
        arrowInstance = Instantiate(arrow, hotbarTransform.position - Vector3.up*arrowShiftDown, arrow.transform.rotation, transform);
        Vector3 origArrowPos = arrowInstance.transform.position;
        while(true){
            arrowInstance.transform.position = origArrowPos - Vector3.up * Mathf.PingPong(100*Time.time, 50f);
            yield return null;
        }
    }
    public void OnClickWallFromHB(){
        if(state == State.ClickWallFromHB){
            StartCoroutine(OnClickWallFromHBEnumerator());
        }
    }
    IEnumerator OnClickWallFromHBEnumerator(){
        Destroy(arrowInstance);
        StopCoroutine(coroutine);
        yield return new WaitForSeconds(0.1f);
        tutPanelInstance = Instantiate(tutPanel, transform);
        tutPanelInstance.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Wall Equiped";
        tutPanelInstance.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "You have equiped the brickwall!\nLets use them as defense for the next wave of enemies!";
        tutPanelInstance.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {state = State.PlaceWall; Destroy(tutPanelInstance);coroutine = null;});
        yield return null;
    }

    public void ToPlaceWall(){
        if(coroutine == null && state == State.PlaceWall){
            coroutine = ToPlaceWallEnumerator();
            StartCoroutine(coroutine);
            Debug.Log("Started Coroutine!");
        }
    }
    IEnumerator ToPlaceWallEnumerator(){
        yield return new WaitForSeconds(0.2f);
        handImageInstance = Instantiate(handImagePrefab, transform);
        handImageInstance.GetComponent<Image>().sprite = handSprites[2];
        Vector3 origArrowPos = handImageInstance.transform.position;
        while(true){
            handImageInstance.transform.position = origArrowPos + Vector3.right * (Mathf.PingPong(150*Time.time, 400f) - 200f);
            if(Input.GetMouseButton(0)) break;
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);
        handImageInstance.GetComponent<Image>().sprite = handSprites[3];
        yield return new WaitForSeconds(0.15f);
        handImageInstance.GetComponent<Image>().sprite = handSprites[4];
        yield return new WaitForSeconds(0.2f);
        Destroy(handImageInstance);
        while(true){
            if(Input.GetMouseButtonUp(0)) break;
            yield return null;
        }
        yield return new WaitForSeconds(0.45f);
        tutPanelInstance = Instantiate(tutPanel, transform);
        tutPanelInstance.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Wall Placed";
        tutPanelInstance.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "You can place walls and turrets anywhere in the map except for places marked red that surround already placed walls.\n\nWalls despawn after their lifetime indicated by the life bar on top of them!";
        tutPanelInstance.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {state = State.ClickBullet; Destroy(tutPanelInstance);coroutine = null;});
    }
    public RectTransform bulletHotbarTransform;
    public void ToClickBullet(){
        if(coroutine == null && state == State.ClickBullet){
            coroutine = ToClickBulletEnumerator();
            StartCoroutine(coroutine);
            Debug.Log("Started Coroutine!");
        }
    }
    IEnumerator ToClickBulletEnumerator(){
        yield return new WaitForSeconds(0.2f);
        arrowInstance = Instantiate(arrow, bulletHotbarTransform.position - Vector3.up*arrowShiftDown, arrow.transform.rotation, transform);
        Vector3 origArrowPos = arrowInstance.transform.position;
        while(true){
            arrowInstance.transform.position = origArrowPos - Vector3.up * Mathf.PingPong(100*Time.time, 50f);
            yield return null;
        }
    }
    public void OnClickBullet(){
        if(state == State.ClickBullet){
            StartCoroutine(OnClickBulletEnumerator());
        }
    }
    public Button nextWave;
    IEnumerator OnClickBulletEnumerator(){
        Destroy(arrowInstance);
        StopCoroutine(coroutine);
        yield return new WaitForSeconds(0.2f);
        tutPanelInstance = Instantiate(tutPanel, transform);
        tutPanelInstance.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Equiped Bullet";
        tutPanelInstance.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "You have equiped the default bullet! With this equiped, you can shoot your enemies.\nLets move to the next wave!";
        tutPanelInstance.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {state = State.ClickNextWave; Destroy(tutPanelInstance);coroutine = null;});
        nextWave.enabled = true;
        nextWave.transform.GetComponent<Image>().enabled = true;
        nextWave.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = true;
        yield return null;
    }
    
    public RectTransform nextWaveTransform;
    public void ToClickNextWave(){
        if(coroutine == null && state == State.ClickNextWave){
            coroutine = ToClickNextWaveEnumerator();
            StartCoroutine(coroutine);
            Debug.Log("Started Coroutine!");
        }
    }
    IEnumerator ToClickNextWaveEnumerator(){
        yield return new WaitForSeconds(0.2f);
        arrowInstance = Instantiate(arrow, nextWaveTransform.position - Vector3.up*arrowShiftDown, arrow.transform.rotation, transform);
        Vector3 origArrowPos = arrowInstance.transform.position;
        while(true){
            arrowInstance.transform.position = origArrowPos - Vector3.up * Mathf.PingPong(100*Time.time, 50f);
            yield return null;
        }
    }
    public void OnClickNextWave(){
        if(state == State.ClickNextWave){
            StartCoroutine(OnClickNextWaveEnumerator());
        }
    }
    IEnumerator OnClickNextWaveEnumerator(){
        Destroy(arrowInstance);
        StopCoroutine(coroutine);
        yield return new WaitForSeconds(0.2f);
        state = State.NotStarted;
        startTut = false;
        coroutine = null;
        PlayerPrefs.SetInt("DoneTut", 1);
        UnblockAllButtons();
    }
    public Button[] buttons;
    public void BlockAllButtons(){
        for(int i = 0; i < buttons.Length; i++){
            buttons[i].interactable = false;
        }
    }
    public void UnblockAllButtons(){
        for(int i = 0; i < buttons.Length; i++){
            buttons[i].interactable = true;
        }
    }

    void Update()
    {
        StateMachine();
    }
}
