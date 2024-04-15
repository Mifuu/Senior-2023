using UnityEngine;

public class TutorialTextTrigger : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private bool useScriptableObject;

    [Header("Scriptable Object Setup")]
    [SerializeField] private TutorialTextSettingSO settingSO;
    
    [Header("Struct Setup")]
    [SerializeField] private TutorialTextController.TutorialSetting setting;

    public void OnTriggerEnter(Collider collider)
    {
        if (useScriptableObject)
            TutorialTextController.Singleton.ShowTutorialText(settingSO);
        else
            TutorialTextController.Singleton.ShowTutorialText(setting);
    }
}
