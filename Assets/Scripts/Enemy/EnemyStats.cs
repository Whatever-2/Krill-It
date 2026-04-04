using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int damage;
    public Shooter playerHealth;
    public int EHealth = 100;
    public int currentEHealth;

    public void Start()
    {
        currentEHealth = EHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        currentEHealth -= damage;
        if (currentEHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
