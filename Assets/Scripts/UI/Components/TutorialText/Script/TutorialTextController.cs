using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class TutorialTextController : MonoBehaviour
{
    public static TutorialTextController Singleton;

    [Serializable]
    public struct TutorialSetting
    {
        [Tooltip("How long the text would last in seconds")]
        [SerializeField] public float lifeSpan;
        [TextArea(10, 20)]
        [SerializeField] public string text;
        [Tooltip("")]
        [SerializeField] public bool usePreferredSize;
    }

    [SerializeField] private GameObject modal;
    [SerializeField] private TextMeshProUGUI text;

    private LayoutElement layout;
    private IEnumerator currentCountdown = null;

    public void Awake()
    {
        if (Singleton == null)
            Singleton = this;
        else
            Destroy(this);

        gameObject.SetActive(false);
        layout = modal.GetComponent<LayoutElement>();
    }

    public void ShowTutorialText(TutorialSetting setting)
    {
        if (currentCountdown != null)
            StopCoroutine(currentCountdown);

        modal.SetActive(true);
        currentCountdown = _ShowTutorialTextCoroutine(setting);
        layout.enabled = setting.usePreferredSize;
        StartCoroutine(currentCountdown);
    }

    public void ShowTutorialText(TutorialTextSettingSO setting)
    {
        if (currentCountdown != null)
            StopCoroutine(currentCountdown);

        modal.SetActive(true);
        currentCountdown = _ShowTutorialTextCoroutine(setting);
        layout.enabled = setting.usePreferredSize;
        StartCoroutine(currentCountdown);
    }

    private IEnumerator _ShowTutorialTextCoroutine(TutorialSetting setting)
    {
        text.text = setting.text;
        yield return new WaitForSeconds(setting.lifeSpan);
        modal.SetActive(false);
        currentCountdown = null;
    }

    private IEnumerator _ShowTutorialTextCoroutine(TutorialTextSettingSO setting)
    {
        text.text = setting.text;
        yield return new WaitForSeconds(setting.lifeSpan);
        modal.SetActive(false);
        currentCountdown = null;
    }
}
