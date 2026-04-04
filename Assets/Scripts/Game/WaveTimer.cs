using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WaveTimer: MonoBehaviour
{

    public float waveTime = 30f;          // Time per wave
    public int totalWaves = 3;

    public TMP_Text timerText;
    public TMP_Text waveText;
    public TMP_Text resultText;
    public ScoreManager scoreManager;

    float currentTime;
    int currentWave = 1;
    bool gameStopped = false;

    void Start()
    {
        resultText.gameObject.SetActive(false);
        StartWave();
    }

    void Update()
    {
        if (gameStopped) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timerText.text = Mathf.Ceil(currentTime).ToString();
        }
        else
        {
            EndWave();
        }
    }

    void StartWave()
    {
        currentTime = waveTime;
        waveText.text = "Wave " + currentWave;
    }

    void EndWave()
    {

        if (currentWave < totalWaves)
        {
            currentWave++;
            StartWave();
        }
        else
        {
            StopGame();
        }
    }

    void StopGame()
    {
        gameStopped = true;
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}