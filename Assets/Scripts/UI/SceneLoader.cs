using System.Diagnostics;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void loadScenByNumber(int sceneNumber)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNumber);
    }   

    public  void LoadNextScene()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void LoadPreviousScene()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex - 1);
    }

    public void QuitGame()
    {
        UnityEngine.Debug.Log("Quitting game...");
        Application.Quit();
    }



}
