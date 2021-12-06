using System.Threading.Tasks;

namespace AevApp.Dependency
{
    public interface ILocationInfo
    {
        Task<Location> GetLastLocation();
    }
}
