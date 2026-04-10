using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class WaveTimer : MonoBehaviour
{
    public float waveTime = 30f;
    public int totalWaves = 3;

    public TMP_Text timerText;
    public TMP_Text waveText;

    public Action<int> OnWaveStart; //event that tells the wave manager to start the wave, and passes the current wave number so it can spawn the correct enemies

    float currentTime;
    int currentWave = 1;
    public bool gameStopped = false;

    void Start()
    {
        StartWave();
    }

    void Update()
    {
        if (gameStopped) return;

        //checks if there are still enemies at large
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            
            //ends wave early if all krill have been krilled
            if (enemies.Length == 0)
            {
                EndWave();
            }

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

        OnWaveStart?.Invoke(currentWave);//tells the wave manager to start the wave, and passes the current wave number so it can spawn the correct enemies
    }

    void EndWave()
    {
        if (currentWave < totalWaves ) 
        {
            currentWave++;
            StartWave();
        }
        else if (currentWave >= totalWaves)
        {
            //checks if there are still enemies at large
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0)
            {
                StopGame();
            }
        }
    }

  
    void StopGame()
    {
        gameStopped = true;
    }



    public int CurrentWave()
    {
        return currentWave;
    }

}