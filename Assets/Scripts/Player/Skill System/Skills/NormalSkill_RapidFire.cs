using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalSkill_RapidFire : Skill
{

    [SerializeField] private float skillDuration = 10f;
    [SerializeField] private float shootingSpeedMultiplier = -0.5f;

    public NormalSkill_RapidFire() : base("Rapid Fire", 20f, 0) // skill cooldown = 20f
    {

    }

    public override void Activate()
    {
        base.Activate();
        StartCoroutine(PerformNormalSkillCoroutine());
    }

    private IEnumerator PerformNormalSkillCoroutine()
    {
        SkillManager skillManager = FindObjectOfType<SkillManager>();
        if (skillManager == null)
        {
            Debug.LogError("SkillManager not found in the scene.");
            yield break;
        }

        BuffManager buffManager = skillManager.buffManager;
        if (buffManager == null)
        {
            Debug.LogError("BuffManager not found in the SkillManager.");
            yield break;
        }

        buffManager.ShootingSpeedBuff_NormalSkill.Value += shootingSpeedMultiplier;
        yield return new WaitForSeconds(skillDuration);
        buffManager.ShootingSpeedBuff_NormalSkill.Value -= shootingSpeedMultiplier;
        Debug.Log("Normal Skill: Rapid Fire activated successfully.");
    }
}
