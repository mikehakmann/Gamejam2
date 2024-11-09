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
    private void U_SplitFour()
    {
        if (hasSpawned)
        {
            return;
        }

        // Define angles for each projectile direction (in degrees)
        float[] angles = { 0f, 90f, 180f, 270f };

        foreach (float angle in angles)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            GameObject spell = Instantiate(projectile, transform.position, rotation);

            Vector2 direction = rotation * Vector2.up;
            spell.GetComponent<Rigidbody2D>().linearVelocity = direction * splitSpeed;

        }

        hasSpawned = true;
    }
    void U_SpawnTwo()
    {
    }
    void U_SpawnThree()
    {
    }


    private void Damage(EnemyHealth enemy)
    {

        if (enemy == null)
        enemy.TakeDamage(damage);
    }
}
