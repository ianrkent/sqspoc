namespace SQS.POC.Core.Adapters.Configuration
{
    public interface IServiceConfiguration
    {
        string GetConfigSetting(string settingName);
    }
}