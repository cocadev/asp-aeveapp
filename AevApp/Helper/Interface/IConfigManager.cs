namespace AevApp.Helper.Interface
{
    public interface IConfigManager
    {
        string Get(string settingName);
        bool GetBoolean(string settingName);
        int GetInt(string settingName);
    }
}