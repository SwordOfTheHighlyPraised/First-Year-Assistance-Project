using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luna_PlayerHealth : MonoBehaviour
{
    [Header("Player Health Settings")]
    [SerializeField] private int maxHealth = 100; // Maximum health
    [SerializeField] private int currentHealth; // Current health of the player

    [Header("UI Elements")]
    [SerializeField] private Luna_HealthBar healthBar; // Reference to a health bar UI script (optional)

    private bool isDead = false; // Tracks if the player is dead

    void Start()
    {
        currentHealth = maxHealth; // Initialize health
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth); // Update health bar UI
        }
    }

    void Update()
    {
        // Check for the 'I' key press and reduce health by 10
        if (Input.GetKeyDown(KeyCode.I))
        {
            TakeDamage(10); // Reduce health by 10
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Ignore damage if the player is already dead

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health stays within valid range

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth); // Update health bar UI
        }

        Debug.Log("Player took damage: " + damage + ". Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        if (isDead) return; // Dead players cannot heal

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health stays within valid range

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth); // Update health bar UI
        }

        Debug.Log("Player healed: " + healAmount + ". Current health: " + currentHealth);
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player has died.");
        // Trigger death animations, disable player input, etc.
    }

    public void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth); // Update health bar UI
        }

        Debug.Log("Player respawned. Health reset to max.");
    }
}
