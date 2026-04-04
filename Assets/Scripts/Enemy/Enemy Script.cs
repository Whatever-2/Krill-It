using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    [SerializeField] private GameObject Proximity;
    [SerializeField] private float ProximityRange = 5.0f;
    [SerializeField] private Vector2 ProximityOffset = Vector2.zero;
    public bool FollowPlayer = false;


    void Start()
    {
        
        if (Proximity == null)
        {
            Proximity = new GameObject("Proximity");
            Proximity.transform.parent = transform;
            Proximity.transform.localPosition = ProximityOffset;
            

            CircleCollider2D collider = Proximity.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = ProximityRange;

        }
        else
        {
            Proximity.transform.parent = transform;
            Proximity.transform.localPosition = ProximityOffset;

            CircleCollider2D collider = Proximity.GetComponent<CircleCollider2D>();
            if (collider == null)
            {
                collider = Proximity.AddComponent<CircleCollider2D>();
                collider.isTrigger = true;
            }
            collider.radius = ProximityRange;
        }

    }

   
   
    void Update()
    {
        
        

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision = Proximity.GetComponent<Collider2D>();

        if (collision.CompareTag("Player"))
        {
            FollowPlayer = true;
            Debug.Log("Player Detected");
        }else 
        {
            FollowPlayer = false;
            Debug.Log("No Player Detected");
        }

    }

}
