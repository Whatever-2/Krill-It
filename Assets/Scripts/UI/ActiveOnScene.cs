using UnityEngine;

public class ActiveOnScene : MonoBehaviour
{
    [SerializeField] private int SceneIndex;
    private int currentSceneIndex;
    // gameobject will onlt be active is current scene is scene specified
    void Start()
    {
        currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        gameObject.SetActive(currentSceneIndex == SceneIndex);
    }

    void Update()
    {
        
        currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        //updates active state of gameobject based on current scene, can be used for things like shop ui that should only be active in certain scenes
        if (currentSceneIndex == SceneIndex && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        else if (currentSceneIndex != SceneIndex && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

}
