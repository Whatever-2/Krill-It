using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int damage;
    public Shooter playerHealth;
    public int EHealth = 100;
    public int currentHealth;

    public void Start()
    {
        currentHealth = EHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
