using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GaugeUI : MonoBehaviour
{
    public static GaugeUI instance;

    [SerializeField] GameObject mainLTTarget;
    [SerializeField] CanvasGroup mainCanvasGroup;
    [SerializeField] TMP_Text title;
    private string previousTitle;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text word;
    private Queue<string> wordQueue = new Queue<string>();
    private Coroutine wordQueueVisibleCR;
    [SerializeField] CanvasGroup wordQueueCanvasGroup;

    float deteriorationRate;

    [Header("WordQueue Settings")]
    [SerializeField] float wordQueueVisibleTime = 3f;
    [SerializeField] float wordQueueMax = 5;

    [Header("Tween Settings")]
    [SerializeField] float mainCanvasGroupAlpha1TweenTime = 0.09f;
    [SerializeField] float mainCanvasGroupAlpha0TweenTime = 0.5f;
    [SerializeField] float wordQueueCanvasGroupAlpha1TweenTime = 0.09f;
    [SerializeField] float wordQueueCanvasGroupAlpha0TweenTime = 0.7f;
    [SerializeField] float simpleTweenSize = 1.2f;
    [SerializeField] float simpleTweenTime = 0.25f;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        mainCanvasGroup.alpha = 0;
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
        // canvasGroup.alpha = isVisible ? 1 : 0;
        SetCanvasGroupAlpha(mainCanvasGroup, isVisible ? 1 : 0, isVisible ? mainCanvasGroupAlpha1TweenTime : mainCanvasGroupAlpha0TweenTime);
    }

    void GaugeLevelChanged(GaugeSetDetail detail)
    {
        // Update UI
        previousTitle = title.text;
        title.text = detail.title;
        slider.value = detail.ratio;
        title.color = new Color(detail.color.r, detail.color.g, detail.color.b, title.color.a);
        deteriorationRate = detail.deteriorationRate;

        // tweening
        if (previousTitle != detail.title)
        {
            SimpleTweenSize(title.gameObject, simpleTweenSize, simpleTweenTime);
            // SimpleTweenSize(mainLTTarget, simpleTweenSize, simpleTweenTime);
        }
        SimpleTweenSize(slider.gameObject, simpleTweenSize, simpleTweenTime);
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

        // tweening
        SimpleTweenSize(this.word.gameObject, simpleTweenSize, simpleTweenTime);
    }

    #region Internal Methods
    LTDescr SimpleTweenSize(GameObject gameObject, float size, float tweenTime)
    {
        LeanTween.cancel(gameObject);

        gameObject.transform.localScale = Vector3.one;

        return LeanTween.scale(gameObject, new Vector3(size, size, size), tweenTime)
            .setEasePunch();
    }

    LTDescr SetCanvasGroupAlpha(CanvasGroup canvasGroup, float alpha, float tweenTime)
    {
        // canvasGroup.alpha = isVisible ? 1 : 0;
        LeanTween.cancel(canvasGroup.gameObject);
        return LeanTween.alphaCanvas(canvasGroup, alpha, tweenTime)
            .setEaseInOutCubic();
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
        SetCanvasGroupAlpha(wordQueueCanvasGroup, 1, wordQueueCanvasGroupAlpha1TweenTime);
        yield return new WaitForSeconds(visibleTime);
        SetCanvasGroupAlpha(wordQueueCanvasGroup, 0, wordQueueCanvasGroupAlpha0TweenTime)
            .setOnComplete(
                () =>
                {
                    // if the visibility is completely clear, clear the queue
                    wordQueue.Clear();
                    word.text = "";
                }
            );
    }
    #endregion
}