using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luna_LightSwitch : MonoBehaviour
{
    [Header("Assign the light inside the box")]
    [SerializeField] private Light boxLight; // Reference to the light inside the box
    private bool isLightOn = false; // Tracks the current state of the light, initially off
    private bool playerInTrigger = false; // Tracks if the player is in the switch's trigger area

    private void Start()
    {
        if (boxLight == null)
        {
            Debug.LogError("Box Light is not assigned!");
            return;
        }

        boxLight.enabled = isLightOn; // Ensure the light starts in the off state
    }

    private void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E)) // Press E only if in trigger
        {
            isLightOn = !isLightOn;
            boxLight.enabled = isLightOn;

            Debug.Log("Light switched " + (isLightOn ? "ON" : "OFF"));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true; // Player entered the trigger area
            Debug.Log("Player entered LightSwitch trigger area.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false; // Player left the trigger area
            Debug.Log("Player exited LightSwitch trigger area.");
        }
    }
}
