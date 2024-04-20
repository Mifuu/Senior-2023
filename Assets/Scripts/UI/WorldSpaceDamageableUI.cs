using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldSpaceDamageableUI : MonoBehaviour
{
    [Header("Settings")]
    bool isFacingCamera = true;

    [Header("References")]
    public GameObject damageableGameObject;
    private IDamageable damageable;
    [Header("Requirements")]
    public Slider healthBarSlider;
    public TextMeshProUGUI levelText;

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
        damageable.currentHealth.OnValueChanged += UpdateHealthBar;
        if (damageableGameObject.TryGetComponent<Enemy.EnemyStat>(out var stat))
        {
            PaintLevel(0, stat.Level.Value);
            stat.Level.OnValueChanged += PaintLevel;
        }
    }

    public void OnDisable()
    {
        damageable.currentHealth.OnValueChanged -= UpdateHealthBar;
        if (damageableGameObject.TryGetComponent<Enemy.EnemyStat>(out var stat))
            stat.Level.OnValueChanged -= PaintLevel;
    }

    public void UpdateHealthBar(float prev, float current) => healthBarSlider.value = current / damageable.maxHealth;
    public void PaintLevel(int prev, int current) => levelText.text = "LV. " + current;
}
