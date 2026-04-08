using UnityEngine;
using System.Runtime.Serialization;
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
   
    [SerializeField] private bool isHoming = true; // Flag to determine if bullets should home in on targets



    public Transform FirePoint;
    private Vector2 direction;
    private Rigidbody2D rb;


    //bullets 
    [SerializeField] private AnimationCurve trajectoryAnimationCurve;
    [SerializeField] private float bulletTravelTime = 0.7f;
    [SerializeField] private float bulletHeight = 1f;
    public float shooterTimer = 1.0f; // Time in seconds for the shooter to be active
    private float timer;
    private Animator animator;
    private float AnimationLength;
    private bool canPlay = true;
    [SerializeField] private AnimationClip AttackAnimation;
    private void Start()
    {

        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        if (mainCamera == null) mainCamera = Camera.main;
        timer = shooterTimer; // Initialize timer
        //instantiate scale of shoot range indicator
        if (shootRange != null)
        {
            float diameter = shootRangeDistance * 2f;
            shootRange.transform.localScale = new Vector3(diameter * shootRangeScale.x, diameter * shootRangeScale.y, 1f); // circle when shootRangeScale is (1,1), oval otherwise
        }
        //initiate enemy detection for shoot range
        CheckRangeActive(); // Check if any enemies are within the shoot range distance and update the shoot range active state
        animator = GetComponent<Animator>();
        if (animator != null && AttackAnimation != null)
        {
            AnimationLength = AttackAnimation.length;
        }
        else
        {
            Debug.LogWarning("Animator component or AttackAnimation clip is not assigned on turret. Shooting animation will not play.");
        }
    }

    private void Update()
    {
        
    // Shoot
    
        timer -= Time.deltaTime; // Decrease the timer by the time elapsed since the last frame


            CheckRangeActive(); // Check if any enemies are within the shoot range distance and update the shoot range active state

            // Update shoot range indicator scale
            if (shootRange != null)
            {
                float diameter = shootRangeDistance * 2f;
                shootRange.transform.localScale = new Vector3(diameter * shootRangeScale.x, diameter * shootRangeScale.y, 1f);
            }

        
            if (shootRangeActive) // Only check for shooting if the shoot range indicator is active
                {
                    //plays shoot animation before instantiating bullet, if animator component exists
                    if (timer < AnimationLength && canPlay) // Adjust the timing of the shoot animation trigger as needed
                    {
                        PlayShootAnimation();
                        canPlay = false; // Ensure the shoot animation is only triggered once per shooting cycle
                    }
                    
                    if (timer < 0)
                        {
                            Shoot();
                            resetTimer(); // Reset the timer after shooting
                            canPlay = true; // Allow the shoot animation to be triggered again in the next cycle
                        }
                
            }
    }
       

  public void Shoot()
    {
        if (bulletPrefab == null || FirePoint == null)
        {
            Debug.LogError("Bullet prefab or FirePoint is not assigned in the inspector.");
            return;
        }
        
      
        var go = Instantiate(bulletPrefab, FirePoint.position, FirePoint.rotation);
        var projectile = go.GetComponent<BulletController>();
        projectile.isTurretBullet = true;

        //TARGETS NEAREST ENEMY INSTEAD OF MOUSE POSITION
         
            if (projectile != null)
            {
                // Find the nearest enemy based on range

                Collider2D rangeCollider = shootRange != null ? shootRange.GetComponent<Collider2D>() : null;

                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                GameObject nearestEnemy = null;
                float minDistance = Mathf.Infinity;
                Vector3 turretPos = transform.position;

                foreach (GameObject enemy in enemies)
                {
                    float distance = Vector3.Distance(turretPos, enemy.transform.position);
                    
                    bool inRange = false;
                    if (rangeCollider != null)
                    {
                        inRange = rangeCollider.OverlapPoint(enemy.transform.position);
                    }
                    else
                    {
                        inRange = IsWithinShootRange(enemy.transform.position);
                    }

                    if (inRange)
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
                if (isHoming)
                {
                    BulletFollowNearestEnemy(projectile);
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
        if (shootRange == null)
        {
            shootRangeActive = false;
            return;
        }

        Collider2D rangeCollider = shootRange.GetComponent<Collider2D>();
        if (rangeCollider == null)
        {
            // Fallback to manual calculation
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            bool enemyInRange = false;
            foreach (GameObject enemy in enemies)
            {
                if (IsWithinShootRange(enemy.transform.position))
                {
                    enemyInRange = true;
                    break;
                }
            }
            shootRangeActive = enemyInRange;
            return;
        }

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.useLayerMask = false; // Adjust if using layers
        Collider2D[] results = new Collider2D[20];
        int count = rangeCollider.Overlap(filter, results);

        bool hasEnemy = false;
        for (int i = 0; i < count; i++)
        {
            if (results[i] != null && results[i].CompareTag("Enemy"))
            {
                hasEnemy = true;
                break;
            }
        }
        shootRangeActive = hasEnemy;
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

    private void BulletFollowNearestEnemy(BulletController projectile)
    {
        if (projectile == null)
            return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float minDistance = Mathf.Infinity;
        Vector3 turretPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(turretPos, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            projectile.SetHomingTarget(nearestEnemy.transform);
        }
        projectile.SetHomingTarget(nearestEnemy.transform);
        projectile.InitializeCurve(FirePoint.position, nearestEnemy.transform.position, trajectoryAnimationCurve, bulletTravelTime, bulletHeight);
    }

    void PlayShootAnimation()
    {
        // Gets turret animator component and plays shoot animation, if it exists
        
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }
        else
        {
            Debug.LogWarning("No Animator component found on turret for shooting animation.");

        }


    }

    public void activateAnimator()
    {
        if (animator != null)
        {
            animator.enabled = true;
        }
    }


}
