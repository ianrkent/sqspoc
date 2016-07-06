namespace PoC.Sqs.Core.Adapters.Configuration
{
    public interface IServiceConfiguration
    {
        string GetConfigSetting(string settingName);
    }
}