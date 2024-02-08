using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelaySlider : MonoBehaviour
{
    public Slider frontSlider;
    public Slider backSlider;

    [Space]
    public Color normalColor = Color.white;
    public Color decrementColor = Color.red;
    public Color incrementColor = Color.green;

    [Space]
    public float speed = 1;

    public float Value { get; private set; }
    public float MaxValue { get; private set; }
    private float relayValue = 1;

    Image frontImage;
    Image backImage;

    void Awake()
    {
        Value = 1;
        MaxValue = 1;

        frontImage = frontSlider.fillRect.GetComponent<Image>();
        backImage = backSlider.fillRect.GetComponent<Image>();
    }

    public void SetValue(float value, float maxValue)
    {
        MaxValue = maxValue;

        SetValue(value);
    }

    public void SetValue(float value)
    {
        Value = value;
        // Debug.Log($"Value: {Value}, MaxValue: {MaxValue}");

        if (Value < relayValue)
        {
            frontImage.color = normalColor;
            backImage.color = decrementColor;
        }
        else
        {
            frontImage.color = normalColor;
            backImage.color = incrementColor;
        }
    }

    void Update()
    {
        UpdateSlider();
    }

    void UpdateSlider()
    {
        if (relayValue == Value) return;

        if (Value < relayValue)
        {
            relayValue -= Time.deltaTime * Mathf.Abs(Value - relayValue) * speed;
            if (relayValue < Value) relayValue = Value;
            frontSlider.value = Value;
            backSlider.value = relayValue;
        }
        else
        {
            relayValue += Time.deltaTime * Mathf.Abs(Value - relayValue) * speed;
            if (relayValue > Value) relayValue = Value;
            frontSlider.value = relayValue;
            backSlider.value = Value;
        }
    }
}
