using UnityEngine;

public class PlayerActions : MonoBehaviour
{


    public GameObject projectile;
    private Transform ShootSpawn;
    private float distanceFromPlayer = 0.2f;
    public float projectileSpeed = 5f;

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

        Vector3 direction = (mousePos - transform.position).normalized;

        ShootSpawn.position = transform.position + direction * distanceFromPlayer;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        ShootSpawn.rotation = Quaternion.Euler(0, 0, angle-90);
    }

    public void Shoot()
    {
        GameObject Spell = Instantiate(projectile, ShootSpawn.position, ShootSpawn.rotation);

        Spell.GetComponent<Rigidbody2D>().linearVelocity = ShootSpawn.up * projectileSpeed;  // Adjust speed as needed
    }
}
