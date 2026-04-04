using UnityEngine;

public class MoneyScript : MonoBehaviour
{
    public int goldValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ScoreManager.Money.AddScore(goldValue);
            Destroy(gameObject);
        }
    }
}
