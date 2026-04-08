using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Money;
    int score;
    public TMP_Text scoreText;
    //public text scoretext;legacy text system

    void Awake()
    {
        Money = this;
    }

    void Start()
    {
        score = 250;
        scoreText.text = score.ToString();
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();

    }

    public void SubScore(int amount)
    {
        score -= amount;

        scoreText.text = score.ToString();
    }

    public int GetScore()
    {
        return score;
    }

}