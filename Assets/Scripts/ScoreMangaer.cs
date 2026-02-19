using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    int score;
    public TMP_Text scoreText;
    //public text scoretext;legacy text system

    void Start()
    {
        score = 0;
        scoreText.text = score.ToString();
    }

    public void AddScore()
    {
        score++;
        scoreText.text = score.ToString();

    }

    public void SubScore()
    {
        if (score > 0)
            score--;

        scoreText.text = score.ToString();
    }

    public int GetScore()
    {
        return score;
    }

}