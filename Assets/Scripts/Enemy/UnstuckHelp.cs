using UnityEngine;

public class UnstuckHelp : MonoBehaviour
{
    //checks enemies every 2 seconds if they are stuck and if so, makes them move upwards to get them unstuck
    private GameObject toCheck;
    private bool isStuck = false;
    private float checkInterval;
    private float checkTimer = 2f;
    private Vector3 lastPosition;
    private Vector3 currentPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        checkInterval = checkTimer;
        toCheck = gameObject; // Set the object to check as the current game object
        lastPosition = toCheck.transform.position; // Initialize last position
        currentPosition = toCheck.transform.position; // Initialize current position
        UpdateCurrPosition();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrPosition();
        if (toCheck != null)
        {
            checkTimer -= Time.deltaTime;
            if (checkTimer <= 0f)
            {
                isStuck = CheckCurrentPosition();
                if (isStuck)
                {
                    Unstuck();
                }
                checkTimer = checkInterval; // Reset the timer for the next check
            }
        }

    }

    private bool CheckCurrentPosition()
    {
        if (toCheck != null)
        {
            // Check if the object is stuck by comparing its current position to a previous position
            Vector3 currentPosition = toCheck.transform.position;
            if (currentPosition == toCheck.transform.position)
            {
                return true; // The object is stuck
            }
        }
        return false; // The object is not stuck
        
    }

    private void Unstuck()
    {
        if (lastPosition == currentPosition)
        {
            // Move the object upwards to try to get it unstuck
            toCheck.transform.position += new Vector3(0, 1, 0);
        }
        else
        {
            // Update the last position to the current position for the next check
            lastPosition = currentPosition;
        }
    }

    private void UpdateCurrPosition()
    {
        if (toCheck != null)
        {
            currentPosition = toCheck.transform.position;
        }
    }

}
