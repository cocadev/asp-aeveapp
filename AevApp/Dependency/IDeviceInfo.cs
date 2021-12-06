namespace AevApp.Dependency
{
    public interface IDeviceInfo
    {
        string DeviceIdentifier { get; }
        string DeviceName { get; }
        string DeviceOperatingSystem { get; }
        string DeviceOperatingSystemVersion { get; }
        string DeviceBrand { get; }
        string DeviceModel { get; }
    }
}
