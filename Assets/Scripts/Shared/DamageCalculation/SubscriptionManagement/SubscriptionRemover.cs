public class SubscriptionRemover
{
    public string UnitName { get; set; }
    private bool isDealer;
    private DamageSubscriptionContainer<float, string, int, bool> subscriptionContainer;

    public SubscriptionRemover(string UnitName, bool isDealer, DamageSubscriptionContainer<float, string, int, bool> subscriptionContainer)
    {
        this.UnitName = UnitName;
        this.isDealer = isDealer;
        this.subscriptionContainer = subscriptionContainer;
    }

    public bool RemoveFloat(string name)
    {
        float temp;
        return subscriptionContainer.RemoveTrackedValue(UnitName, isDealer, name, out temp);
    }

    public bool RemoveString(string name)
    {
        string temp;
        return subscriptionContainer.RemoveTrackedValue(UnitName, isDealer, name, out temp);
    }

    public bool RemoveInt(string name)
    {
        int temp;
        return subscriptionContainer.RemoveTrackedValue(UnitName, isDealer, name, out temp);
    }

    public bool RemoveBool(string name)
    {
        bool temp;
        return subscriptionContainer.RemoveTrackedValue(UnitName, isDealer, name, out temp);
    }
}
