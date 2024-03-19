using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCardUI : MonoBehaviour
{
    SkillCard[] skillCards = new SkillCard[8];
    // Start is called before the first frame update
    void Start()
    {
        skillCards = Resources.LoadAll<SkillCard>("Assets/Scriptable Objects/Skill Cards");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
