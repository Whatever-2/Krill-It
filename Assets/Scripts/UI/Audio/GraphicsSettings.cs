using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GraphicsSettings : MonoBehaviour
{
   
    Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;


   void Start()
    {
        InitializeResolutionDropDown(); //initializes resolution dropdown with available resolutions and sets current resolution as default value

        if (PlayerPrefs.HasKey("QualitySetting") || PlayerPrefs.HasKey("FullscreenSetting") || PlayerPrefs.HasKey("ResolutionSetting"))
        {
            InitializeSavedSettings(); //initializes saves player pref settings
        }
    }

    private void InitializeResolutionDropDown()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void InitializeSavedSettings()
    {
        //load saved settings here and apply them to the UI elements and game settings
        if (PlayerPrefs.HasKey("QualitySetting"))
        {
            int qualityIndex = PlayerPrefs.GetInt("QualitySetting");
            SetQuality(qualityIndex);
        }
        if (PlayerPrefs.HasKey("FullscreenSetting"))
        {
            bool isFullscreen = PlayerPrefs.GetInt("FullscreenSetting") == 1;
            SetFullscreen(isFullscreen);
        }
        if (PlayerPrefs.HasKey("ResolutionSetting"))
        {
            int resolutionIndex = PlayerPrefs.GetInt("ResolutionSetting");
            SetResolution(resolutionIndex);
        }
    }

   public void SetQuality (int quality_index)
    {
        QualitySettings.SetQualityLevel(quality_index);
        PlayerPrefs.SetInt("QualitySetting", quality_index);
    }
    public void SetFullscreen (bool is_fullscreen)
    {
        Screen.fullScreen = is_fullscreen;
        PlayerPrefs.SetInt("FullscreenSetting", is_fullscreen ? 1 : 0);
    }

    public void SetResolution (int resolution_index)
    {
        Resolution resolution = resolutions[resolution_index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionSetting", resolution_index);
    }
}
