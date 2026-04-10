using UnityEngine;


public class Wall : MonoBehaviour
{
    //Detects if the player is colliding with the wall and if so, it will prevent further movement through object
    [SerializeField] private bool isSolid = true;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isSolid)
            {
                // Prevent the player from moving through the wall
                Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                if (playerRigidbody != null)
                {
                    playerRigidbody.linearVelocity = Vector3.zero; // Stop the player's movement
                }
                
            }
        }
    }

}
