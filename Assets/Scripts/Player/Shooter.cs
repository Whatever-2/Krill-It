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

        rollAction = playerInput.actions.FindAction("Roll");
        interactAction = playerInput.actions.FindAction("Interact");
        moveAction = playerInput.actions.FindAction("Move");
        shootAction = playerInput.actions.FindAction("Shoot");
        
    }

    private void Update()
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
    
     // Shoot
    
        timer -= Time.deltaTime; // Decrease the timer by the time elapsed since the last frame

        //null check for shoot action since it is not used for turret behavior
        if (shootAction == null)
        {
            shootAction = playerInput.actions.FindAction("Shoot");
        }
            if (shootAction.WasPressedThisFrame() )
            {
                if (timer < 0)
                {
                    Shoot();
                    resetTimer(); // Reset the timer after shooting
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