using UnityEngine;

public class SpellSplit : MonoBehaviour
{

    public float damage;
    public int pierce = 0;
    private PlayerActions player;
    private bool firstHitIgnored = false; // Track if the first enemy hit should be ignored
    private GameObject firstEnemyHit;     //
    private void Start()
    {
        player = PlayerActions.instance;
        Destroy(gameObject, 4);
        if (player != null)
        {
            foreach (var upgrade in player.upgrades)
            {
                if (upgrade.name == "Sniper")
                {
                    pierce = 2;
                }
            }
        }
    }


  
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the spell collides with an enemy
        if (collision.gameObject.tag == "Enemy")
        {
            if (!firstHitIgnored)
            {
                // Record the first enemy hit and ignore it
                firstEnemyHit = collision.gameObject;
                firstHitIgnored = true;
                Debug.Log("Ignoring first enemy hit: " + firstEnemyHit.name);
                return; // Exit the method without applying damage or decreasing pierce
            }

            if (pierce <= 0)
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

    private void Damage(EnemyHealth enemy)
    {

        if (enemy == null)
            enemy.TakeDamage(damage);
    }
}
