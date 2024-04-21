using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Matchmaker.Models;
using System.Collections;

[RequireComponent(typeof(ModalController))]
public class FindingMatchModalController : MonoBehaviour
{
    [SerializeField] private ModalController.ModalSetting setting;
    [SerializeField] private List<string> listOfChangingTitle;

    private ModalController controller;
    private MultiplayAssignment.StatusOptions latestStatus = MultiplayAssignment.StatusOptions.InProgress;
    private IEnumerator changeTitleCoroutine;

#if !DEDICATED_SERVER
    public void Awake()
    {
        controller = GetComponent<ModalController>();
    }

    public void Start()
    {
        CloudService.MatchMakingService.Singleton.isServiceReady.OnValueChanged += SetupModal;
    }

    public void SetupModal(bool _, bool current)
    {
        if (current)
        {
            CloudService.MatchMakingService.Singleton.isServiceReady.OnValueChanged -= SetupModal;
            CloudService.MatchMakingService.Singleton.isSearching.OnValueChanged += ChangeModalState;
        }
    }

    public void ChangeModalState(bool _, bool current)
    {
        if (current)
        {
            controller.ShowModal(setting);
            controller.OnCancelButtonPressed += CancelMatch;
            CloudService.MatchMakingService.Singleton.OnMatchingSuccess += CleanUp;
            CloudService.MatchMakingService.Singleton.OnMatchingStatusUpdate += SwitchText;

            changeTitleCoroutine = ChangeTitle();
            StartCoroutine(changeTitleCoroutine);
        }
        else
        {
            controller.HideModal();
            controller.OnCancelButtonPressed -= CancelMatch;
            CloudService.MatchMakingService.Singleton.OnMatchingSuccess -= CleanUp;
            CloudService.MatchMakingService.Singleton.OnMatchingStatusUpdate -= SwitchText;
            StopCoroutine(changeTitleCoroutine);
        }
    }

    private void CancelMatch()
    {
        CleanUp();
        CloudService.MatchMakingService.Singleton.StopFindingMatch();
    }

    private void CleanUp()
    {
        controller.OnCancelButtonPressed -= CancelMatch;
        CloudService.MatchMakingService.Singleton.OnMatchingSuccess -= CleanUp;
        CloudService.MatchMakingService.Singleton.OnMatchingStatusUpdate -= SwitchText;
    }

    private void SwitchText(MultiplayAssignment.StatusOptions current)
    {
        if (setting.content_IsVertical)
            controller.verticalContentText.text = ("MatchMaking Status: ") + current.ToString();
        else
            controller.horizontalContentText.text = ("MatchMaking Status: ") + current.ToString();
    }

    private IEnumerator ChangeTitle()
    {
        int count = 0;
        int time = 0;
        while (true)
        {
            controller.headerText.text = listOfChangingTitle[count++] + "     " + FormTime(time++);
            if (count >= listOfChangingTitle.Count) count = 0;
            yield return new WaitForSeconds(1.0f);
        }
    }

    private string FormTime (int seconds) 
    {
        var minutes = seconds % 60;
        string minsText = minutes.ToString();
        if (seconds % 60 < 10) minsText = "0" + minsText;
        return (seconds / 60) + ":" + (minsText);
    }

#endif
}
