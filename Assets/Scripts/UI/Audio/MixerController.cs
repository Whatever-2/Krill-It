using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer Mixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider SFXSlider;
        [SerializeField] private Slider MusicSlider;
    private void Start()
    {
        
            if (PlayerPrefs.HasKey("MasterVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume();
            SetSFXVolume();
            SetMusicVolume();
        }
            
    }


    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        Mixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);

    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        Mixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);

    }
    
          public void SetMusicVolume()
    {
        float volume = MusicSlider.value;
        Mixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);

    }
    
    
    private void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");


        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
    }
    
 

}
