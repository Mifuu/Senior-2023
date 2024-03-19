using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GaugeUI : MonoBehaviour
{
    public static GaugeUI instance;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TMP_Text title;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text word;
    private Queue<string> wordQueue = new Queue<string>();
    private Coroutine wordQueueVisibleCR;
    [SerializeField] CanvasGroup wordQueueCanvasGroup;

    Color titleColor;
    float deteriorationRate;

    [Header("Settings")]
    [SerializeField] float wordQueueVisibleTime = 3f;
    [SerializeField] float wordQueueMax = 5;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        canvasGroup.alpha = 0;
    }

    void Start()
    {
        // Register to the event
        GaugeSystem.Singleton.isGaugeVisible.OnValueChanged += GaugeVisibleChanged;
        GaugeSystem.Singleton.OnGaugeLevelChanged += GaugeLevelChanged;
        GaugeSystem.Singleton.OnNewWordQueued += NewWordQueued;
    }

    void FixedUpdate()
    {
        // update the slider deterioration rate
        slider.value = Mathf.Clamp01(slider.value - deteriorationRate);
    }

    void GaugeVisibleChanged(bool wasVisible, bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1 : 0;
    }

    void GaugeLevelChanged(GaugeSetDetail detail)
    {
        // Update UI
        title.text = detail.title;
        slider.value = detail.ratio;
        titleColor = detail.color;
        deteriorationRate = detail.deteriorationRate;
    }

    void NewWordQueued(string word)
    {
        // Add to queue and set word text
        AddToWordQueue(word);
        this.word.text = GetWordStringFromWordQueue();

        // reset visibility coroutine
        if (wordQueueVisibleCR != null)
            StopCoroutine(wordQueueVisibleCR);
        wordQueueVisibleCR = StartCoroutine(WordQueueVisibleCR(wordQueueVisibleTime));
    }

    void AddToWordQueue(string word)
    {
        if (wordQueue.Count >= wordQueueMax)
            wordQueue.Dequeue();
        wordQueue.Enqueue(word);
    }

    string GetWordStringFromWordQueue()
    {
        string output = "";
        foreach (var w in wordQueue)
        {
            output += $"{w} +\n";
        }
        return output;
    }

    IEnumerator WordQueueVisibleCR(float visibleTime)
    {
        wordQueueCanvasGroup.alpha = 1;
        yield return new WaitForSeconds(visibleTime);
        wordQueueCanvasGroup.alpha = 0;

        // if the visibility is completely clear, clear the queue
        yield return new WaitForSeconds(0.6f);
        wordQueue.Clear();
    }
}
