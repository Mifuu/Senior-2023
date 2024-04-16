using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextAnimation : MonoBehaviour
{
    [SerializeField] private List<string> listOfWords;
    [SerializeField] private float deltaTime;
    [SerializeField] private TextMeshProUGUI target;
    private IEnumerator loop;

    public void Start()
    {
        loop = LoopAnimation();
        if (listOfWords.Count == 0)
        {
            Debug.LogError("List of word is 0");
            return;
        }
        StartCoroutine(loop);
    }

    public void OnDestroy()
    {
        if (loop == null) return;
        StopCoroutine(loop);
    }

    public IEnumerator LoopAnimation()
    {
        int count = 0;
        while (true) 
        {
            target.text = listOfWords[count++];
            if (count >= listOfWords.Count)
                count = 0;
            yield return new WaitForSeconds(deltaTime);
        }
    }
}
