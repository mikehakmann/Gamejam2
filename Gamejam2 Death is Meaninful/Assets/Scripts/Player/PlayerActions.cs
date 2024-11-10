using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    //singleton
    public static PlayerActions instance;

    public List<Upgrade> upgrades = new List<Upgrade>();

    public GameObject projectile;
    public GameObject bigProjectile;
    private Transform ShootSpawn;
    private float distanceFromPlayer = 0.2f;
    public float projectileSpeed = 5f;

    private float cooldownTime;
    private float nextShootTime = 0f;

    //upgradeable stats
    public float damage = 1f;
    public float speed = 5;
    public float fireRate = 1;
    public float maxHP = 100;

    void Start()
    {
        ShootSpawn = transform.Find("ShootSpawn");
        instance = this;
        speed = GetComponent<Player>().moveSpeed; 
        cooldownTime = 1 / fireRate;
    }

    void Update()
    {
        MoveSpawnTowardsMouse();

        // Update cooldownTime based on current fireRate
        cooldownTime = 1 / fireRate;

        if (Input.GetMouseButtonDown(0) && Time.time >= nextShootTime)
        {
            nextShootTime = Time.time + cooldownTime; // Set the time for the next shot

            foreach (var upgrade in upgrades)
            {
                if (upgrade.name == "ShootTwo")
                {
                    ShootTwo();
                    break;
                }
               
                if (upgrade.name == "ShootThree")
                {
                    ShootThree();
                    break;
                }

                Shoot();
                return;

            }

            if(upgrades.Count == 0)
                Shoot();
        }
    }
    public void Upgrade(Upgrade upgrade)
    {
        // Add the upgrade to the list
        upgrades.Add(upgrade);

        // Update the player's stats based on the upgrade
        UpdateUpgrades();
    }

    public void UpdateUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            if (upgrade.name == "Damage")
            {
                damage += 5;
            }
            else if (upgrade.name == "Speed")
            {
                speed += 1;
            }
            else if (upgrade.name == "FireRate")
            {
                fireRate += 0.2f;
            }
            else if (upgrade.name == "MaxHP")
            {
                maxHP += 10;
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
        ShootSpawn.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    public void Shoot()
    {
        AudioManager.instance.Play("Cast");

        GameObject Spell = Instantiate(projectile, ShootSpawn.position, ShootSpawn.rotation);

        Spell.GetComponent<Rigidbody2D>().linearVelocity = ShootSpawn.up * projectileSpeed;  // Adjust speed as needed

        Debug.Log("Shooting normal");
    }

    public float shootingAngle = 20f;
    private void ShootTwo()
    {
        AudioManager.instance.Play("Cast");
        //rotate the shootspawn by -45
        ShootSpawn.Rotate(0, 0, -shootingAngle);
        GameObject Spell1 = Instantiate(bigProjectile, ShootSpawn.position, ShootSpawn.rotation);
        Spell1.GetComponent<Rigidbody2D>().linearVelocity = ShootSpawn.up * projectileSpeed;

        ShootSpawn.Rotate(0, 0, shootingAngle * 2);
        GameObject Spell2 = Instantiate(bigProjectile, ShootSpawn.position, ShootSpawn.rotation);
        Spell2.GetComponent<Rigidbody2D>().linearVelocity = ShootSpawn.up * projectileSpeed;

        Debug.Log("Shooting two");
    }
    private void ShootThree()
    {
        AudioManager.instance.Play("Cast");

        Debug.Log("Shooting three");
    }

    public void PickUpPowerUp(EnemyDrop enemyDrop)
    {
        upgrades.Add(enemyDrop.upgrade);  // Add the power-up to the list
    }


}
