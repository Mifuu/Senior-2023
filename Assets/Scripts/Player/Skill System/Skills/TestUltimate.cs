using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUltimate : Skill
{
    public TestUltimate() : base("UltimateSkillTest", 3f, 0)
    {

    }

    public override void Activate()
    {
        base.Activate();
        PerformTest();
    }

    private void PerformTest()
    {
        Debug.Log("Skill test: Ultimate Skill Activate Successfully");
    }
}
