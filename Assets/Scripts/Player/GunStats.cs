using UnityEngine;

public class GunStats : MonoBehaviour
{
    public int damage;
    public EnemyStats enemyHealth;

    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyStats enemy = collision.gameObject.GetComponent<EnemyStats>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
    
}
