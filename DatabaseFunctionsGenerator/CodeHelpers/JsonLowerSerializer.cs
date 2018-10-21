using DatabaseFunctionsGenerator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator.CodeHelpers
{
    public class JsonLowerSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonLowerSerializer serializer)
        {
            var name = value as User;
            writer.WriteStartObject();

            foreach (PropertyInfo prop in typeof(User).GetProperties())
            {
                string propertyName;

                propertyName = prop.Name;
                if (2 > propertyName.Length)
                {
                    propertyName = propertyName.Substring(0, 1).ToLower();
                }
                else
                {
                    propertyName = propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
                }
                writer.WritePropertyName(propertyName);
                serializer.Serialize(writer, prop.GetValue(value));
            }


            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonLowerSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            var name = new User();
            serializer.Populate(jsonObject.CreateReader(), name);
            return name;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(JsonLowerCaseSerializer).IsAssignableFrom(objectType);
        }
    }
}
