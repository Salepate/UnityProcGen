using Newtonsoft.Json;

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
                ContractResolver = new GenerativeGraphContractResolver()
            };

            SerializationSettings.Converters.Add(new VectorConverter());
            SerializationSettings.Converters.Add(NodeConverter);
        }
    }
}