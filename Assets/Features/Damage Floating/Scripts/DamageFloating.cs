using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageFloating : MonoBehaviour
{
    [SerializeField] private DamageFloatingSetting setting;

    [Header("Requirements")]
    [SerializeField] private TMP_Text damageTMP;

    [Header("Value")]
    string value = "0";

    float lifeTime = float.MaxValue;
    float timer = 0f;
    Vector3 initScale;
    float upVelocity;

    void OnEnable()
    {
        // set initial value
        lifeTime = Random.Range(setting.durationRandRange.x, setting.durationRandRange.y);
        timer = 0f;
        initScale = transform.localScale;
        damageTMP.text = value;

        MoveToRandomPosition();

        UpdateAlphaAndScale();

        upVelocity = setting.startingUpVelocity;
    }

    public void SetValue(string value)
    {
        this.value = value;
        damageTMP.text = value;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > lifeTime)
        {
            Destroy(gameObject);
        }

        UpdateAlphaAndScale();

        if (Camera.main != null) transform.rotation = Camera.main.transform.rotation;
    }

    void FixedUpdate()
    {
        FixedUpdateMoveToCamera();
        FixedUpdateUpVelocity();
    }

    void UpdateAlphaAndScale()
    {
        float t = timer / lifeTime;
        damageTMP.color = new Color(damageTMP.color.r, damageTMP.color.g, damageTMP.color.b, setting.alphaOverLifetime.Evaluate(t));
        transform.localScale = initScale * setting.scaleOverLifetime.Evaluate(t);
    }

    void FixedUpdateMoveToCamera()
    {
        if (Camera.main == null) return;
        float dist = setting.velocityToCamera * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position, dist);
    }

    void FixedUpdateUpVelocity()
    {
        upVelocity -= setting.gravity * Time.deltaTime;
        transform.position += new Vector3(0, upVelocity * Time.fixedDeltaTime, 0);
    }

    void MoveToRandomPosition()
    {
        transform.position += new Vector3(Random.Range(-setting.randSpawnOffset.x, setting.randSpawnOffset.x),
            Random.Range(-setting.randSpawnOffset.y, setting.randSpawnOffset.y),
            Random.Range(-setting.randSpawnOffset.z, setting.randSpawnOffset.z));
    }
}
