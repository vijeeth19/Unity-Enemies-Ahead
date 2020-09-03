using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject explosiveBulletPrefab;
    public GameObject fireBulletPrefab;
    public GameObject electricBulletPrefab;
    public GameObject iceBulletPrefab;
    public GameObject poisonBulletPrefab;

    public GameObject[] bulletArray;
    
    GameObject instantiationPrefab;
    public Transform player;
    float bulletForceMagn = 200000f;
    public Vector3 bulletForce;
    public Camera camera;
    public LayerMask mask;
    IngameUI ingameUIScript;
    public Inventory inv;
    public ShopUI shop;
    public EnemySpawner es;
    public PlayerMovement pm;
    public CameraAnimation ca;

    int bulletChoice = 0; //0 -> normal bullet, 1 -> explosive bullet, 2 -> fire bullet, 3 -> electric bullet, 4 -> ice bullet, 5-> poison bullet

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(FindObjectOfType<TrajectoryScript>().line.enabled);
        ingameUIScript = FindObjectOfType<IngameUI>();
        
    }

    void shootBullet(){
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        
        if(Physics.Raycast(ray, out hit, 100f, mask)){

            if(Input.GetMouseButton(0) && FindObjectOfType<IngameUI>().playmode == IngameUI.Playmode.Shoot){
                FindObjectOfType<TrajectoryScript>().line.enabled = true;
                FindObjectOfType<TrajectoryScript>().endPoint = hit.point;
            }
            else{
                FindObjectOfType<TrajectoryScript>().line.enabled = false;
            }

            if(Input.GetMouseButtonUp(0) && FindObjectOfType<IngameUI>().playmode == IngameUI.Playmode.Shoot){
                FindObjectOfType<TrajectoryScript>().line.enabled = false;
                GameObject bulletInstance;
                //bulletInstance = Instantiate(instantiationPrefab, player.position + new Vector3(1.5f,0,0), player.rotation);

                bulletInstance = ObjectPooler.SharedInstance.GetPooledObject(inv.hotbarSelectedItem.name); 
                if (bulletInstance != null) {
                    bulletInstance.transform.position = player.position + new Vector3(1.5f,0,0);
                    bulletInstance.transform.rotation = player.rotation;
                    bulletInstance.SetActive(true);
                }

                Vector3 tangentialDirection = FindObjectOfType<TrajectoryScript>().tangentialDirection;
                Vector3 tangentialVel = FindObjectOfType<TrajectoryScript>().initVelocityX * tangentialDirection;
                Vector3 normalVel = FindObjectOfType<TrajectoryScript>().initVelocityY * Vector3.up;

                bulletInstance.GetComponent<Rigidbody>().AddForce(tangentialVel + normalVel, ForceMode.VelocityChange);
                if(inv.hotbarSelectedItem.id != 28 && inv.hotbarSelectedItem.type == ItemDetails.ItemType.Shoot) inv.UpdateItem(inv.hotbarSelectedItem.id, -1);
            }
        }
        
    }

    public void UpdateInstantiationPrefab(){

        if(inv.hotbarSelectedItem.type == ItemDetails.ItemType.Shoot){
            if(inv.hotbarSelectedItem.id == 28) instantiationPrefab = bulletArray[13];
            else instantiationPrefab = bulletArray[inv.hotbarSelectedItem.id-1];
        }

    }

    public bool IsMouseOverUIWithIgnores(){
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        for(int i = 0; i < raycastResultList.Count; i++){
            if(raycastResultList[i].gameObject.GetComponent<MouseUIClickThrough>() != null || raycastResultList[i].gameObject.tag == "Click Through"){
                raycastResultList.RemoveAt(i--);
            }
        }
        
        return raycastResultList.Count > 0;
    }



    // Update is called once per frame
    void Update()
    {
        UpdateInstantiationPrefab();
        if(!inv.isInventoryOpen & !shop.isShopOpen & !es.openNextWave & !IsMouseOverUIWithIgnores() & !pm.playerDead & !ca.introduction) shootBullet();
    }
}
