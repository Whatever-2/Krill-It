using UnityEngine;


public class PlaySound : MonoBehaviour
{
    //ways of playing sound listed here for easy access, can be called from other scripts or from unity events in the inspector

    void Update()
    {
        
    }
    public void PlaySoundFX(int soundIndex)
    {
        AudioManager.instance.PlaySFX(soundIndex);
    }


}
