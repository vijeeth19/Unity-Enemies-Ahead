using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;


public class EnemySpawner : MonoBehaviour
{
    public BasicEnemy basicEnemyPrefab;
    public DodgerEnemy dodgerEnemyPrefab;
    public SprinterEnemy sprinterEnemyPrefab;
    public JumperEnemy jumperEnemyPrefab;
    public TankEnemy tankEnemyPrefab;

    public Wave currentWave;

    public EnemyWaveSpawn sprinterSpawner;
    public EnemyWaveSpawn jumperSpawner;
    public EnemyWaveSpawn dodgerSpawner;
    public EnemyWaveSpawn tankSpawner;
    public EnemyWaveSpawn basicSpawner;


    
    public struct spawnPosition{
        public float spawnPointX;
        public float spawnPointY;
        public float spawnPointZ;
    }

    
    float spawnTime;
    float startTime;
    int spawnCount = 0;


    public List<string> introductionOrder;

    float basicStartTime = 2f; float basicTimeInterval = 4f;
    float sprinterStartTime = 5f; float sprinterTimeInterval = 5f;
    float jumperStartTime = 10f; float jumperTimeInterval = 3f;
    float dodgerStartTime = 15f; float dodgerTimeInterval = 7f;
    float tankStartTime = 20f; float tankTimeInterval = 2f;

    public event Action BetweenWavesEvent;

    public TextMeshProUGUI waveNumberText;

    public Inventory inv;
    public WaveAnimation wi;
    public CameraAnimation ca;
    public GameObject nextWaveButton, nextWaveText;
    public Manager manager;
    public PlayerMovement pm;
    public Animator waveAnimator;

    

    //[System.Serializable]
    public class Wave{
        public int numberOfEnemies;
        public static int waveNumber;
        public static Dictionary<string, float> enemyProportions = new Dictionary<string, float>();

        public Wave (int count){
            waveNumber = 1;
            numberOfEnemies = count;
            if(enemyProportions.Count == 0){
                enemyProportions.Add("Basic", 0.1f);
                enemyProportions.Add("Sprinter", 0.2f);
                enemyProportions.Add("Jumper", 0.3f);
                enemyProportions.Add("Dodger", 0.2f); //0.2 before
                enemyProportions.Add("Tank", 0.2f);
            }
            
        }
    }

    public class EnemyWaveSpawn{
        public float[] spawnTimes;
        public int totalEnemiesThisWave;
        public int specificEnemyCount;
        float spawnStartTime;
        public float spawnTimeInterval;
        public spawnPosition[] spawnPoints;
        public Vector3[] spawnPositions;
        public string enemyType;
        public bool[] spawned;
        public int enemiesDied;
        

        public EnemyWaveSpawn(string type, float startTime, float timeInterval, int count){
            enemiesDied = 0;
            enemyType = type;
            totalEnemiesThisWave = count;
            spawnStartTime = startTime;
            spawnTimeInterval = timeInterval;
            
        }

        public void OnEnemyDeath(){
            enemiesDied++;
            Debug.Log("Killed");
        }

        public void OnEnemyPassed(){
            
            //Debug.Log("passed");
            enemiesDied++;
        }

        public bool CheckSpeciesWipeout(){
            //Debug.Log(enemyType + " " + enemiesDied + " " + specificEnemyCount);
            if(enemiesDied >= specificEnemyCount){
                return true;
            }
            else{
                return false;
            }
                
        }

        public void SetSpecificEnemyCount(){
            specificEnemyCount = (int) (Wave.enemyProportions[enemyType] * totalEnemiesThisWave);
            Debug.Log("Type: " + enemyType + ", Count: " + specificEnemyCount);
        }

        public void SetSpawnTimes(){
            if(specificEnemyCount != null){
                spawnTimes = new float[specificEnemyCount];
                spawned = new bool[specificEnemyCount];

                for(int i = 0; i < specificEnemyCount; i++){
                    spawnTimes[i] = spawnStartTime + i * spawnTimeInterval;
                    spawned[i] = false;
                }
            }
            
        }

        public void SetSpawnPosition(){
            spawnPoints = new spawnPosition[specificEnemyCount];

            for(int i = 0; i<specificEnemyCount; i++){
                int spawnPositionIndex = UnityEngine.Random.Range(0,6);
            
                float[] spawnPositionXArray = new float[] {-5f,-3f,-1f,1f,3f,5f};
                spawnPoints[i].spawnPointX = spawnPositionXArray[spawnPositionIndex];
                spawnPoints[i].spawnPointY = 1f;
                spawnPoints[i].spawnPointZ = 80f; //WAS 80
            }

            /*
            if(specificEnemyCount >= 5){
                spawnPositions = new Vector3[5];
                VFormation(70, 4f, spawnTimes, spawnPositions, 0, 5);
            }
            */
            
        }

        public void MakeFormations(){
            int spawnsSet = 0;
            int i=0;

            spawnTimes = new float[specificEnemyCount];
            spawnPositions = new Vector3[specificEnemyCount];
            spawned = new bool[specificEnemyCount];

            if(Wave.waveNumber < 7 && Wave.waveNumber != 2){
                float z = UnityEngine.Random.Range(75f, 80f);
                DisperseFormation(z, spawnStartTime, spawnTimeInterval, spawnTimes, spawnPositions, 0, specificEnemyCount);
            }
            else{
                while(spawnsSet < specificEnemyCount){
                    float z = UnityEngine.Random.Range(65f, 85f);
                    
                    if(specificEnemyCount-spawnsSet < 3){
                        DisperseFormation(z, spawnStartTime + i*spawnTimeInterval, 2f, spawnTimes, spawnPositions, spawnsSet, specificEnemyCount-spawnsSet);
                        spawnsSet += specificEnemyCount-spawnsSet;
                    }
                    else if(specificEnemyCount-spawnsSet == 3){
                        int disperseSelection = UnityEngine.Random.Range(0,3);
                        if(disperseSelection == 0){
                            DisperseFormation(z, spawnStartTime + i*spawnTimeInterval, 2f, spawnTimes, spawnPositions, spawnsSet, 3);
                        }
                        else
                            ThreeFormation(z, spawnStartTime + i*spawnTimeInterval, spawnTimes, spawnPositions, spawnsSet);
                        spawnsSet += 3;
                    }
                    else if(specificEnemyCount-spawnsSet == 4){
                        DashFormation(z, spawnStartTime + i*spawnTimeInterval, spawnTimes, spawnPositions, spawnsSet);
                        spawnsSet += 4;
                    }
                    else if(specificEnemyCount-spawnsSet == 5){
                        VFormation(z, spawnStartTime + i*spawnTimeInterval, spawnTimes, spawnPositions, spawnsSet);
                        spawnsSet += 5;
                    }
                    else if(specificEnemyCount-spawnsSet == 6){
                        AFormation(z, spawnStartTime + i*spawnTimeInterval, spawnTimes, spawnPositions, spawnsSet);
                        spawnsSet += 6;
                    }
                    else{
                        int selection = UnityEngine.Random.Range(0,4);
                        switch(selection){
                            case 0:
                                ThreeFormation(z, spawnStartTime + i*spawnTimeInterval, spawnTimes, spawnPositions, spawnsSet);
                                spawnsSet += 3;
                                break;
                            case 1:
                                VFormation(z, spawnStartTime + i*spawnTimeInterval, spawnTimes, spawnPositions, spawnsSet);
                                spawnsSet += 5;
                                break;
                            case 2:
                                AFormation(z, spawnStartTime + i*spawnTimeInterval, spawnTimes, spawnPositions, spawnsSet);
                                spawnsSet += 6;
                                break;
                            case 3:
                                int disperseCount = UnityEngine.Random.Range(0, (int) (specificEnemyCount - spawnsSet) / 2);
                                DisperseFormation(z, spawnStartTime + i*spawnTimeInterval, 2f, spawnTimes, spawnPositions, spawnsSet, disperseCount);
                                spawnsSet += disperseCount;
                                break;
                            default: break;
                        }
                    }

                    i++;
                }
            }
            
        }

        public void VFormation(float z, float startTime, float[] spawnTimes, Vector3[] spawnPositions, int startIndex){
            int j = UnityEngine.Random.Range(0,2);
            float forwardShift = 3f;
            float tempZ = z;
            for(int i = 0; i < 5; i++){
                spawnPositions[startIndex+i] = new Vector3(2*(j+i) - 5f,1f, tempZ);
                spawnTimes[startIndex+i] = startTime;
                //Debug.Log("i: " + i +", SpawnPosition: " + spawnPositions[startIndex+i] + ", SpawnTimes: " + spawnTimes[startIndex+i]);
                if(i < 2) tempZ += forwardShift;
                else tempZ -= forwardShift;
            }
        }
        public void AFormation(float z, float startTime, float[] spawnTimes, Vector3[] spawnPositions, int startIndex){
            float forwardShift = 5f;
            float tempZ = z;
            for(int i = 0; i < 6; i++){
                spawnPositions[startIndex+i] = new Vector3(2*(i) - 5f,1f, tempZ);
                spawnTimes[startIndex+i] = startTime;
                //Debug.Log("i: " + i +", SpawnPosition: " + spawnPositions[startIndex+i] + ", SpawnTimes: " + spawnTimes[startIndex+i]);
                if(i < 2) tempZ -= forwardShift;
                else if(i == 2) tempZ -= 0f;
                else tempZ += forwardShift;
            }
        }
        public void DashFormation(float z, float startTime, float[] spawnTimes, Vector3[] spawnPositions, int startIndex){
            int j = UnityEngine.Random.Range(0,3);
            for(int i = 0; i < 4; i++){
                spawnPositions[startIndex+i] = new Vector3(2*(j+i) - 5f,1f, z);
                spawnTimes[startIndex+i] = startTime;
                //Debug.Log("i: " + i +", SpawnPosition: " + spawnPositions[startIndex+i] + ", SpawnTimes: " + spawnTimes[startIndex+i]);
            }
        }
        public void DisperseFormation(float z, float startTime, float timeInterval, float[] spawnTimes, Vector3[] spawnPositions, int startIndex, int num){
            for(int i = 0; i < num; i++){
                int rowIndex = UnityEngine.Random.Range(0,6);
                spawnPositions[startIndex+i] = new Vector3(2*(rowIndex) - 5f, 1f, z);
                spawnTimes[startIndex+i] = startTime + timeInterval*i;
            }
        }
        public void ThreeFormation(float z, float startTime, float[] spawnTimes, Vector3[] spawnPositions, int startIndex){
            int j = UnityEngine.Random.Range(0,2);
            for(int i = 0; i < 3; i++){
                spawnPositions[startIndex+i] = new Vector3(2*(j+2*i) - 5f,1f, z);
                if(i == 1) spawnPositions[startIndex+i] = new Vector3(2*(j+2*i) - 5f,1f, z + 2f);
                spawnTimes[startIndex+i] = startTime;
                //Debug.Log("i: " + i +", SpawnPosition: " + spawnPositions[startIndex+i] + ", SpawnTimes: " + spawnTimes[startIndex+i]);
            }
        }

    }

    public void OnEnemyExploded(object source, EnemyDeathEventArgs enemyDeath){
        switch(enemyDeath.enemyType){
            case "Basic":
                basicSpawner.OnEnemyDeath();
                break;
            case "Sprinter":
                sprinterSpawner.OnEnemyDeath();
                break;
            case "Dodger":
                dodgerSpawner.OnEnemyDeath();
                break;
            case "Jumper":
                jumperSpawner.OnEnemyDeath();
                break;
            case "Tank":
                tankSpawner.OnEnemyDeath();
                break;
            default:
                break;
        }
    }
    
    

    // Start is called before the first frame update
    void Start(){

        startTime = Time.time; 
        spawnTime = startTime;

        startNextWave = false;

        currentWave = new Wave(5);
        
        introductionOrder = CreateIntroductionOrder();
        
        NextWave();

        //FindObjectOfType<Enemy>().EnemyExploded += this.OnEnemyExploded;
        BetweenWavesEvent += FindObjectOfType<CameraAnimation>().ViewChangeBetweenWaves;
        
        
        //PlayerPrefs.DeleteKey("BasicIntroduced");
        //PlayerPrefs.DeleteKey("SprinterIntroduced");
        //PlayerPrefs.DeleteKey("JumperIntroduced");
        //PlayerPrefs.DeleteKey("DodgerIntroduced");
        //PlayerPrefs.DeleteKey("TankIntroduced");
        //PlayerPrefs.DeleteKey("DoneTut");
    }

    

    List<string> CreateIntroductionOrder(){
        string[] enemyTypeCollection = {"Sprinter", "Jumper", "Tank", "Dodger"};
        //string[] enemyTypeCollection = {"Jumper", "Jumper", "Jumper", "Jumper"};
        //string[] enemyTypeCollection = {"Sprinter", "Sprinter", "Sprinter", "Sprinter"};
        //string[] enemyTypeCollection = {"Dodger", "Dodger", "Dodger", "Dodger"};
        //string[] enemyTypeCollection = {"Tank", "Tank", "Tank", "Tank"};
        List<int> selectedIndexes = new List<int>();
        List<string> orderedEnemies = new List<string>();

        UnityEngine.Random.seed = System.DateTime.Now.Millisecond;
        while(selectedIndexes.Count < 4){
            int newIndex = UnityEngine.Random.Range(0,4);
            if( !selectedIndexes.Contains(newIndex) ){
                selectedIndexes.Add(newIndex);
                orderedEnemies.Add(enemyTypeCollection[newIndex]);
                //Debug.Log(enemyTypeCollection[newIndex]);
            }
        }
        
        return orderedEnemies;

    }

    void NextWave(){
        spawnTime = Time.time;
        
        updateWave();
        
        NextWaveEnemies();
        InitializeEnemySpawners(basicSpawner);
        InitializeEnemySpawners(sprinterSpawner);
        InitializeEnemySpawners(jumperSpawner);
        InitializeEnemySpawners(dodgerSpawner);
        InitializeEnemySpawners(tankSpawner);
    }

    
    void updateWave(){
        //update the number of enemies and the proportion of them
        //update the enemySpawner intance parameters
        if(Wave.waveNumber == 1){
            ClearEnemyProportions();
            Wave.enemyProportions["Basic"] = 1f;
        }
        else if(Wave.waveNumber == 2){
            currentWave.numberOfEnemies = 7;
            ClearEnemyProportions();
            Wave.enemyProportions["Basic"] = 1f;

            basicStartTime = 5f;
            basicTimeInterval = 0.5f;
        }
        else if(Wave.waveNumber < 7){
            currentWave.numberOfEnemies = 5;
            ClearEnemyProportions();
            Wave.enemyProportions["Basic"] = 0.4f;
            Wave.enemyProportions[introductionOrder[Wave.waveNumber - 3]] = 0.6f;

            basicStartTime = 5f;
            basicTimeInterval = 0.5f;

            if(introductionOrder[Wave.waveNumber - 3] == sprinterSpawner.enemyType){
                sprinterStartTime = 3f;
                sprinterTimeInterval = 3f;
            }
            if(introductionOrder[Wave.waveNumber - 3] == jumperSpawner.enemyType){
                jumperStartTime = 3f;
                jumperTimeInterval = 3f;
            }
            if(introductionOrder[Wave.waveNumber - 3] == dodgerSpawner.enemyType){
                dodgerStartTime = 3f;
                dodgerTimeInterval = 3f;
            }
            if(introductionOrder[Wave.waveNumber - 3] == tankSpawner.enemyType){
                tankStartTime = 3f;
                tankTimeInterval = 3f;
            }
            
            
        }
        else{
            currentWave.numberOfEnemies += 3;
            ClearEnemyProportions();

            int waveTypeSelection = UnityEngine.Random.Range(0,200);

            if(waveTypeSelection < 170){
                Wave.enemyProportions["Basic"] = 0.25f;
                Wave.enemyProportions["Sprinter"] = 0.2f;
                Wave.enemyProportions["Jumper"] = 0.2f;
                Wave.enemyProportions["Dodger"] = 0.2f;
                Wave.enemyProportions["Tank"] = 0.15f;
            }
            else if(waveTypeSelection < 176){
                //Basic Burst 
                Wave.enemyProportions["Basic"] = 0.55f;
                Wave.enemyProportions["Sprinter"] = 0.1f;
                Wave.enemyProportions["Jumper"] = 0.1f;
                Wave.enemyProportions["Dodger"] = 0.1f;
                Wave.enemyProportions["Tank"] = 0.15f;
            }
            else if(waveTypeSelection < 182){
                //Sprinter Burst
                Wave.enemyProportions["Basic"] = 0.1f;
                Wave.enemyProportions["Sprinter"] = 0.55f;
                Wave.enemyProportions["Jumper"] = 0.1f;
                Wave.enemyProportions["Dodger"] = 0.15f;
                Wave.enemyProportions["Tank"] = 0.10f;
            }
            else if(waveTypeSelection < 188){
                //Jumper Burst
                Wave.enemyProportions["Basic"] = 0.15f;
                Wave.enemyProportions["Sprinter"] = 0.1f;
                Wave.enemyProportions["Jumper"] = 0.55f;
                Wave.enemyProportions["Dodger"] = 0.1f;
                Wave.enemyProportions["Tank"] = 0.10f;
            }
            else if(waveTypeSelection < 194){
                //Dodger Burst
                Wave.enemyProportions["Basic"] = 0.10f;
                Wave.enemyProportions["Sprinter"] = 0.15f;
                Wave.enemyProportions["Jumper"] = 0.1f;
                Wave.enemyProportions["Dodger"] = 0.55f;
                Wave.enemyProportions["Tank"] = 0.10f;
            }
            else{
                //Tank Burst
                Wave.enemyProportions["Basic"] = 0.30f;
                Wave.enemyProportions["Sprinter"] = 0.15f;
                Wave.enemyProportions["Jumper"] = 0.10f;
                Wave.enemyProportions["Dodger"] = 0.10f;
                Wave.enemyProportions["Tank"] = 0.35f;
            }

            float spawnCoeff = 0.75f;

            basicStartTime = 1f;
            //basicTimeInterval = UnityEngine.Random.Range(0.1f*currentWave.numberOfEnemies, Mathf.Sqrt(currentWave.numberOfEnemies));
            basicTimeInterval = Mathf.Min(15f, Wave.waveNumber * spawnCoeff);

            sprinterStartTime = UnityEngine.Random.Range(0.4f*currentWave.numberOfEnemies, 0.5f*currentWave.numberOfEnemies);
            //sprinterTimeInterval = UnityEngine.Random.Range(0.1f*currentWave.numberOfEnemies, Mathf.Sqrt(currentWave.numberOfEnemies));
            sprinterStartTime = 10f;
            sprinterTimeInterval = Mathf.Min(15f, Wave.waveNumber * spawnCoeff);

            jumperStartTime = UnityEngine.Random.Range(0.5f*currentWave.numberOfEnemies, 0.6f*currentWave.numberOfEnemies);
            //jumperTimeInterval = UnityEngine.Random.Range(0.1f*currentWave.numberOfEnemies, Mathf.Sqrt(currentWave.numberOfEnemies));
            jumperStartTime = 15f;
            jumperTimeInterval = Mathf.Min(15f, Wave.waveNumber * spawnCoeff);
        
            dodgerStartTime = UnityEngine.Random.Range(0.6f*currentWave.numberOfEnemies, 0.75f*currentWave.numberOfEnemies);
            //dodgerTimeInterval = UnityEngine.Random.Range(0.1f*currentWave.numberOfEnemies, Mathf.Sqrt(currentWave.numberOfEnemies));
            dodgerStartTime = 5f;
            dodgerTimeInterval = Mathf.Min(15f, Wave.waveNumber * spawnCoeff);
        
            tankStartTime = UnityEngine.Random.Range(0.7f*currentWave.numberOfEnemies, 1f*currentWave.numberOfEnemies);
            //tankTimeInterval = UnityEngine.Random.Range(0.1f*currentWave.numberOfEnemies, Mathf.Sqrt(currentWave.numberOfEnemies));
            tankStartTime = 20f;
            tankTimeInterval = Mathf.Min(10f, Wave.waveNumber * spawnCoeff);
        }
    }

    void ClearEnemyProportions(){
        Wave.enemyProportions["Basic"] = 0f;
        Wave.enemyProportions["Sprinter"] = 0f;
        Wave.enemyProportions["Jumper"] = 0f;
        Wave.enemyProportions["Dodger"] = 0f;
        Wave.enemyProportions["Tank"] = 0f;
    }

    //creates new enemy spawner objects from EnemyWaveSpawner class for each enemy type using the instance parameters
    void NextWaveEnemies(){

        basicSpawner = new EnemyWaveSpawn("Basic", basicStartTime, basicTimeInterval, currentWave.numberOfEnemies);
        sprinterSpawner = new EnemyWaveSpawn("Sprinter", sprinterStartTime, sprinterTimeInterval, currentWave.numberOfEnemies);
        jumperSpawner = new EnemyWaveSpawn("Jumper", jumperStartTime, jumperTimeInterval, currentWave.numberOfEnemies);
        dodgerSpawner = new EnemyWaveSpawn("Dodger", dodgerStartTime, dodgerTimeInterval, currentWave.numberOfEnemies);
        tankSpawner = new EnemyWaveSpawn("Tank", tankStartTime, tankTimeInterval, currentWave.numberOfEnemies);

        //GC.Collect();

        
    }

    //spawns the enemies of type enemySpawnInfo in a timely manner using the parameter's spawning properties 
    void SpawnEnemy(EnemyWaveSpawn enemySpawnInfo){
        BasicEnemy basicEnemyInstance;
        SprinterEnemy sprinterEnemyInstance;
        JumperEnemy jumperEnemyInstance;
        DodgerEnemy dodgerEnemyInstance;
        TankEnemy tankEnemyInstance;
        
        
        for(int i = 0; i < enemySpawnInfo.specificEnemyCount; i++){
            if(Time.time - spawnTime >= enemySpawnInfo.spawnTimes[i] && enemySpawnInfo.spawned[i] == false){
                //Vector3 spawnPoint = new Vector3(enemySpawnInfo.spawnPoints[i].spawnPointX, enemySpawnInfo.spawnPoints[i].spawnPointY, enemySpawnInfo.spawnPoints[i].spawnPointZ);
                Vector3 spawnPoint = enemySpawnInfo.spawnPositions[i];

                if(string.Compare(enemySpawnInfo.enemyType, "Basic") == 0){
                    Debug.Log("About to spawn basic enemy");
                    basicEnemyInstance = Instantiate(basicEnemyPrefab, spawnPoint, Quaternion.identity);
                    if(PlayerPrefs.GetInt("BasicIntroduced", 0) == 0 && Wave.waveNumber == 2){
                        StartCoroutine(IntroduceEnemies(basicEnemyInstance.gameObject, enemySpawnInfo.enemyType, 10f, 5f, 5f));
                        PlayerPrefs.SetInt("BasicIntroduced", 1);
                    }
                }
                else {
                    Debug.Log("About to spawn other enemy");
                }
                if(string.Compare(enemySpawnInfo.enemyType, "Sprinter") == 0){
                    sprinterEnemyInstance = Instantiate(sprinterEnemyPrefab, spawnPoint, Quaternion.identity);
                    if(PlayerPrefs.GetInt("SprinterIntroduced", 0) == 0){
                        StartCoroutine(IntroduceEnemies(sprinterEnemyInstance.gameObject, enemySpawnInfo.enemyType, 5f, 15f, 10f));
                        PlayerPrefs.SetInt("SprinterIntroduced", 1);
                    }
                }
                if(string.Compare(enemySpawnInfo.enemyType, "Jumper") == 0){
                    jumperEnemyInstance = Instantiate(jumperEnemyPrefab, spawnPoint, Quaternion.identity);
                    if(PlayerPrefs.GetInt("JumperIntroduced", 0) == 0){
                        StartCoroutine(IntroduceEnemies(jumperEnemyInstance.gameObject, enemySpawnInfo.enemyType, 10f, 7f, 10f));
                        PlayerPrefs.SetInt("JumperIntroduced", 1);
                    }
                }
                if(string.Compare(enemySpawnInfo.enemyType, "Dodger") == 0){
                    dodgerEnemyInstance = Instantiate(dodgerEnemyPrefab, spawnPoint, Quaternion.identity);
                    if(PlayerPrefs.GetInt("DodgerIntroduced", 0) == 0){
                        StartCoroutine(IntroduceEnemies(dodgerEnemyInstance.gameObject, enemySpawnInfo.enemyType, 20f, 5f, 15f));
                        PlayerPrefs.SetInt("DodgerIntroduced", 1);
                    }
                }
                if(string.Compare(enemySpawnInfo.enemyType, "Tank") == 0){
                    tankEnemyInstance = Instantiate(tankEnemyPrefab, spawnPoint, Quaternion.identity);
                    if(PlayerPrefs.GetInt("TankIntroduced", 0) == 0){
                        StartCoroutine(IntroduceEnemies(tankEnemyInstance.gameObject, enemySpawnInfo.enemyType, 50f, 3f, 20f));
                        PlayerPrefs.SetInt("TankIntroduced", 1);
                    }
                }
                Debug.Log("New enemy spawned");
                enemySpawnInfo.spawned[i] = true;
                
            }
        }
    }
    AnimatorStateInfo currInfo;
    IEnumerator IntroduceEnemies(GameObject go, string enemyType, float health, float speed, float damage){
        currInfo = waveAnimator.GetCurrentAnimatorStateInfo(0);
        while(!currInfo.IsName("PostAnim")){
            currInfo = waveAnimator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }
        //yield return new WaitForSeconds(1f);
        ca.IntroduceCharacter(go.transform, enemyType, health, speed, damage);
    }

    //Initializes the enemy Objects using the setter functions of the objects - Very simple function
    void InitializeEnemySpawners(EnemyWaveSpawn enemySpawnInfo){
        enemySpawnInfo.SetSpecificEnemyCount();
        //enemySpawnInfo.SetSpawnTimes();
        //enemySpawnInfo.SetSpawnPosition();
        enemySpawnInfo.MakeFormations();
    }
    public UITutorial uitut;
    public void AdvanceToNextWave(){
        Wave.waveNumber++;
        waveNumberText.text = Wave.waveNumber.ToString();
        //wi.OnBetweenWaves();
        ca.GoBackToDefaultView();
        NextWave();
        Debug.Log("next wave");
        inv.CloseBetweenWaves();
        StartCoroutine(EndBetweenWaves());
        manager.IncrementWaveNumber(1);
    }
    IEnumerator EndBetweenWaves(){
        yield return new WaitForSeconds(0.2f);
        openNextWave = false;
        nextWaveButton.tag = "Click Through";
        nextWaveText.tag = "Click Through";
    }

    public void RestartCurrentWave(){
        waveNumberText.text = Wave.waveNumber.ToString();
        NextWave();
        inv.CloseBetweenWaves();
    }

    //Waits for input after a wave is finished before proceeding to the next wave
    void InBetweenWaves(){
        if(Wave.waveNumber == 1 && PlayerPrefs.GetInt("DoneTut", 0) == 0){
            uitut.startTut = true;
            
        }
        openNextWave = true;
        nextWaveButton.tag = "Untagged";
        nextWaveText.tag = "Untagged";
        Debug.Log("In between Waves");
        StartCoroutine(BeginBetweenWaves());
    }
    IEnumerator BeginBetweenWaves(){
        yield return new WaitForSeconds(1f);
        Debug.Log("After a wave waiting");
        BetweenWavesEvent();
        inv.OpenBetweenWaves();
    }
    
    
    public bool startNextWave;
    public bool openNextWave = false;
    
    // Update is called once per frame
    void Update()
    {
        if(startNextWave){
            SpawnEnemy(basicSpawner);
            SpawnEnemy(sprinterSpawner);
            SpawnEnemy(jumperSpawner);
            SpawnEnemy(dodgerSpawner);
            SpawnEnemy(tankSpawner);

            if(basicSpawner.CheckSpeciesWipeout() && sprinterSpawner.CheckSpeciesWipeout() && jumperSpawner.CheckSpeciesWipeout() && dodgerSpawner.CheckSpeciesWipeout() && tankSpawner.CheckSpeciesWipeout() ){
                
                if(!pm.playerDead){
                    if(!openNextWave) InBetweenWaves();
                }
                
        
            }
            else{
                
            }
        }
        //Debug.Log("Wave no.: " + Wave.waveNumber);
        
    }
}
