using UnityEditor.Rendering.Universal;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public GameObject projectile;
    public Upgrade[] upgrades;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the spell collides with an enemy
        if (collision.gameObject.tag == "Enemy")
        {

            foreach (var upgrade in upgrades)
            {
                if(upgrade.name == "SplitFour")
                {
                    U_SplitFour();
                }
                // if (upgrade.name == "SpawnTwo")
                //{
                //    U_SpawnTwo();
                //}
                // if (upgrade.name == "SpawnThree")
                //{
                //    U_SpawnThree();
                //}


            }
            // Destroy the spell
            Destroy(gameObject);
        }
    }
  
    float splitSpeed = 2f;
    bool hasSpawned = false;
    private void U_SplitFour()
    {
        if (hasSpawned)
        {
            return;
        }
        // Split the spell into four directions
        // Instantiate the projectile at ShootSpawn's position and rotation
        GameObject Spell1 = Instantiate(projectile, transform.position, transform.rotation);
        GameObject Spell2 = Instantiate(projectile, transform.position, transform.rotation);
        GameObject Spell3 = Instantiate(projectile, transform.position, transform.rotation);
        GameObject Spell4 = Instantiate(projectile, transform.position, transform.rotation);
        //turn off all spell scripts on the spells

        Spell1.GetComponent<Rigidbody2D>().linearVelocity = transform.up * splitSpeed;      // Up
        Spell2.GetComponent<Rigidbody2D>().linearVelocity = transform.right * splitSpeed;   // Right
        Spell3.GetComponent<Rigidbody2D>().linearVelocity = -transform.up * splitSpeed;     // Down
        Spell4.GetComponent<Rigidbody2D>().linearVelocity = -transform.right * splitSpeed;  // Left
        hasSpawned = true;

    }
    void U_SpawnTwo()
    {
    }
    void U_SpawnThree()
    {
    }
}
