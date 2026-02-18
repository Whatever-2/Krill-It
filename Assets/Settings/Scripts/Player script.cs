using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Transform FirePoint;
    public GameObject bulletPrefab;
    public PlayerInput playerInput;

    private InputAction rollAction;
    private InputAction interactAction;
    private InputAction moveAction;
    private InputAction shootAction;

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

        // Horizontal movement
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        // Shoot
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Shoot();
        }

        // interact
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Interact();
        }

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            Roll();
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
        Instantiate(bulletPrefab, FirePoint.position, FirePoint.rotation);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Triangle"))
        {
            Destroy(gameObject);
        }
    }

}