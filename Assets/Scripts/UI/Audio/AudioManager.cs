using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("----------Audio Source----------")]
    //Plays Music sand Sound Effects
    [SerializeField] AudioSource musicSource; //controls music
    [SerializeField] AudioSource SFXSource; // controls sfx

    [Header("----------Audio Clip----------")]

    [Header("===Music===")]
    public AudioClip BGM;


    [Header("===SFX===")]
    public AudioClip[] SFX;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicSource.clip = BGM;
        musicSource.Play();
    }

    public void PlaySFX(int index)
    {
        SFXSource.PlayOneShot(SFX[index]);
    }

}
