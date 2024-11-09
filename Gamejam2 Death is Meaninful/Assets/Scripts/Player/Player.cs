using UnityEngine;

public class Player : MonoBehaviour
{
    //singleton
    public static Player instance;


    public float moveSpeed = 5f;  

    private Rigidbody2D rb;       
    private Vector2 movement;     

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
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
