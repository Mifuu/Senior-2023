using System.Collections;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private Transform normalTransform;
    [SerializeField] private Transform ultimateTransform;

    private Skill normalSkill;
    private Skill ultimateSkill;

    private bool normalSkillReady = true;
    private bool ultimateSkillReady = true;

    public void Awake()
    {
        normalSkill = normalTransform.GetComponent<Skill>();
        ultimateSkill = ultimateTransform.GetComponent<Skill>();
        Debug.Log(normalSkill.name);

    }
    public void ActivateNormalSkill()
    {
        Debug.Log("SkillManaget: Try to activate Normal skill");
        if (normalSkillReady)
        {
            Debug.Log("SkillManaget: Try to activate Normal skill success");
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
        Debug.Log("SkillManaget: Try to activate Ultimate skill");
        if (ultimateSkill != null&& ultimateSkillReady)
        {
            ultimateSkill.Activate();
            ultimateSkillReady = false;
            StartCoroutine(UltimateSkillCooldown());
        }
    }

    // Example coroutine for normal skill cooldown
     private IEnumerator NormalSkillCooldown()
     {
         yield return new WaitForSeconds(normalSkill.Cooldown);
         normalSkillReady = true;
     }

     //Example coroutine for ultimate skill cooldown
     private IEnumerator UltimateSkillCooldown()
     {
         yield return new WaitForSeconds(ultimateSkill.Cooldown);
         ultimateSkillReady = true;
     }
}
