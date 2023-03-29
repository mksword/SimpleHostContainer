using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace SimpleHostContainer
{
    /// <summary>
    /// 字符串资源转换类
    /// </summary>
    internal class StringResourceConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanWrite => true;

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(StringResource).IsAssignableFrom(objectType);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (CanConvert(objectType))
            {
                var jsonObj = JObject.Load(reader);
                StringResource value = new StringResource();
                JToken token = null;

                if (jsonObj.TryGetValue(nameof(value.Culture), out token))
                {
                    if (token != null)
                    {
                        string culture_name = token.Value<string>();
                        CultureInfo culture = new CultureInfo(culture_name);
                        StringResource.Helper.SetCulture(value, culture);
                        token = null;
                    }
                }

                if (jsonObj.TryGetValue(nameof(value.Texts), out token))
                {
                    if (token != null)
                    {
                        foreach (var child in token.Children())
                        {
                            JProperty property = child as JProperty;
                            StringResource.Helper.AddText(value, property.Name, property.Value.Value<string>());
                        }
                    }
                }

                return value;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            StringResource resource = value as StringResource;

            if (resource != null)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(nameof(resource.Culture));
                writer.WriteValue(resource.Culture.Name);
                writer.WritePropertyName(nameof(resource.Texts));
                writer.WriteStartObject();

                foreach (string key in resource.Texts.Keys)
                {
                    writer.WritePropertyName(key);
                    writer.WriteValue(resource.Texts[key]);
                }

                writer.WriteEndObject();
                writer.WriteEndObject();
            }
        }
    }
}
