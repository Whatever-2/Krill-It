using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win_LoseCondition : MonoBehaviour
{
    //setup for lose condition
    private int PalaceHealth;
    [SerializeField]private int MaxPalaceHealth = 5;
    private int krillCount = 0;
    private bool isLose;

    //setup for win condition
    private int WaveNumber;
    [SerializeField] private WaveTimer waveTimer;
    private int WaveGoal;
    private bool isWin;


    [SerializeField] private int WinSceneIndex = 0;
    [SerializeField] private int LoseSceneIndex = 0;


    void Start()
    {
        PalaceHealth = MaxPalaceHealth;
        krillCount = 0;
        isLose = false;
        WaveNumber = waveTimer.CurrentWave();
        WaveGoal = waveTimer.totalWaves;
        isWin = false;
    }


    void Update()
    {
        CheckLoseCon();

        if (waveTimer.gameStopped)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(WinSceneIndex);
        }
        if (isLose)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(LoseSceneIndex);
        }


    }


    private void CheckLoseCon()
    {
        PalaceHealth = MaxPalaceHealth - krillCount;
        if (PalaceHealth == 0)
        {
            isLose = true;
        }
    }
/*    

    private void CheckWinCon()
    {
        //checks and updates the wave count
        WaveNumber = waveTimer.CurrentWave();
        if (WaveNumber > WaveGoal )
        {
            isWin = true;
        }
    }
    

*/

     private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
         krillCount++;   
         Destroy(collision.gameObject);
        }
    }


}
