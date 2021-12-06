using System;
using AevApp.Helper.Interface;
using Xamarin.Forms;

namespace AevApp.Helper
{
    public class AppSettings
    {
        private readonly IConfigManager _configManager;
        private int? _airportId;
        private int? _locationId;
        private const string AirportIdKey = "airport";
        private const string LocationIdKey = "location";

        public AppSettings(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public int AirportId()
        {
            if (_airportId == null)
            {
                _airportId = _configManager.GetInt(AirportIdKey);
                if (_airportId == int.MinValue)
                {
                    _airportId = Convert.ToInt32(Application.Current.Properties[AirportIdKey]);
                }
            }

            return _airportId.Value;
        }

        public int LocationId()
        {
            if (_locationId == null)
            {
                _locationId = _configManager.GetInt(LocationIdKey);
                if (_locationId == int.MinValue)
                {
                    _locationId = Convert.ToInt32(Application.Current.Properties[LocationIdKey]);
                }
            }

            return _locationId.Value;
        }
    }
}