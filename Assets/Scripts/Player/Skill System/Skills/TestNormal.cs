using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNormal : Skill
{
    public TestNormal() : base("NormalSkillTest", 2f, 0)
    {

    }

    public override void Activate()
    {
        base.Activate();
        PerformTest();
    }

    private void PerformTest()
    {
        Debug.Log("Skill test: Normal Skill Activate Successfully");
    }
}
