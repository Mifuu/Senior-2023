public class SubscriptionAdder 
{
    public string UnitName { private get; set; }
    private bool isDealer;
    private DamageSubscriptionContainer<float, string, int, bool> subscriptionContainer;

    public SubscriptionAdder(string UnitName, bool isDealer, DamageSubscriptionContainer<float, string, int, bool> subscriptionContainer)
    {
        this.UnitName = UnitName;
        this.isDealer = isDealer;
        this.subscriptionContainer = subscriptionContainer;
    }

    public bool AddFloat(string name, ObserverPattern.IObservable<float> subject)
    {
        return subscriptionContainer.AddParameterToTrackList(UnitName, isDealer, name, subject);
    }

    public bool AddString(string name, ObserverPattern.IObservable<string> subject)
    {
        return subscriptionContainer.AddParameterToTrackList(UnitName, isDealer, name, subject);
    }

    public bool AddInt(string name, ObserverPattern.IObservable<string> subject)
    {
        return subscriptionContainer.AddParameterToTrackList(UnitName, isDealer, name, subject);
    }

    public bool AddBool(string name, ObserverPattern.IObservable<bool> subject)
    {
        return subscriptionContainer.AddParameterToTrackList(UnitName, isDealer, name, subject);
    }
}
