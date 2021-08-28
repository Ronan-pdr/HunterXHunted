using System.Collections;
using System.Collections.Generic;
using Script.Audio;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using Script.EntityPlayer;

public class SettingsMenu : MonoBehaviour
{
    Resolution[] resolutions;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    int currentResolutionIndex = 0;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Slider volumeSlider;
    
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private TMP_Dropdown graphicsDropdown;
    private Resolution selectedResolution;
    
    void Start()
    {
        resolutions = Screen.resolutions;
        LoadSettings();
        int quality = PlayerPrefs.GetInt("_qualityIndex", 2);
        QualitySettings.SetQualityLevel(quality);
        graphicsDropdown.value = quality;
        CreateResolutionDropdown();
    }

    public void StartVolume()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volumeMenu",30f*0.15f/100f);
        audioManager.audioSource.volume = PlayerPrefs.GetFloat("volumeMenu", 30f*0.15f/100f);
    }
    
    
    private void LoadSettings()
    {
        selectedResolution = new Resolution();
        selectedResolution.width = PlayerPrefs.GetInt("resolutionWidth", Screen.currentResolution.width);
        selectedResolution.height = PlayerPrefs.GetInt("resolutionHeight", Screen.currentResolution.height);
        selectedResolution.refreshRate = PlayerPrefs.GetInt("resolutionRefreshRate", Screen.currentResolution.refreshRate);
         
        fullScreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) > 0;
 
        Screen.SetResolution(
            selectedResolution.width,
            selectedResolution.height,
            fullScreenToggle.isOn
        );
    }
    
    private void CreateResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height 
                                                                      && resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                currentResolutionIndex = i;
            }
        }
        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("resolutionIndex", currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();
    }
    
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("_qualityIndex", qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolutionWidth", resolution.width);
        PlayerPrefs.SetInt("resolutionHeight", resolution.height);
        PlayerPrefs.SetInt("resolutionRefreshRate", resolution.refreshRate);
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
    }

    public void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("volumeMenu", value);
        PlayerPrefs.Save();

        if (audioManager)
        {
            audioManager.audioSource.volume = value;
        }
            
    }
}