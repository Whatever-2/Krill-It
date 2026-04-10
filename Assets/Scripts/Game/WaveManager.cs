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

    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    public float respawnDelay = 2f;

    private GameObject currentPlayer;
    private float respawnTimer;
    private bool isRespawning;

    void Awake()
    {
        waveTimer.OnWaveStart += StartWave;
    }

    void Start()
    {
        SpawnPlayer();
    }

    void Update()
    {
        if (currentPlayer == null && !isRespawning)
        {
            isRespawning = true;
            respawnTimer = respawnDelay;
        }

        if (isRespawning)
        {
            respawnTimer -= Time.deltaTime;

            if (respawnTimer <= 0f)
            {
                SpawnPlayer();
                isRespawning = false;
            }
        }
    }

    void SpawnPlayer()//function to instantiate the player at the spawn point when the game starts or when the player dies and needs to respawn
    {
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            currentPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        }
    }

    void StartWave(int waveIndex)//function to start a wave of enemies based on the wave index provided by the WaveTimer script
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

    void SpawnEnemy(GameObject enemy)//function to instantiate enemies at random spawn points when a wave starts
    {
        int random = Random.Range(0, spawnPoints.Length);
        Instantiate(enemy, spawnPoints[random].position, spawnPoints[random].rotation);
    }
}