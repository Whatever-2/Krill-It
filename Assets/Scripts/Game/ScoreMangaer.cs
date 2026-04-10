using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Money;
    [SerializeField]int cash;
    public TMP_Text scoreText;
    //public text scoretext;legacy text system

    void Awake()
    {
        Money = this;
    }

    void Start()
    {
        scoreText.text = cash.ToString();
    }

    public void AddScore(int amount)
    {
        cash += amount;
        scoreText.text = cash.ToString();
    }

    public void SubScore(int amount)
    {
        cash -= amount;

        scoreText.text = cash.ToString();
    }

    public int GetScore()
    {
        return cash;
    }

}