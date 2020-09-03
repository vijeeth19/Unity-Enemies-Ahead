using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraAnimation : MonoBehaviour
{

    Vector3 sceneStartPos = new Vector3(0.9613357f, 88.80704f, 113.4292f);
    Vector3 sceneStartRot;
    Vector3 fortViewPos = new Vector3(1.039321f, 24.58054f, 30.54834f);
    Vector3 fortViewRot = new Vector3(21.383f, -178.624f, 0f);
    Vector3 gameViewPos = new Vector3(0f, 20.26f, -12.02f);
    Vector3 gameViewRot = new Vector3(25f,0f,0f);
    Vector3 focusPoint = new Vector3(0f, 2.73f, 0.11f);

    public enum CAnimation {startingZoom, rotation};
    CAnimation animation;

    float startTime;
    float maxDistStart, maxDistRotation;
    float startAngle;
    public float maxHealth, maxSpeed, maxDamage;


    bool finishedFortLerp;
    bool startedGameLerp;
    bool finishedGameLerp;
    bool playAnimation, playedAnimation;
    
    public Transform betweenWavesTransform;
    public WaveAnimation wi;

    IEnumerator MoveToTop;
    IEnumerator MoveBack;

    public Animator shopAnimator;
    public ShopUI shop;
    public Inventory inv;
    public GameObject introPanelPrefab;
    public Transform introPanelParent;
    public bool introduction = false;
    bool initialOverviewDone = false;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        sceneStartPos = transform.position - 3f*(fortViewPos - transform.position).normalized;
        //sceneStartRot = transform.
        maxDistStart = (transform.position - fortViewPos).magnitude;
        maxDistRotation = (fortViewPos - gameViewPos).magnitude;

        animation = CAnimation.startingZoom;

        finishedFortLerp = false;
        startedGameLerp = false;
        finishedGameLerp = false;
        playAnimation = false;
        playedAnimation = false;

        for(int i = 0; i < 11; i++){
            //Debug.Log(exponentialCurve(i*0.1f));
        }

        if(PlayerPrefs.GetInt("InitTut",0) == 0){
            initialOverviewDone = false;
            StartCoroutine(InitialOverview());
        }
        else{
            initialOverviewDone = true;
        }
        
    }
    public GameObject tutPanelPrefab, tutPanelInstance;
    public Transform tutPanelParent;
    IEnumerator InitialOverview(){
        yield return new WaitForSeconds(2f);
        tutPanelInstance = Instantiate(tutPanelPrefab, tutPanelParent);
        tutPanelInstance.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Defend your fort";
        tutPanelInstance.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Prevent enemies from entering the fort!\n\nYour health bar is in the bottom of the screen.";
        tutPanelInstance.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {Destroy(tutPanelInstance); initialOverviewDone = true; PlayerPrefs.SetInt("InitTut",1);});
    }

    float SCurve(float x){
        return (1f/(1f + Mathf.Pow((x/(1f-x)),-3f)));
    }

    float exponentialCurve(float x){
        return ((Mathf.Pow(9f, x) - 1f)/(8f));
    }

    float SInterpolate(){
        float percentDistance = 0;
        if(animation == CAnimation.startingZoom){
            percentDistance = ((transform.position - sceneStartPos).magnitude)/maxDistStart;
            return (SCurve(percentDistance) + 0.2f);
        }
        else if(animation == CAnimation.rotation){
            percentDistance = ((transform.position - fortViewPos).magnitude)/maxDistRotation;
            return (SCurve(percentDistance));
        }

        return 0f;

        
    }

    public void ViewChangeBetweenWaves(){
        Debug.Log("Changing view");
        MoveToTop = ChangeView(0);
        MoveBack = ChangeView(1);
        StopCoroutine(MoveBack);
        StartCoroutine(MoveToTop);
    }
    IEnumerator ChangeView(int view){
        if(view == 0){
            shopAnimator.SetInteger("Menu", 1);
            while((betweenWavesTransform.position - transform.position).magnitude > 0.5f){
                transform.position = Vector3.Lerp(transform.position, betweenWavesTransform.position, 1*Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, betweenWavesTransform.rotation, 1*Time.deltaTime);
                yield return null;
            }
            
            
            /*
            shop.OnShopOpenned();
            inv.IsHotbarVisible(false);
            inv.OnCloseInventory();
            */
        }
        else{
            Debug.Log("Go to previous pos");
            while((gameViewPos - transform.position).magnitude > 0.5f){
                transform.position = Vector3.Lerp(transform.position, gameViewPos, 2*Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(gameViewRot), 2*Time.deltaTime);
                yield return null;
            }
            shopAnimator.SetInteger("Menu", 0);
            wi.OnBetweenWaves();
        }
        
    }

    public void GoBackToDefaultView(){
        StartCoroutine(MoveBack);
        StopCoroutine(MoveToTop);
    }

    public void IntroduceCharacter(Transform enemyTransform, string enemyType, float health, float speed, float damage){
        Debug.Log("Introduce character");
        Vector3 enemyFrontPosition = enemyTransform.position - Vector3.forward * 15f + Vector3.up * 3f;
        if(enemyType == "Dodger") enemyFrontPosition += Vector3.up * 1.5f;
        if(enemyType == "Tank") enemyFrontPosition += Vector3.up * 3;

        
        StartCoroutine(GoToEnemyView(enemyFrontPosition, enemyType, health, speed, damage));
        
        introduction = true;
    }
    IEnumerator GoToEnemyView(Vector3 position, string enemyType, float health, float speed, float damage){
        Time.timeScale = 0.05f;
        yield return new WaitForSeconds(0.0015f);
        while((position - transform.position).magnitude > 1f){
            //Debug.Log("Time scale" + Time.timeScale);
            transform.position = Vector3.Lerp(transform.position, position, 20*Time.deltaTime);
            transform.rotation = Quaternion.Euler(Mathf.Lerp(transform.rotation.eulerAngles.x, 5f, 5*Time.deltaTime), transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            yield return null;
        }
        yield return new WaitForSeconds(0.01f);
        Time.timeScale = 0.0f;
        CreateIntroUI(enemyType, health, speed, damage);
    }
    GameObject introPanelInstance;
    public void CreateIntroUI(string enemyType, float health, float speed, float damage){
        introPanelInstance = Instantiate(introPanelPrefab, introPanelParent);
        introPanelInstance.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate {StopIntroduction();});
        string name = "", abilityDesc = "";
        switch(enemyType){
            case "Basic": name = "Chuck"; abilityDesc = "No ability. This not-so-bright enemy will stop before walls and turrets"; break;
            case "Sprinter": name = "Dash"; abilityDesc = "This rapid enemy is the fastest of all enemies. He can be blocked by walls and turrets though"; break;
            case "Jumper": name = "Shinobi"; abilityDesc = "This jumpy enemy will jump above all the obstacles in front of him. Walls and turret wont get in this guy's way!"; break;
            case "Dodger": name = "Anubis"; abilityDesc = "This trident wielding floating noble has the ability of NAVIGATION. He will weave through all the BRICK WALLS and TURRETS"; break;
            case "Tank": name = "Buffallo"; abilityDesc = "This tanky giant will destroy any BRICK WALL using his hammer. He can be blocked by any TURRET you place though!"; break;
            default: break;
        }
        introPanelInstance.transform.GetChild(6).GetComponent<TextMeshProUGUI>().fontSize = (enemyType == "Dodger") ? 10f: 11f;

        introPanelInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        introPanelInstance.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = abilityDesc;
        introPanelInstance.transform.GetChild(10).GetComponent<TextMeshProUGUI>().text = health.ToString();
        introPanelInstance.transform.GetChild(11).GetComponent<TextMeshProUGUI>().text = speed.ToString();
        introPanelInstance.transform.GetChild(12).GetComponent<TextMeshProUGUI>().text = damage.ToString();
        introPanelInstance.transform.GetChild(7).GetComponent<Image>().fillAmount = health/maxHealth;
        introPanelInstance.transform.GetChild(8).GetComponent<Image>().fillAmount = speed/maxSpeed;
        introPanelInstance.transform.GetChild(9).GetComponent<Image>().fillAmount = damage/maxDamage;
    }

    public void StopIntroduction(){
        Time.timeScale = 0.05f;
        Destroy(introPanelInstance);
        StartCoroutine(GoToGameView());
        introduction = false;
    }
    IEnumerator GoToGameView(){
        while((gameViewPos - transform.position).magnitude > 0.5f){
            transform.position = Vector3.Lerp(transform.position, gameViewPos, 40*Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(gameViewRot), 40*Time.deltaTime);
            yield return null;
        }
        Time.timeScale = 1f;
    }
    public AudioSource music;
    public bool musicPlayed = false;
    void Update()
    {
        if(Time.time - startTime > 1.5f && !finishedFortLerp && initialOverviewDone){
            transform.position = Vector3.Lerp(transform.position, fortViewPos, 2f*SInterpolate()*Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(fortViewRot),  2f*SInterpolate()*Time.deltaTime);
        }

        if((transform.position - fortViewPos).magnitude < 5f){
            finishedFortLerp = true;
            startedGameLerp = true;
            animation = CAnimation.rotation;
            
        }

        if(startedGameLerp){
            
            if(!finishedGameLerp){
                Vector3 slerpCenter = new Vector3(13.6f, 12.73f, -6.88f);
                transform.position = slerpCenter + Vector3.Slerp((transform.position - slerpCenter), (gameViewPos-slerpCenter), Time.deltaTime);

                Vector3 directionToTarget = (focusPoint - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation,  4f *Time.deltaTime);
            }
            

            if((transform.position - gameViewPos).magnitude < 5f & !finishedGameLerp){
                focusPoint = Vector3.Lerp(focusPoint, focusPoint+Vector3.forward, 10f*Time.deltaTime);
                
                if(transform.rotation.eulerAngles.x < 25f)
                    finishedGameLerp = true;
            }

        }

        if(finishedGameLerp){
            FindObjectOfType<EnemySpawner>().startNextWave = true;
            if(!musicPlayed){ music.Play(0); musicPlayed = true;}
            playAnimation = true;
        }
        if(playAnimation & !playedAnimation){
            FindObjectOfType<WaveAnimation>().OnBetweenWaves();
            playedAnimation = true;
        }

        //Debug.Log(finishedGameLerp);
        
    }
}
