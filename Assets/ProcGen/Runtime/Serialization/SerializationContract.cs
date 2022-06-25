using ProcGen;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using UnityEngine;
using Type = System.Type;

namespace ProcGen.Serialization
{
    public static class ProcGenSerialization
    {
        public static JsonSerializerSettings SerializationSettings { get; private set; }

        public static BaseNodeConverter NodeConverter { get; private set; }
        static ProcGenSerialization()
        {
            NodeConverter = new BaseNodeConverter();
            SerializationSettings = new JsonSerializerSettings()
            {
                ContractResolver = new ProcGenContractResolver()
            };

            SerializationSettings.Converters.Add(new VectorConverter());
            SerializationSettings.Converters.Add(NodeConverter);
        }
    }
    public class ProcGenContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = base.CreateProperties(type, memberSerialization);
            for(int i = props.Count - 1; i >= 0; --i)
            {
                if (type.GetField(props[i].PropertyName) == null) // keep fields only
                    props.RemoveAt(i);
            }
            return props;
        }
    }
    public class BaseNodeConverter : JsonConverter
    {
        private string[] m_TypeArray;
        private int m_Index;
        public override bool CanWrite => false;
        public void SetTypeArray(string[] typeArray)
        {
            m_TypeArray = typeArray;
            m_Index = 0;
        }
        public override bool CanConvert(Type objectType)
        {
            return typeof(BaseNode) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type objType = Type.GetType(m_TypeArray[m_Index++], true);
#if UNITY_EDITOR
            ScriptableObject so = ScriptableObject.CreateInstance(objType);
            serializer.Populate(reader, so);
            return so;
#else
            return serializer.Deserialize(reader, objType);
#endif
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new System.Exception("Should not be used when serializing");
        }
    }
    public class VectorConverter : JsonConverter
    {
       private readonly Type[] m_CompatibleTypes;

        public VectorConverter()
        {
            m_CompatibleTypes = new Type[] { typeof(Vector2) };
        }

        public override bool CanConvert(Type objectType)
        {
            return System.Array.IndexOf(m_CompatibleTypes, objectType) != -1;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if ( objectType == typeof(Vector2))
            {
                reader.Read();
                float x = (float) ( (double) reader.Value);
                reader.Read();
                float y = (float) ( (double) reader.Value);
                reader.Read();
                return new Vector2(x,y);
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Vector2 valueVec = (Vector2)value;
            writer.WriteStartArray();
            writer.WriteValue(valueVec.x);
            writer.WriteValue(valueVec.y);
            writer.WriteEndArray();
        }
    }
}