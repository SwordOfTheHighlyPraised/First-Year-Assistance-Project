using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Luna_HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider; // Reference to the slider UI element
    [SerializeField] private RectTransform fillArea; // Reference to the slider's fill area (ensure this is assigned in the Inspector)
    [SerializeField] private RectTransform handleArea; // Reference to the slider's handle area 
    [SerializeField] private Image background; // Reference to the health bar background

    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;

        AdjustFillArea();
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        AdjustFillArea();

        // Hide fill area when health is 0
        if (fillArea != null)
        {
            fillArea.gameObject.SetActive(health > 0);
        }
    }

        private void AdjustFillArea()
    {
        if (fillArea != null)
        {
            Vector2 anchorMin = new Vector2(0f, 0f); // Align fill area to the start
            Vector2 anchorMax = new Vector2(1f, 1f); // Align fill area to the end

            fillArea.anchorMin = anchorMin;
            fillArea.anchorMax = anchorMax;
            fillArea.offsetMin = Vector2.zero; // Remove any offsets
            fillArea.offsetMax = Vector2.zero;
        }

        if (handleArea != null)
        {
            handleArea.anchorMin = new Vector2(0f, 0f);
            handleArea.anchorMax = new Vector2(1f, 1f);
            handleArea.offsetMin = Vector2.zero;
            handleArea.offsetMax = Vector2.zero;
        }
    }

    public void SetBackgroundColor(Color color)
    {
        if (background != null)
        {
            Luna_HealthBar healthBar = GetComponent<Luna_HealthBar>();
            healthBar.SetBackgroundColor(Color.red); // Set the background to red
        }
    }
}
