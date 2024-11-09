using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    private Camera mainCamera;
    private float screenHalfWidth;
    private float screenHalfHeight;
    private float objectRadius;

    void Start()
    {
        // Get the main camera in the scene
        mainCamera = Camera.main;

        // Calculate the half-screen width and height in world units
        screenHalfHeight = mainCamera.orthographicSize;
        screenHalfWidth = screenHalfHeight * mainCamera.aspect;

        // Calculate the radius of the object using the CircleCollider2D's radius
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            objectRadius = collider.radius * transform.localScale.x; // Account for object scaling
        }
        else
        {
            Debug.LogError("ScreenWrap script requires a CircleCollider2D component.");
        }
    }

    void Update()
    {
        // Check if the object is out of bounds and teleport it to the opposite side if needed
        Vector3 newPosition = transform.position;

        // Check horizontal wrapping
        if (transform.position.x > screenHalfWidth + objectRadius)
        {
            newPosition.x = -screenHalfWidth - objectRadius;
        }
        else if (transform.position.x < -screenHalfWidth - objectRadius)
        {
            newPosition.x = screenHalfWidth + objectRadius;
        }

        // Check vertical wrapping
        if (transform.position.y > screenHalfHeight + objectRadius)
        {
            newPosition.y = -screenHalfHeight - objectRadius;
        }
        else if (transform.position.y < -screenHalfHeight - objectRadius)
        {
            newPosition.y = screenHalfHeight + objectRadius;
        }

        // Apply the new position
        transform.position = newPosition;
    }
}