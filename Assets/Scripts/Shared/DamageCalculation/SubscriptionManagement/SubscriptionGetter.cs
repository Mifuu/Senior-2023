public class SubscriptionGetter
{
    public string UnitName { private get; set; }
    private bool isDealer;
    private DamageSubscriptionContainer<float, string, int, bool> subscriptionContainer;

    public SubscriptionGetter(string UnitName, bool isDealer, DamageSubscriptionContainer<float, string, int, bool> subscriptionContainer)
    {
        this.UnitName = UnitName;
        this.isDealer = isDealer;
        this.subscriptionContainer = subscriptionContainer;
    }

    public bool GetFloat(string name, out float value)
    {
        return subscriptionContainer.GetTrackedValue(UnitName, name, out value);
    }

    public bool GetString(string name, out string value)
    {
        return subscriptionContainer.GetTrackedValue(UnitName, name, out value);
    }

    public bool GetInt(string name, out int value)
    {
        return subscriptionContainer.GetTrackedValue(UnitName, name, out value);
    }

    public bool GetBool(string name, out bool value)
    {
        return subscriptionContainer.GetTrackedValue(UnitName, name, out value);
    }
}
