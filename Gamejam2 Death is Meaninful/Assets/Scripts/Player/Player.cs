using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    // Singleton instance
    public static Player instance;

    public float moveSpeed = 5f;
    public Transform spriteTransform;      // Reference to the sprite's Transform for rotation

    private Rigidbody2D rb;
    private Vector2 movement;

    // Private list to store picked-up power-ups, visible in the Inspector
    [SerializeField]
    private List<EnemyDrop> pickedUpPowerUps = new List<EnemyDrop>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        instance = this;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        // Rotate the sprite based on the movement direction
        if (movement != Vector2.zero && spriteTransform != null)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            spriteTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    
}