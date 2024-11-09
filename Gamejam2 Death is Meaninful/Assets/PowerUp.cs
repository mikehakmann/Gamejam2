using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, Shield, ExtraLife, DamageBoost }
    public enum PowerUpState { GraveStoneState, HellState }

    public PowerUpType powerUpType;     // Type of the power-up
    public PowerUpState powerUpState;   // Current state of the power-up

    public GameObject GraveStoneSprite; // Reference to the GraveStone visual
    public GameObject HellStateSprite;  // Reference to the HellState visual

    private DamageArea damageArea;      // Reference to the DamageArea component

    private void Start()
    {
        damageArea = GetComponent<DamageArea>();
        UpdatePowerUpState();  // Initialize the correct state on start
    }

    // Method to set the power-up's state and update visuals/effects
    public void SetPowerUpState(PowerUpState newState)
    {
        powerUpState = newState;
        UpdatePowerUpState();
    }

    // Update visuals and effects based on the current state
    private void UpdatePowerUpState()
    {
        if (powerUpState == PowerUpState.GraveStoneState)
        {
            GraveStoneSprite.SetActive(true);
            HellStateSprite.SetActive(false);
            if (damageArea != null) damageArea.enabled = false;
        }
        else if (powerUpState == PowerUpState.HellState)
        {
            GraveStoneSprite.SetActive(false);
            HellStateSprite.SetActive(true);
            if (damageArea != null) damageArea.enabled = true;
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
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.PickUpPowerUp(this);
            Destroy(gameObject);
        }
    }
}
