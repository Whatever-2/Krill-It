using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int damage;
    public Shooter playerHealth;
    public int EHealth = 100;
    public int currentHealth;

    //Money prefab
    public GameObject moneyPrefab;

    //Random drop range
    public int minMoneyDrop = 1;
    public int maxMoneyDrop = 5;

    //bounce force
    public float dropForce = 2f;

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

    void DropMoney()
    {
        if (moneyPrefab != null)
        {
            //Get random amount between min and max
            int amountToDrop = Random.Range(minMoneyDrop, maxMoneyDrop + 1);

            for (int i = 0; i < amountToDrop; i++)
            {
                GameObject money = Instantiate(moneyPrefab, transform.position, Quaternion.identity);

                Rigidbody2D rb = money.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
                    rb.AddForce(randomDirection * dropForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
<<<<<<< HEAD
        currentHealth -= damage;
        if (currentHealth <= 0)
=======
        currentEHealth -= damage;

        if (currentEHealth <= 0)
>>>>>>> parent of 35e4700 (Revert "added gold drop rate")
        {
            DropMoney();
            Destroy(gameObject);
        }
    }   
}
