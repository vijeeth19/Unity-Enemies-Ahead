using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{

    //Turret Icon Images
    public Image wallIcon, normalTurretIcon, poisonTurretIcon, ice1TurretIcon, ice2TurretIcon, ice3TurretIcon, fire1TurretIcon, fire2TurretIcon, fire3TurretIcon, elec1TurretIcon, elec2TurretIcon, elec3TurretIcon, exp1TurretIcon, exp2TurretIcon;

    //Bullet Icon Images
    public Image poisonBulletIcon, ice1BulletIcon, ice2BulletIcon, ice3BulletIcon, fire1BulletIcon, fire2BulletIcon, fire3BulletIcon, elec1BulletIcon, elec2BulletIcon, elec3BulletIcon, exp1BulletIcon, exp2BulletIcon, exp3BulletIcon;

    //Inventory slot images
    public Image normalTurretSlot, poisonTurretSlot, exp3BulletSlot;

    //Bullet and turret icons and panels
    public Image bulletPanel, bulletIcon, turretPanel, turretIcon;

    //DetailTab
    public TextMeshProUGUI name, desc, cost, numberToBuy, buyButtonPrice;
    public Image displayPic, buyButtonImage, buyButtonCoin;
    public Button buyButton;

    public Animator anim, itemDetailAnim;
    public CoinManager cm;
    public Inventory inv;
    public bool isShopOpen = false;
    public Color defaultBuyButtonColor, onPanelColor, onImageColor, offPanelColor, offImageColor;
    

    bool openShop = false, openItemDetail = false;
    public ItemDetails currentItem;
    
    void Start()
    {
        displayPic.preserveAspect = true;   
    }

    void SetBulletIcons(bool enabled){
        poisonBulletIcon.enabled = enabled; ice1BulletIcon.enabled = enabled; ice2BulletIcon.enabled = enabled; ice3BulletIcon.enabled = enabled; fire1BulletIcon.enabled = enabled; fire2BulletIcon.enabled = enabled; fire3BulletIcon.enabled = enabled; elec1BulletIcon.enabled = enabled; elec2BulletIcon.enabled = enabled; elec3BulletIcon.enabled = enabled; exp1BulletIcon.enabled = enabled; exp2BulletIcon.enabled = enabled; exp3BulletIcon.enabled = enabled;
    }

    void SetTurretIcons(bool enabled){
        wallIcon.enabled = enabled; normalTurretIcon.enabled = enabled; poisonTurretIcon.enabled = enabled; ice1TurretIcon.enabled = enabled; ice2TurretIcon.enabled = enabled; ice3TurretIcon.enabled = enabled; fire1TurretIcon.enabled = enabled; fire2TurretIcon.enabled = enabled; fire3TurretIcon.enabled = enabled; elec1TurretIcon.enabled = enabled; elec2TurretIcon.enabled = enabled; elec3TurretIcon.enabled = enabled; exp1TurretIcon.enabled = enabled; exp2TurretIcon.enabled = enabled;
    }

    public void OnShopOpenned(){
        isShopOpen = true;
        openShop = true;
        if(anim.GetInteger("Menu") == 0){
            SetBulletIcons(true);
            SetTurretIcons(false);
            Debug.Log("Shop openned");
            normalTurretSlot.enabled = false;
            poisonTurretSlot.enabled = false;
            exp3BulletSlot.enabled = true;   
        }
        else{
            SetBulletIcons(false);
            SetTurretIcons(true);
            Debug.Log("Shop openned");
            normalTurretSlot.enabled = true;
            poisonTurretSlot.enabled = true;
            exp3BulletSlot.enabled = false;   
        }
    }

    public void OnItemClicked(ItemDetails item){
        currentItem = item;
        openItemDetail = true;
        name.text = item.name;
        desc.text = item.power;
        cost.text = item.cost.ToString();
        displayPic.sprite = item.image;
        numberToBuy.text = "x " + item.numberOfItems.ToString();
        if(item.cost > cm.almuns){
            buyButtonImage.color = new Color(0.6f,0.6f,0.6f,1f);
        }
        else{
            buyButtonImage.color = defaultBuyButtonColor;
        }
    }

    public void OnBuyClicked(){
        if(currentItem.cost <= cm.almuns){
            OnItemBought(currentItem);
            cm.DebitAlmuns(currentItem.cost);
            OnItemDetailClosed();
        }
        else{
            StartCoroutine("ChangeText");
        }
        
    }
    IEnumerator ChangeText(){
        cost.text = "Not Enough Money";
        buyButtonCoin.enabled = false;
        yield return new WaitForSeconds(1f);
        cost.text = currentItem.cost.ToString();
        buyButtonCoin.enabled = true;
    }

    public delegate void ItemBoughtEventHandler(object source, ItemDetails args);
    public event ItemBoughtEventHandler ItemBought;

    protected virtual void OnItemBought(ItemDetails item){
        if(ItemBought != null)
            ItemBought(this, item);
    }

    public void OnItemDetailClosed(){
        openItemDetail = false;
    }

    public void OnShopClosed(){
        isShopOpen = false;
        openItemDetail = false;
        Debug.Log("Shop closed");
        anim.enabled = true;
        openShop = false;
        /*
        bulletPanel.color = new Color(1f,1f,1f,0f);
        bulletIcon.color = new Color(0f,0f,0f,0f);
        turretPanel.color = new Color(1f,1f,1f,0f);
        turretIcon.color = new Color(0f,0f,0f,0f);
        */
        
        StartCoroutine("MakeHotBarVisible");
    }
    IEnumerator MakeHotBarVisible(){
        yield return new WaitForSeconds(0.2f);
        inv.IsHotbarVisible(true);
    }

    public void OnBulletMenuOpenned(){
        //anim.enabled = false;
        anim.SetInteger("Menu", 0);
        bulletPanel.color = onPanelColor;
        bulletIcon.color = onImageColor;
        turretPanel.color = offPanelColor;
        turretIcon.color = offImageColor;

        SetBulletIcons(true);
        SetTurretIcons(false);

        normalTurretSlot.enabled = false;
        poisonTurretSlot.enabled = false;
        exp3BulletSlot.enabled = true;

        anim.enabled = false;
        anim.Play("Close Shop");
    }

    public void OnTurretMenuOpenned(){
        //anim.enabled = false;
        anim.SetInteger("Menu", 1);
        bulletPanel.color = offPanelColor;
        bulletIcon.color = offImageColor;
        turretPanel.color = onPanelColor;
        turretIcon.color = onImageColor;

        SetBulletIcons(false);
        SetTurretIcons(true);

        normalTurretSlot.enabled = true;
        poisonTurretSlot.enabled = true;
        exp3BulletSlot.enabled = false;

        anim.enabled = false;
        anim.Play("Close Shop 2");
    }

    void Update()
    {
        anim.SetBool("openShop", openShop);
        itemDetailAnim.SetBool("openItemDetail", openItemDetail);
    }
}
