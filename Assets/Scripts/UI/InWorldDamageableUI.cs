using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InWorldDamageableUI : MonoBehaviour
{
    [Header("References")]
    public GameObject damageableGameObject;
    private IDamageable damageable;
    [Header("Requirements")]
    public Slider healthBarSlider;

    void OnEnable()
    {
        damageable = damageableGameObject.GetComponent<IDamageable>();
        damageable.OnHealthChanged.AddListener(UpdateHealthBar);
    }

    void OnDisable()
    {
        damageable = damageableGameObject.GetComponent<IDamageable>();
        damageable.OnHealthChanged.RemoveListener(UpdateHealthBar);
    }

    public void UpdateHealthBar()
    {
        Debug.Log("test");
        healthBarSlider.value = damageable.currentHealth.Value / damageable.maxHealth;
    }
}
