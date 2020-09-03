using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class Manager : MonoBehaviour, IUnityAdsListener
{
    private int enemiesKilled = 0;
    private int waveNumber = 0;
    private int score;
    
    public Animator gameoverAnim, pauseAnim;

    public Inventory inv;
    public PlayerMovement pm;
    public EnemySpawner es;
    public WaveAnimation wi;

    public TextMeshProUGUI scoreText, waveSurvived, enemiesKilledText, highscoreText, watchAdText, reviveText, pauseScoreText, pauseWaveSurvText;
    public Button watchAdButton;
    public Image watchAdButtonImage, gameOverPanelImage;
    public AudioSource asrc;

    string googlePlayID = "3765247";
    string myPlacementId = "rewardedVideo";
    bool testMode = false;

    void Start()
    {
        Advertisement.AddListener (this);
        Advertisement.Initialize (googlePlayID, testMode);

        waveSurvived.text = "Waves survived\t";
        enemiesKilledText.text = "Enemies killed\t";

        slider.normalizedValue = 0.5f;
    }

    public void ShowRewardedVideo() {
        // Check if UnityAds ready before calling Show method:
        if (Advertisement.IsReady(myPlacementId)) {
            Advertisement.Show(myPlacementId);
        } 
        else {
            Debug.Log("Rewarded video is not ready at the moment! Please try again later!");
        }
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish (string placementId, ShowResult showResult) {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished) {
            // Reward the user for watching the ad to completion.
            Debug.Log("You get a reward.");
            DisableWatchAdButton();
            RestartWave();
        } else if (showResult == ShowResult.Skipped) {
            // Do not reward the user for skipping the ad.
            Debug.Log("The ad was skipped so no error.");
            DisableWatchAdButton();
        } else if (showResult == ShowResult.Failed) {
            Debug.Log("The ad did not finish due to an error.");
        }
    }

    public void DisableWatchAdButton(){
        watchAdButton.enabled = false;
        watchAdText.enabled = false;
        reviveText.enabled = false;
        watchAdButtonImage.enabled = false;
    }

    public void RestartWave(){
        Time.timeScale = 1f;
        var enemyObjects = FindObjectsOfType<Enemy>();
        for(int i = 0; i < enemyObjects.Length; i ++){
            Destroy(enemyObjects[i].gameObject);
        }
        es.RestartCurrentWave();
        pm.playerDead = false;
        pm.playerHealth = 100f;
        pm.playerHealthBar.fillAmount = 1f;
        pm.healthBarCorrectWidthFillAmount = 1f;
        gameoverAnim.SetBool("gameOver", false);

        wi.OnBetweenWaves();
        gameOverPanelImage.enabled = false;
    }

    public void OnUnityAdsReady (string placementId) {
        // If the ready Placement is rewarded, show the ad:
        if (placementId == myPlacementId) {
            // Optional actions to take when the placement becomes ready(For example, enable the rewarded ads button)
        }
    }

    public void OnUnityAdsDidError (string message) {
        // Log the error.
    }

    public void OnUnityAdsDidStart (string placementId) {
        // Optional actions to take when the end-users triggers an ad.
    } 

    // When the object that subscribes to ad events is destroyed, remove the listener:
    public void OnDestroy() {
        Advertisement.RemoveListener(this);
    }

    public int GetEnemiesKilled(){
        return enemiesKilled;
    }

    public int GetWaveNumber(){
        return waveNumber;
    }

    public void IncrementEnemiesKilled(int inc){
        enemiesKilled += inc;
    }

    public void IncrementWaveNumber(int inc){
        waveNumber += inc;
    }

    public void OnGameOver(){
        gameOverPanelImage.enabled = true;
        gameoverAnim.SetBool("gameOver", true);
        inv.IsHotbarVisible(false);

        score = (waveNumber)*10 + enemiesKilled;
        scoreText.text = score.ToString();
        waveSurvived.text = "waves survived\t" + waveNumber.ToString();
        enemiesKilledText.text = "enemies killed\t" + enemiesKilled.ToString();
        if(score > PlayerPrefs.GetInt("HighScore", 0)){
            PlayerPrefs.SetInt("HighScore", score);
            highscoreText.text = "Highscore\t" + score.ToString();
        }
        else{
            highscoreText.text = "Highscore\t" + PlayerPrefs.GetInt("HighScore", 0).ToString();
        }
    }

    public void Restart(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenPauseMenu(){
        pauseAnim.SetBool("openPause", true);
        score = (waveNumber)*10 + enemiesKilled;
        pauseScoreText.text = "Score\t" + score.ToString();
        pauseWaveSurvText.text = "waves survived\t" + waveNumber.ToString();
        StartCoroutine(FreezeTime());
    }
    AnimatorStateInfo currInfo;
    IEnumerator FreezeTime(){
        currInfo = pauseAnim.GetCurrentAnimatorStateInfo(0);

        while(!currInfo.IsName("PauseClicked")){
            currInfo = pauseAnim.GetCurrentAnimatorStateInfo(0);
            Debug.Log("not pauseclicked");
            yield return null;
        }
        Debug.Log("Name: " + currInfo.IsName("New State") + currInfo.IsName("PauseClicked") + currInfo.IsName("PauseClosed"));
        Debug.Log("time: " + currInfo.normalizedTime);
        
        while(currInfo.normalizedTime < 1f){
            Debug.Log("is in while");
            yield return null;
        }
        Time.timeScale = 0f;
    }

    public void ClosePauseMenu(){
        pauseAnim.SetBool("openPause", false);
        Time.timeScale = 1f;
    }
    public Slider slider;
    void Update()
    {
        //slider.normalizedValue = asrc.volume;
        asrc.volume = slider.normalizedValue;
    }
}
