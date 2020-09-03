using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{

    public Image bulletPanel, wallPanel, turretPanel;

    //public enum Playmode {Shoot, Wall, Turret};
    public enum Playmode {Shoot, Build};
    public Playmode playmode; 

    public enum BuildChoice {Wall, Turret};
    public BuildChoice buildchoice;

    public Color panelColor;

    IEnumerator coroutine;

    public void ShootMode(){
        playmode = Playmode.Shoot;
        Debug.Log("shooting now");
    }

    public void BuildWall(){
        //playmode = Playmode.Wall; 
        playmode = Playmode.Build;
        buildchoice = BuildChoice.Wall;
        Debug.Log("wall-ing now");
    }

    public void BuildTurret(){
        //playmode = Playmode.Turret;
        playmode = Playmode.Build;
        buildchoice = BuildChoice.Turret;
        Debug.Log("turret-ing now");
    }


    void Start()
    {
        playmode = Playmode.Shoot;
        panelColor = bulletPanel.color;
    }

    

    void AdjustUI(){
        if(playmode == Playmode.Shoot){
            bulletPanel.color = panelColor;
            wallPanel.color = panelColor + new Color(0f, 0f, 0f, -1 * panelColor.a);
            turretPanel.color = panelColor + new Color(0f, 0f, 0f, -1 * panelColor.a);
        } else{
            if(buildchoice == BuildChoice.Wall){
                bulletPanel.color = panelColor + new Color(0f, 0f, 0f, -1 * panelColor.a);;
                wallPanel.color = panelColor;
                turretPanel.color = panelColor + new Color(0f, 0f, 0f, -1 * panelColor.a);
            }
            if(buildchoice == BuildChoice.Turret){
                bulletPanel.color = panelColor + new Color(0f, 0f, 0f, -1 * panelColor.a);;
                wallPanel.color = panelColor + new Color(0f, 0f, 0f, -1 * panelColor.a);
                turretPanel.color = panelColor;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        AdjustUI();

        //Debug.Log(Input.GetMouseButton(0));
        //Debug.Log(playmode);
    }


}
