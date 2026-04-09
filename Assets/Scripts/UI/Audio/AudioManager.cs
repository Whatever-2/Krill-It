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
    [SerializeField] private AudioClip[] BGM;
    [SerializeField] private int[] BGMSceneIndex = { 0 }; //specifies which music to play in each scene, can be used to have different music for different levels or special music for boss fights]
    private AudioClip DefaultBGM; //default music to play if current scene index is not specified in BGMSceneIndex [default to first bgm]
    [Header("===SFX===")]
    [SerializeField] private AudioClip[] SFX;

    private int currentSceneIndex;
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
        if (musicSource == null)
        {
            Debug.LogError("Music Source is not assigned in the inspector.");
            return;
        }
        if (DefaultBGM == null)
        {
            Debug.LogError("Default BGM is not assigned in the inspector.");
            return;
        }
        DefaultBGM = BGM[0];
        musicSource.clip = DefaultBGM;
        musicSource.Play();
    }

    private void Update()
    {
        //checks current scene and changes music accordingly, can be used to change music in different levels or have special music for boss fights
        currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        if (BGMSceneIndex.Length > 0)
        {
            for (int i = 0; i < BGMSceneIndex.Length; i++)
            {
                if (currentSceneIndex == BGMSceneIndex[i])
                {
                    if (musicSource.clip != BGM[i])
                    {
                        musicSource.clip = BGM[i];
                        musicSource.Play();
                    }
                    return;
                }
            }
        }
    
    }

    public void PlaySFX(int index)
    {
        SFXSource.PlayOneShot(SFX[index]);
    }

}
