using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;  

    private Rigidbody2D rb;       
    private Vector2 movement;

    // Private list to store picked-up power-ups, but visible in the Inspector
    [SerializeField]
    private List<PowerUp> pickedUpPowerUps = new List<PowerUp>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;  
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    // Method to handle power-up pickup
    public void PickUpPowerUp(PowerUp powerUp)
    {
        pickedUpPowerUps.Add(powerUp);  // Add the power-up to the list
        Debug.Log("Picked up power-up: " + powerUp.powerUpType);
    }
}