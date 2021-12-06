using System.Collections.Generic;

namespace AevApp.Model.Responses
{
    public class GetLocationsResponse
    {
        public int Total { get; set; }

        public List<Location> Items { get; set; }
    }
}