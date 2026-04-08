using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyType
{
    public GameObject enemyPrefab;
    public int amount;
}

[System.Serializable]
public class Wave
{
    public List<EnemyType> enemies;
}

public class WaveManager : MonoBehaviour
{
    public WaveTimer waveTimer;
    public Transform[] spawnPoints;
    public List<Wave> waves;

    void Awake()
    {
        waveTimer.OnWaveStart += StartWave;
    }

    void StartWave(int waveIndex)
    {
        if (waveIndex - 1 >= waves.Count) return;

        Wave wave = waves[waveIndex - 1];

        for (int i = 0; i < wave.enemies.Count; i++)
        {
            EnemyType enemy = wave.enemies[i];

            for (int j = 0; j < enemy.amount; j++)
            {
                SpawnEnemy(enemy.enemyPrefab);
            }
        }
    }

    void SpawnEnemy(GameObject enemy)
    {
        int random = Random.Range(0, spawnPoints.Length);
        Instantiate(enemy, spawnPoints[random].position, spawnPoints[random].rotation);
    }
}
