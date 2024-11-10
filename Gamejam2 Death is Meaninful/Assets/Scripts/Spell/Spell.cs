using UnityEditor.Rendering.Universal;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public GameObject projectile;
    private Upgrade[] upgrades;

    public float damage;
    public int pierce = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upgrades = GameObject.Find("Player").GetComponent<PlayerActions>().upgrades;
        foreach (var upgrade in upgrades)
        {
            if (upgrade.name == "Sniper")
            {
                pierce = 2;
            }
        }

        Destroy(gameObject,4);
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
                    U_SplitFour(collision.gameObject.GetComponent<EnemyHealth>());
                }
              
            }

            if(pierce <= 0)
            {
                Damage(collision.gameObject.GetComponent<EnemyHealth>());
                Destroy(gameObject);
            }
            else
            {
                pierce--;
            }
        }
    }
  
    float splitSpeed = 2f;
    bool hasSpawned = false;
    private void U_SplitFour(EnemyHealth enemyHit)
    {
        if (hasSpawned)
        {
            return;
        }

        // Define base angles for each projectile direction (in degrees)
        float[] baseAngles = { 0f, 90f, 180f, 270f };

        foreach (float baseAngle in baseAngles)
        {
            // Add a random offset between -10 and +10 degrees
            float randomOffset = Random.Range(-10f, 10f);
            float angle = baseAngle + randomOffset;

            // Create rotation with the modified angle
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            // Instantiate the projectile at this rotation
            GameObject spell = Instantiate(projectile, transform.position, rotation);

            // Calculate the direction based on the adjusted rotation
            Vector2 direction = rotation * Vector2.up;
            spell.GetComponent<Rigidbody2D>().linearVelocity = direction * splitSpeed;
        }

        hasSpawned = true;
    }



    private void Damage(EnemyHealth enemy)
    {

        if (enemy == null)
        enemy.TakeDamage(damage);
    }


}
