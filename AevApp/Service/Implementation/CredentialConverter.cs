using System;
using AevApp.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AevApp.Service.Implementation
{
    public class CredentialConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(CredentialDto));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            if (jo["credentialType"].Value<string>() == "ASIC")
                return jo.ToObject<AsicDto>(serializer);

            if (jo["credentialType"].Value<string>() == "VisitorPass")
                return jo.ToObject<VisitorPassDto>(serializer);

            return null;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
