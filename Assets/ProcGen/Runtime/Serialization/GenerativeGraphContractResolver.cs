using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using Type = System.Type;

namespace ProcGen.Serialization
{
    public class GenerativeGraphContractResolver : DefaultContractResolver
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
}