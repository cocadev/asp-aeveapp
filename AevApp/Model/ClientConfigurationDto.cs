using System.Collections.Generic;

namespace AevApp.Model
{
    public class ClientConfigurationDto
    {
        public int Id { get; set; }

        public Dictionary<string, string> ConfigurationValues { get; set; }
    }
}
