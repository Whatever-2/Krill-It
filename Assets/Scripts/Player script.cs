using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject bulletPrefab;
    public PlayerInput playerInput;
    
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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        
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

        // Shoot
        if (shootAction.WasPressedThisFrame())
        {
            Shoot();
        }

        // interact
        if (interactAction.WasPressedThisFrame())
        {
            Interact();
        }

        if (rollAction.WasPressedThisFrame() )
        {
            Roll();
        }

        //firepoint to mouse position
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 firePointDirection = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(firePointDirection.y, firePointDirection.x) * Mathf.Rad2Deg;
        FirePoint.rotation = Quaternion.Euler(0, 0, angle);


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
        Instantiate(bulletPrefab, FirePoint.position, FirePoint.rotation);
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

}