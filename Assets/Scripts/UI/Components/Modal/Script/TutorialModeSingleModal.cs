public class TutorialModeSingleModal : ModalTrigger
{
    public override void OnAlternateButtonPress() { }

    public override void OnCancelButtonPress() { CloseModal(); }

    public override void OnConfirmButtonPress() { }
}
