using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    public float rollCooldown = 1f;

    public float rollDuration = 0.2f; //Roll duration on how fast it moves
    public int maxHealth = 20;
    public int currentHealth;
    public Camera mainCamera;
    public GameObject bulletPrefab;
    public PlayerInput playerInput;
    public float speed = 5f;
    public float RollDist = 12f;   //roll distance

    [SerializeField] GameObject shootRange;
    [SerializeField] private float shootRangeDistance = 5f; // Distance for the shoot range indicator
    [SerializeField] private Vector2 shootRangeScale = Vector2.one; // X/Y scale multipliers for the range shape: (1,1)=circle
    [SerializeField] private bool shootRangeActive = true; // Flag to track if the shoot range indicator is active
   
    private Animator animator;

    public Transform FirePoint;

    private bool isRolling = false; //to check if its rolling
    private float rollDurationTimer; //to check how long the roll has been active
    private InputAction rollAction;
    private InputAction interactAction;
    private InputAction moveAction;
    private InputAction shootAction;
    private float rollTimer;

    private Vector2 LastDirection;
    private Vector2 velocity;

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
        animator = GetComponent<Animator>();
        
    }

    private void Update()
    {
        rollTimer -= Time.deltaTime;


/*

        direction = moveAction.ReadValue<Vector2>();

        if (moveAction.IsPressed())
        {
            LastDirection = direction;
        }else if (moveAction.WasReleasedThisFrame())
        {
            LastDirection = direction;
        }


*/
        MoveInput();


        //to check if the player is rolling and update the timer
        if (!isRolling)
        {
            rb.linearVelocity = direction * speed;
        }

        // interact
        if (interactAction.WasPressedThisFrame() )
        {
            Interact();
        }

        //roll
        if (rollAction.WasPressedThisFrame() && isGrounded && rollTimer <= 0f && !isRolling)
        {
            Roll();
        }

        if (isRolling) //check if the player is rolling and update the timer
        {
            rollDurationTimer -= Time.deltaTime;

            if (rollDurationTimer <= 0f)
            {
                isRolling = false;
            }
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


    private void MoveInput()
    {
        //=====================Player Input=======================//
        direction = moveAction.ReadValue<Vector2>();

        if (moveAction.IsPressed())
        {
            animator.SetBool("isMoving", true);
            LastDirection = direction;

            animator.SetFloat("InputX", direction.x);
            animator.SetFloat("InputY", direction.y);
            animator.SetFloat("LastInputX", LastDirection.x);
            animator.SetFloat("LastInputY", LastDirection.y);

        }else if (moveAction.WasReleasedThisFrame())
        {
            animator.SetFloat("LastInputX", LastDirection.x);
            animator.SetFloat("LastInputY", LastDirection.y);
            animator.SetBool("isMoving", false);
        }

        velocity = speed * direction * Time.deltaTime;

        transform.Translate(velocity);
        //========================================================//
    }

    public void Roll() //to make the player roll and checks the direction of the roll and applies the velocity to the player
    {
        isRolling = true;
        rollTimer = rollCooldown;
        rollDurationTimer = rollDuration;

        Vector2 rollDirection = LastDirection.normalized;

        if (rollDirection == Vector2.zero)
            rollDirection = Vector2.right * transform.localScale.x;

        rb.linearVelocity = rollDirection * RollDist; // use velocity instead of force
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
                animator.SetTrigger("Attack");
                Vector3 mouseScreen = Mouse.current.position.ReadValue();
                float zDist = transform.position.z - (mainCamera != null ? mainCamera.transform.position.z : 0f);
                Vector3 mouseWorld = (mainCamera != null)
                    ? mainCamera.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, zDist))
                    : new Vector3(mouseScreen.x, mouseScreen.y, transform.position.z);
                mouseWorld.z = FirePoint.position.z;
                
                projectile.InitializeCurve(FirePoint.position, mouseWorld, trajectoryAnimationCurve, bulletTravelTime, bulletHeight);
            }
    }

    private void OnCollisionEnter2D(Collision2D collision) //to check if the player is grounded and can roll(updated because the last one causes the roll to break)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
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