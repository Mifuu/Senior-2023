using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;
using System;

public class TutorialModeMultipleModal : ModalTrigger
{
    private Subject<int> currentModalIndex = new Subject<int>(0);
    private int totalModalNumbers;

    [Header("Modal Setup")]
    private bool destroyOnFinished;

    [Header("Scriptable Object Setup")]
    [Tooltip("The Modal will use the setting from Scriptable Object first")]
    [SerializeField] List<ModalSettingSO> settingSOList = new List<ModalSettingSO>();

    [Header("Struct Setup")]
    [Tooltip("Then it will use the setting from struct")]
    [SerializeField] List<ModalController.ModalSetting> settingList = new List<ModalController.ModalSetting>();

    public override void Start()
    {
        totalModalNumbers = settingSOList.Count + settingList.Count;
        currentModalIndex.OnValueChanged += ApplyModalChange;

        if (totalModalNumbers == 0) throw new InvalidOperationException("Both modal list setting is not valid");
        if (settingSOList.Count != 0)
        {
            useScriptableObject = true;
            modalSO = settingSOList[0];
        }
    }

    public void OnDestroy()
    {
        currentModalIndex.OnValueChanged -= ApplyModalChange;
    }

    public override void OnConfirmButtonPress()
    {
        if (currentModalIndex.Value < totalModalNumbers - 1)
        {
            currentModalIndex.Value++;
            return;
        }
        CloseModal();
        currentModalIndex.Value = 0;
        if (destroyOnFinished)
            Destroy(gameObject);
    }

    public override void OnCancelButtonPress()
    {
        if (currentModalIndex.Value > 0)
        {
            currentModalIndex.Value--;
            return;
        }
        CloseModal();
    }

    public override void OnAlternateButtonPress()
    { }

    // Apply change to modal according currentModalIndex
    public void ApplyModalChange(int prev, int current)
    {
        if (current < settingSOList.Count)
            modal.ShowModal(settingSOList[current]);
        else
            modal.ShowModal(settingList[current - settingSOList.Count]);
    }
}
