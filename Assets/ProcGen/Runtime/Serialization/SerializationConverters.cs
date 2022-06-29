using Newtonsoft.Json;
using System;
using UnityEngine;

namespace ProcGen.Serialization
{
    public class BaseNodeConverter : JsonConverter
    {
        private GraphReflection m_Reflection;
        private int m_Index;
        public override bool CanWrite => false;
        public void SetGraphReflection(GraphReflection reflection)
        {
            m_Reflection = reflection;
            m_Index = 0;
        }
        public override bool CanConvert(Type objectType)
        {
            return typeof(BaseNode) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type objType = Type.GetType(m_Reflection.GetQualifiedName(m_Index++), true);
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
            if (objectType == typeof(Vector2))
            {
                reader.Read();
                float x = (float)((double)reader.Value);
                reader.Read();
                float y = (float)((double)reader.Value);
                reader.Read();
                return new Vector2(x, y);
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