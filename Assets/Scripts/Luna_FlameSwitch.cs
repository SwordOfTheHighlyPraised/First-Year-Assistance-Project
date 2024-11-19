using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luna_FlameSwitch : MonoBehaviour
{
    [Header("Assign the flame components")]
    [SerializeField] private ParticleSystem flameEmitter; // Reference to the particle system
    [SerializeField] private BoxCollider flameCollider;   // Reference to the flame collider

    private bool flamesOn = false; // Tracks if the flames are on
    private bool playerInTrigger = false; // Tracks if the player is in the switch's trigger area

    private void Start()
    {
        if (flameEmitter == null || flameCollider == null)
        {
            Debug.LogError("Flame Emitter or Flame Collider is not assigned!");
            return;
        }

        flameEmitter.Stop(); // Ensure the flames are off initially
        flameCollider.enabled = flamesOn; // Disable the collider initially
    }

    private void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E)) // Press E only if in trigger
        {
            flamesOn = !flamesOn;

            if (flamesOn)
            {
                flameEmitter.Play(); // Start the particle system
            }
            else
            {
                flameEmitter.Stop(); // Stop the particle system
            }

            flameCollider.enabled = flamesOn; // Enable/Disable the collider

            Debug.Log("Flames switched " + (flamesOn ? "ON" : "OFF"));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true; // Player entered the trigger area
            Debug.Log("Player entered FlameSwitch trigger area.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false; // Player left the trigger area
            Debug.Log("Player exited FlameSwitch trigger area.");
        }
    }
}
