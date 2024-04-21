using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ModalTrigger : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] protected ModalController modal;
    [SerializeField] protected bool useScriptableObject;

    [Header("Scriptable Object Setup")]
    [SerializeField] protected ModalSettingSO modalSO;

    [Header("Struct Setup")]
    [SerializeField] protected ModalController.ModalSetting modalSetting;

    public virtual void Start()
    {
        // Check if both the setting is not valid
        if (modalSO == null && EqualityComparer<ModalController.ModalSetting>
                .Default.Equals(modalSetting, default(ModalController.ModalSetting)))
            throw new InvalidOperationException("Both modal setting is not valid");
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (useScriptableObject)
            modal.ShowModal(modalSO);
        else
            modal.ShowModal(modalSetting);

        Cursor.visible = true;
        modal.OnCancelButtonPressed += OnConfirmButtonPress;
        modal.OnConfirmButtonPressed += OnConfirmButtonPress;
        modal.OnAlternateButtonPressed += OnAlternateButtonPress;
    }

    public void CloseModal()
    {
        modal.OnCancelButtonPressed -= OnConfirmButtonPress;
        modal.OnConfirmButtonPressed -= OnConfirmButtonPress;
        modal.OnAlternateButtonPressed -= OnAlternateButtonPress;
        modal.HideModal();
    }

    public virtual void OnConfirmButtonPress() {}

    public virtual void OnCancelButtonPress() 
    {
        Cursor.visible = false;
    }

    public virtual void OnAlternateButtonPress() {}
}
