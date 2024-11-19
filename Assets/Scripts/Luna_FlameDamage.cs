using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luna_FlameDamage : MonoBehaviour
{
    [Header("Flame Damage Settings")]
    [SerializeField] private int damage = 10;           // Damage dealt per tick
    [SerializeField] private float damageInterval = 3f; // Time in seconds between damage ticks

    private float nextDamageTime = 0f; // Tracks when the next damage can occur

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the collider belongs to the player
        {
            if (Time.time >= nextDamageTime)
            {
                DealDamage(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nextDamageTime = 0f; // Reset the timer when the player exits
        }
    }

    private void DealDamage(Collider player)
    {
        Luna_PlayerHealth playerHealth = player.GetComponent<Luna_PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage); // Apply damage to the player
            Debug.Log("Player took " + damage + " damage from flames.");
            nextDamageTime = Time.time + damageInterval; // Set the next damage time
        }
    }
}
