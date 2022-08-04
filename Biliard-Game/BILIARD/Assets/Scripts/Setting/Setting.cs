using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
/// <summary>
/// class that responsible on setting of the game
/// </summary>

public class Setting : MonoBehaviour
{
  
    [SerializeField] AudioSource audioSource = default;
    [SerializeField] Slider musicSlider = default, tableColorSlider = default;
    [SerializeField] Image xMusicImage = default;
    [SerializeField] Material tableMaterial = default;
    [SerializeField] LineRenderer predictionLineRenderer = default;
    [SerializeField] Toggle predictionLineToggle = default;
    float lastAmountOfMusicSliderValue = default;
    float predictionLineTransperencyAmountUserPrefs = default;
    float musicValueUserPrefs = default;
    bool  isPredictionLineActiveUserPrefs = default;
    private void OnDisable()
    {
        SetPlayerPreferences();
    }
    private void Start()
    {
        LimitFramRate();
        GetPlayerPreferences();
    }
    /// <param name="buildIndex">build index of scene to load</param>
    public void StartNewGame(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
        Time.timeScale = 1;
    }
    public static void QuitGame() => Application.Quit();
  
    void LimitFramRate()=> Application.targetFrameRate = 60;
    void GetPlayerPreferences()

    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            audioSource.volume = PlayerPrefs.GetFloat("musicVolume");
            musicSlider.value = PlayerPrefs.GetFloat("sliderValue");
        }
        if (PlayerPrefs.HasKey("lineRendererToggleValue"))
        {
            predictionLineToggle.isOn = PlayerPrefs.GetInt("lineRendererToggleValue") == 1;
            predictionLineRenderer.startColor = new Color(predictionLineRenderer.startColor.r, predictionLineRenderer.startColor.g,
            predictionLineRenderer.startColor.b, PlayerPrefs.GetFloat("predictionLineTransperencyAmount"));
        }
        if (PlayerPrefs.HasKey("tableColor"))
        {
            float r = PlayerPrefs.GetFloat("r");
            float g = PlayerPrefs.GetFloat("g");
            float b = PlayerPrefs.GetFloat("b");
            tableMaterial.color = new Color(r, g, b);
        }

    }
    void SetPlayerPreferences()
    {
        PlayerPrefs.SetFloat("musicVolume", musicValueUserPrefs);
        PlayerPrefs.SetFloat("sliderValue", musicValueUserPrefs);
        PlayerPrefs.SetInt("lineRendererToggleValue", isPredictionLineActiveUserPrefs ? 1 : 0);
        PlayerPrefs.SetFloat("predictionLineTransperencyAmount", predictionLineTransperencyAmountUserPrefs);
        PlayerPrefs.SetFloat("r", tableMaterial.color.r);
        PlayerPrefs.SetFloat("g", tableMaterial.color.g);
        PlayerPrefs.SetFloat("b", tableMaterial.color.b);
    }
    public void AdjustMusicSliderVolume()
    {
        audioSource.volume = musicSlider.value;
        bool activateXvolumeimage = audioSource.volume == 0;
        xMusicImage.gameObject.SetActive(activateXvolumeimage);
        musicValueUserPrefs = musicSlider.value;
    }
    /// <summary>
    /// control on turn on/off volume button if the volume was larger than ziro when we press on the button we turn off the volume
    /// and if the volum was ziro we return the volum to the volume it was before we turn the volume off
    /// </summary>
    public void ActivateAndDeactivateMusic()
    {
        lastAmountOfMusicSliderValue = musicSlider.value > 0 ? musicSlider.value : lastAmountOfMusicSliderValue ;
        musicSlider.value = musicSlider.value > 0 ? 0 : lastAmountOfMusicSliderValue;
        bool activateXimage = musicSlider.value == 0;
        xMusicImage.gameObject.SetActive(activateXimage);
        musicValueUserPrefs = musicSlider.value;
    }
        
  /// <summary>
  /// chenge table color with slider
  /// </summary>
    public void ChangeTableColor()
    {
        float H, S, V;
        Color.RGBToHSV(tableMaterial.color, out H, out S, out V);
        tableMaterial.color = Color.HSVToRGB(tableColorSlider.value, S, V);
    }
   public void ActivateAndDeactivateSettingWindow(GameObject settingWindow)
    {
        bool isSettingWindowActiveInHirarchy = settingWindow.activeInHierarchy;
        settingWindow.gameObject.SetActive(!isSettingWindowActiveInHirarchy);
    }

    /// <summary>
    /// turn on end off second prediction line by chanching line renderer transperency amount
    /// </summary>
    public void ActivateAndDeactivatePredictionLine()
    {
        isPredictionLineActiveUserPrefs = predictionLineToggle.isOn;
        if (isPredictionLineActiveUserPrefs)
            predictionLineRenderer.startColor = new Color(Color.white.r,
            Color.white.g, Color.white.b,0.2f);
        else
           predictionLineRenderer.startColor = Color.clear;
           predictionLineTransperencyAmountUserPrefs = predictionLineRenderer.startColor.a;
    }
        
}
 
    
        
  
  
        
        
      
   
    

  
   
                
                
                

  

    
    

    
   

        
      




       
    



   

          
         
        
       

        
        
        
        


