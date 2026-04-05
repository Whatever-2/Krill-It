using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    public int maxHealth = 20;
    public int currentHealth;
    public Camera mainCamera;
    public GameObject bulletPrefab;
    public PlayerInput playerInput;
    
    [SerializeField] GameObject shootRange;
    [SerializeField] private float shootRangeDistance = 5f; // Distance for the shoot range indicator
    [SerializeField] private Vector2 shootRangeScale = Vector2.one; // X/Y scale multipliers for the range shape: (1,1)=circle
    [SerializeField] private bool shootRangeActive = true; // Flag to track if the shoot range indicator is active
   

    public Transform FirePoint;
    
    private InputAction rollAction;
    private InputAction interactAction;
    private InputAction moveAction;
    private InputAction shootAction;


    private Vector2 LastDirection;
    private Vector2 velocity;
    public float speed = 5f;
    public float RollDist = 12f;   //roll distance

    private Vector2 direction;
    private Rigidbody2D rb;

    private bool isGrounded = true;

    //experimenting on enableable targeting
    [SerializeField] private bool isTurret = false;



    //bullet
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
        if(!isTurret)
        {
        rollAction = playerInput.actions.FindAction("Roll");
        interactAction = playerInput.actions.FindAction("Interact");
        moveAction = playerInput.actions.FindAction("Move");
        shootAction = playerInput.actions.FindAction("Shoot");
        }
    }

    private void Update()
    {
        if (!isTurret)
        {
            direction = moveAction.ReadValue<Vector2>();

        if (moveAction.IsPressed())
        {
            LastDirection = direction;
        }else if (moveAction.WasReleasedThisFrame())
        {
            LastDirection = direction;
        }

        velocity = speed * direction * Time.deltaTime;

        transform.Translate(velocity);

        // Flip the player sprite based on movement direction
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // interact
        if (interactAction.WasPressedThisFrame() )
        {
            Interact();
        }

        if (rollAction.WasPressedThisFrame() && isGrounded)
        {
            Roll();
        }

        }
        
        // Shoot
    
        timer -= Time.deltaTime; // Decrease the timer by the time elapsed since the last frame

        //null check for shoot action since it is not used for turret behavior
        if (shootAction == null)
        {
            shootAction = playerInput.actions.FindAction("Shoot");
        }
        if (!isTurret ) //player behavior shooting when mouse is clicked
        {
            if (shootAction.WasPressedThisFrame() )
            {
                if (timer < 0)
                {
                    Shoot();
                    resetTimer(); // Reset the timer after shooting
                }   
            }
        }else if (isTurret) //setup for turret behavior auto shooting based on shooter timer
        {
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
       

    }

    public void Roll()
    {
        Vector2 rollDirection = new Vector2(transform.localScale.x, 0).normalized;
        rb.AddForce(rollDirection * RollDist, ForceMode2D.Impulse);
        isGrounded = false;
    }


    public void Interact()
    {
        // Implement interaction logic here
    }

  public void Shoot()
    {
        var go = Instantiate(bulletPrefab, FirePoint.position, FirePoint.rotation);
        var projectile = go.GetComponent<BulletController>();
        
        
        if (!isTurret)
        {
            if (projectile != null)
            {
                Vector3 mouseScreen = Mouse.current.position.ReadValue();
                float zDist = transform.position.z - (mainCamera != null ? mainCamera.transform.position.z : 0f);
                Vector3 mouseWorld = (mainCamera != null)
                    ? mainCamera.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, zDist))
                    : new Vector3(mouseScreen.x, mouseScreen.y, transform.position.z);
                mouseWorld.z = FirePoint.position.z;
                
                projectile.InitializeCurve(FirePoint.position, mouseWorld, trajectoryAnimationCurve, bulletTravelTime, bulletHeight);
            }
        }else if (isTurret)
        {   //TARGETS NEAREST ENEMY INSTEAD OF MOUSE POSITION
         
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }else
        {
            isGrounded = false;
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

    private void resetTimer()
    {
        timer = shooterTimer;
    }


}