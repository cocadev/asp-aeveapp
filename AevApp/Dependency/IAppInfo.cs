namespace AevApp.Dependency
{
    public interface IAppInfo
    {
        string ApplicationName { get; }
        string ApplicationVersion { get; }
        string ApplicationBuild { get; }
        bool HasCameraPermission { get; }
        bool HasCameraGeoLocationPermission { get; }
    }
}