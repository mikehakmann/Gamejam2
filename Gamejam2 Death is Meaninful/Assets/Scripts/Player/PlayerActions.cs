using UnityEngine;

public class PlayerActions : MonoBehaviour
{


    public GameObject projectile;
    private Transform ShootSpawn;
    private float distanceFromPlayer = 0.2f;
    private float projectileSpeed = 10f;

    void Start()
    {
        ShootSpawn = transform.Find("ShootSpawn");
    }

    void Update()
    {
        MoveSpawnTowardsMouse();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void MoveSpawnTowardsMouse()
    {
        // Get the mouse position in world coordinates
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;  // Set z to 0 since we're in 2D

        // Calculate the direction from the player to the mouse
        Vector3 direction = (mousePos - transform.position).normalized;

        // Set ShootSpawn position based on the calculated direction and distance from the player
        ShootSpawn.position = transform.position + direction * distanceFromPlayer;

        // Rotate ShootSpawn to face the mouse by setting its rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        ShootSpawn.rotation = Quaternion.Euler(0, 0, angle-90);
    }

    public void Shoot()
    {
        // Instantiate the projectile at ShootSpawn's position and rotation
        GameObject Spell = Instantiate(projectile, ShootSpawn.position, ShootSpawn.rotation);

        // Set the projectile's velocity in the direction ShootSpawn is facing
        Spell.GetComponent<Rigidbody2D>().linearVelocity = ShootSpawn.up * projectileSpeed;  // Adjust speed as needed
    }
}
