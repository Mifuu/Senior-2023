using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceDamageableUI : MonoBehaviour
{
    [Header("Settings")]
    bool isFacingCamera = true;

    [Header("References")]
    public GameObject damageableGameObject;
    private IDamageable damageable;
    [Header("Requirements")]
    public Slider healthBarSlider;

    void Update()
    {
        if (isFacingCamera && Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
        }
    }

    void OnEnable()
    {
        damageable = damageableGameObject.GetComponent<IDamageable>();
        damageable.currentHealth.OnValueChanged += (prev, current) => UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        healthBarSlider.value = damageable.currentHealth.Value / damageable.maxHealth;
    }
}
