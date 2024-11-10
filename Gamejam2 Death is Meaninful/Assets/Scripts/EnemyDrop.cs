using System.Collections.Generic;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{

    public Upgrade upgrade;
    public enum PowerUpState { GraveStoneState, HellState }

    public PowerUpState powerUpState;         // Current state of the power-up

    public GameObject GraveStoneSprite;       // Reference to the GraveStone visual
    public GameObject HellStateSprite;        // Reference to the HellState visual

    private DamageArea damageArea;            // Reference to the DamageArea component
    private Collider2D powerUpCollider;       // Reference to the Collider component

    public bool canBePickedUp = false;
    
    private void Start()
    {
        // Ensure the power-up starts in GraveStoneState
        powerUpState = PowerUpState.GraveStoneState;

        upgrade = GetRandomUpgrade();

        // Get the DamageArea and Collider components
        damageArea = GetComponent<DamageArea>();
        powerUpCollider = GetComponent<Collider2D>();

        // Initialize the correct visuals, effects, and collider based on GraveStoneState
        UpdatePowerUpState();
    }

    private Upgrade GetRandomUpgrade()
    {
         List<Upgrade> upgrades = GameManager.Instance.upgradesAvailiable;
        int randomIndex = Random.Range(0, upgrades.Count);
        return upgrades[randomIndex];
    }

    // Method to set the power-up's state and update visuals/effects
    public void SetPowerUpState(PowerUpState newState)
    {
        powerUpState = newState;
        UpdatePowerUpState();
    }

    // Update visuals, effects, and collider based on the current state
    private void UpdatePowerUpState()
    {
        if (powerUpState == PowerUpState.GraveStoneState)
        {
            // Activate GraveStone visual and disable HellState visual
            GraveStoneSprite.SetActive(true);
            HellStateSprite.SetActive(false);

            // Disable damage area and collider in GraveStoneState
            if (damageArea != null) damageArea.enabled = false;
            if (powerUpCollider != null) powerUpCollider.enabled = false;
        }
        else if (powerUpState == PowerUpState.HellState)
        {
            // Activate HellState visual and disable GraveStone visual
            GraveStoneSprite.SetActive(false);
            HellStateSprite.SetActive(true);

            // Enable damage area and collider in HellState
            if (damageArea != null) damageArea.enabled = true;
            if (powerUpCollider != null) powerUpCollider.enabled = true;
        }
    }

    // Method to toggle the state (used when the player dies)
    public void ChangeState()
    {
        if (powerUpState == PowerUpState.GraveStoneState)
        {
            SetPowerUpState(PowerUpState.HellState);
        }
        else
        {
            SetPowerUpState(PowerUpState.GraveStoneState);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!canBePickedUp) {
            return;
        }
        PlayerActions player = other.GetComponent<PlayerActions>();
        if (player != null)
        {
            player.PickUpPowerUp(this);
            Destroy(gameObject);
        }
    }
}
