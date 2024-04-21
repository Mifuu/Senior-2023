using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SkillManager : NetworkBehaviour
{
    // [SerializeField] private Transform normalTransform;
   // [SerializeField] private Transform ultimateTransform;

    [SerializeField] private Skill normalSkill;
    [SerializeField] private Skill ultimateSkill;
    public BuffManager buffManager;

    private bool normalSkillReady = true;
    private bool ultimateSkillReady = true;

    public void Awake()
    {
        //normalSkill = normalTransform.GetComponent<Skill>();
        //ultimateSkill = ultimateTransform.GetComponent<Skill>();
        //Debug.Log(normalSkill.name);
        buffManager = transform.parent.GetComponent<BuffManager>();
    }

    public void SetSkillCooldownMultiplier(float multiplier)
    {
        SetNormalSkillCooldownMultiplier(multiplier);
        SetUltimateSkillCooldownMultiplier(multiplier);
    }

    public void SetNormalSkillCooldownMultiplier(float multiplier)
    {
        normalSkill.SetCooldownMultiplier(multiplier);
    }

    public void SetUltimateSkillCooldownMultiplier(float multiplier)
    {
        ultimateSkill.SetCooldownMultiplier(multiplier);
    }

    public float GetCooldownMultiplier()
    {
        return normalSkill.GetCooldownMultiplier();
    }

    public void ActivateNormalSkill()
    {
        if (!IsOwner) return;

        //Debug.Log("SkillManaget: Try to activate Normal skill");
        if (normalSkillReady)
        {
            //Debug.Log("SkillManaget: Try to activate Normal skill success");
            normalSkill.Activate();
            normalSkillReady = false;
            StartCoroutine(NormalSkillCooldown());
        }
        else
        {
            // Log a message indicating why the condition was not met
            if (normalSkill == null)
            {
                Debug.LogWarning("NormalSkill component is null.");
            }

            if (!normalSkillReady)
            {
                Debug.LogWarning("NormalSkill is not ready.");
            }
        }
    }

    public void ActivateUltimateSkill()
    {
        if (!IsOwner) return;

        //Debug.Log("SkillManaget: Try to activate Ultimate Skil");
        if (ultimateSkillReady)
        {
            //Debug.Log("SkillManaget: Try to activate Normal skill success");
            ultimateSkill.Activate();
            ultimateSkillReady = false;
            StartCoroutine(UltimateSkillCooldown());
        }
        else
        {
            // Log a message indicating why the condition was not met
            if (ultimateSkill == null)
            {
                Debug.LogWarning("UltimateSkill component is null.");
            }

            if (!ultimateSkillReady)
            {
                Debug.LogWarning("Ultimate Skill is not ready.");
            }
        }
    }

    // Coroutine for normal skill cooldown
     private IEnumerator NormalSkillCooldown()
     {
         yield return new WaitForSeconds(normalSkill.Cooldown);
         normalSkillReady = true;
     }

     // Coroutine for ultimate skill cooldown
     private IEnumerator UltimateSkillCooldown()
     {
         yield return new WaitForSeconds(ultimateSkill.Cooldown);
         ultimateSkillReady = true;
     }
}
