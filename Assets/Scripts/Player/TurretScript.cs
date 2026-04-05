using UnityEngine;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;


public class TurretScript : MonoBehaviour
{


    public int maxHealth = 20;
    public int currentHealth;
    public Camera mainCamera;
    public GameObject bulletPrefab;
 
    
    [SerializeField] GameObject shootRange;
    [SerializeField] private float shootRangeDistance = 5f; // Distance for the shoot range indicator
    [SerializeField] private Vector2 shootRangeScale = Vector2.one; // X/Y scale multipliers for the range shape: (1,1)=circle
    [SerializeField] private bool shootRangeActive = true; // Flag to track if the shoot range indicator is active
   

    public Transform FirePoint;
    private Vector2 direction;
    private Rigidbody2D rb;


    //bullets 
    [SerializeField] private AnimationCurve trajectoryAnimationCurve;
    [SerializeField] private float bulletTravelTime = 0.7f;
    [SerializeField] private float bulletHeight = 1f;
    public float shooterTimer = 1.0f; // Time in seconds for the shooter to be active
    private float timer;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        if (mainCamera == null) mainCamera = Camera.main;
        timer = shooterTimer; // Initialize timer

    }

    private void Update()
    {
        
    // Shoot
    
        timer -= Time.deltaTime; // Decrease the timer by the time elapsed since the last frame


            CheckRangeActive(); // Check if any enemies are within the shoot range distance and update the shoot range active state
            if (timer < 0)
            {
                if (shootRangeActive) // Only check for shooting if the shoot range indicator is active
                {
                    Shoot();
                    resetTimer(); // Reset the timer after shooting
                }
                
            }
    }
       

  public void Shoot()
    {
        var go = Instantiate(bulletPrefab, FirePoint.position, FirePoint.rotation);
        var projectile = go.GetComponent<BulletController>();

        //TARGETS NEAREST ENEMY INSTEAD OF MOUSE POSITION
         
            if (projectile != null)
            {
                // Find the nearest enemy bsed on range of shootRangeDistance
                if (shootRange != null)
                {
                    float diameter = shootRangeDistance * 2f;
                    shootRange.transform.localScale = new Vector3(diameter * shootRangeScale.x, diameter * shootRangeScale.y, 1f); // circle when shootRangeScale is (1,1), oval otherwise
                }

                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                GameObject nearestEnemy = null;
                float minDistance = Mathf.Infinity;
                Vector3 turretPos = transform.position;

                foreach (GameObject enemy in enemies)
                {
                    float distance = Vector3.Distance(turretPos, enemy.transform.position);
                    
                    if (IsWithinShootRange(enemy.transform.position)) // Check if the enemy is within the shoot range shape
                    {
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestEnemy = enemy;
                        }
                    }
                }

                if (nearestEnemy != null)
                {
                    projectile.InitializeCurve(FirePoint.position, nearestEnemy.transform.position, trajectoryAnimationCurve, bulletTravelTime, bulletHeight);
                }
            }
        }

    private bool IsWithinShootRange(Vector3 targetPosition)
    {
        Vector2 offset = targetPosition - transform.position;
        float radiusX = shootRangeDistance * shootRangeScale.x;
        float radiusY = shootRangeDistance * shootRangeScale.y;

        if (Mathf.Approximately(radiusX, radiusY))
        {
            return offset.sqrMagnitude <= (radiusX * radiusX);
        }

        float normalizedX = offset.x / radiusX;
        float normalizedY = offset.y / radiusY;
        return (normalizedX * normalizedX) + (normalizedY * normalizedY) <= 1f;
    }

    private void CheckRangeActive()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        bool enemyInRange = false;

        foreach (GameObject enemy in enemies)
        {
            if (IsWithinShootRange(enemy.transform.position))
            {
                enemyInRange = true;
                break; // No need to check further if at least one enemy is in range
            }
        }

        shootRangeActive = enemyInRange; // Set the shoot range active state based on whether an enemy is in range
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void resetTimer()
    {
        timer = shooterTimer;
    }


}
