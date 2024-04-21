public class TutorialModeSingleModal : ModalTrigger
{
    public override void OnAlternateButtonPress() { }

    public override void OnCancelButtonPress()
    {
        base.OnCancelButtonPress();
        CloseModal();
    }

    public override void OnConfirmButtonPress() { }
}
