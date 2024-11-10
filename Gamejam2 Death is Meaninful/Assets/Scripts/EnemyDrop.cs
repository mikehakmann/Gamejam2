using System;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, Shield, ExtraLife, DamageBoost }
    public enum PowerUpState { GraveStoneState, HellState }

    public PowerUpType powerUpType;           // Type of the power-up
    public PowerUpState powerUpState;         // Current state of the power-up

    public GameObject GraveStoneSprite;       // Reference to the GraveStone visual
    public GameObject HellStateSprite;        // Reference to the HellState visual

    private DamageArea damageArea;            // Reference to the DamageArea component
    private Collider2D powerUpCollider;       // Reference to the Collider component

    public bool canBePickedUp = false;         // Flag to check if the power-up can be picked up

    private void Start()
    {
        damageArea = GetComponent<DamageArea>();
        powerUpCollider = GetComponent<Collider2D>();

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
        if(canBePickedUp == false) return;
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.PickUpPowerUp(this);
            Destroy(gameObject);
        }
    }
}
