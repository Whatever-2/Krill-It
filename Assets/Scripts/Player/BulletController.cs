using UnityEngine;
using UnityEngine.InputSystem;

public class BulletController : MonoBehaviour
{
    //PlayerBulletController

    private Camera mainCam;
    private Rigidbody2D rb;

    [Header("Legacy linear shot")]
    public float force;
    public float DestroyDistance = 20f;

    [Header("Curve trajectory")]
    [SerializeField] private AnimationCurve trajectoryCurve;
    [SerializeField] private float travelTime = 0.7f;
    [SerializeField] private float heightScale = 1f;
    
    private Transform HomingTarget; // Store the current homing target
    private Vector3 startPos;
    private Vector3 direction;  // stores target position for lerp
    private float elapsed;
    // Track last movement direction and rotation
    private Vector3 lastDirection = Vector3.zero;
    private Quaternion lastRotation = Quaternion.identity;
    private Vector3 lastVelocity = Vector3.zero;
    private bool useCurve = true;
    public bool isTurretBullet = false;



    void Awake()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (useCurve)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / travelTime);
            
            // Lerp from start to target position (direction variable holds target)
            Vector3 flatPos = Vector3.Lerp(startPos, direction, t);
            
            // Apply curve offset
            float curveValue = trajectoryCurve.Evaluate(t);
            Vector3 offset = Vector3.up * curveValue * heightScale;
            Vector3 newPos = flatPos + offset;
            
            // Calculate velocity for rotation
            if (t < 1f)
            {
                float tNext = Mathf.Clamp01((elapsed + Time.deltaTime) / travelTime);
                Vector3 flatNext = Vector3.Lerp(startPos, direction, tNext);
                float curveNext = trajectoryCurve.Evaluate(tNext);
                Vector3 nextPos = flatNext + Vector3.up * curveNext * heightScale;
                Vector3 vel = nextPos - newPos;
                
                if (vel.sqrMagnitude > 1e-6f)
                {
                    float rot = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, rot);
                }
            }
            
            transform.position = newPos;

            // Destroy when animation completes
            if (t >= 1f)
            {
                if (isTurretBullet)
                {
                    transform.position = direction;  // Ensure turret bullet reaches target position
                }
                Destroy(gameObject);
            }
        }
        else
        {
            if (transform.position.magnitude > DestroyDistance)
            {
                Destroy(gameObject);
            }
        }
        if (HomingTarget != null)
        {
            direction = HomingTarget.position; // Update direction to homing target position
        }
    }

    public void SetHomingTarget(Transform target)
    {
        HomingTarget = target;
    }
    public void InitializeCurve(Vector3 start, Vector3 targetPosition, AnimationCurve curve, float travelTimeSec, float heightScaleMultiplier)
    {
        startPos = start;
        direction = targetPosition;  // store target position for lerp
        trajectoryCurve = curve != null ? curve : AnimationCurve.Linear(0, 0, 1, 0);
        travelTime = Mathf.Max(0.01f, travelTimeSec);
        heightScale = heightScaleMultiplier;
        elapsed = 0f;
        useCurve = true;
        //curveFinished = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        transform.position = startPos;
    }

    public void InitializeAnimaitonCurve(AnimationCurve curve)
    {
        trajectoryCurve = curve != null ? curve : AnimationCurve.Linear(0, 0, 1, 0);
    }

    public Vector3 GetLastDirection()
    {
        return lastDirection;
    }
    
    public Quaternion GetLastRotation()
    {
        return lastRotation;
    }
    
    /// <summary>
    /// Get the last recorded velocity of the bullet.
    /// </summary>
    public Vector3 GetLastVelocity()
    {
        return lastVelocity;
    }
}
