using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    //singleton
    public static PlayerActions instance;


    public Upgrade[] upgrades;

    public GameObject projectile;
    public GameObject bigProjectile;
    private Transform ShootSpawn;
    private float distanceFromPlayer = 0.2f;
    public float projectileSpeed = 5f;

    void Start()
    {
        ShootSpawn = transform.Find("ShootSpawn");
        instance = this;

    }

    void Update()
    {
        MoveSpawnTowardsMouse();

        if (Input.GetMouseButtonDown(0))
        {
            foreach (var upgrade in upgrades)
            {
                if (upgrade.name == "ShootTwo")
                {
                    ShootTwo();
                    break;
                }
                else if (upgrade.name == "ShootThree")
                {
                    ShootThree();
                    break;
                }
                else
                {
                    Shoot();
                    break;

                }
            }
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

        Debug.Log("Shooting normal");
    }

    public float shootingAngle = 20f;
    private void ShootTwo()
    {
        //rotate the shootspawn by -45
        ShootSpawn.Rotate(0, 0, -shootingAngle);
        GameObject Spell1 = Instantiate(bigProjectile, ShootSpawn.position, ShootSpawn.rotation);
        Spell1.GetComponent<Rigidbody2D>().linearVelocity = ShootSpawn.up * projectileSpeed;

        ShootSpawn.Rotate(0, 0, shootingAngle*2);
        GameObject Spell2 = Instantiate(bigProjectile, ShootSpawn.position, ShootSpawn.rotation);
        Spell2.GetComponent<Rigidbody2D>().linearVelocity = ShootSpawn.up * projectileSpeed;

        Debug.Log("Shooting two");
    }
    private void ShootThree()
    {

        Debug.Log("Shooting three");
    }
}
