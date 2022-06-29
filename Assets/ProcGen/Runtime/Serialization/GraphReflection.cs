using System.Collections.Generic;

namespace ProcGen.Serialization
{
    using Type = System.Type;
    [System.Serializable]
    public struct GraphReflection
    {
        public string[] Assemblies;
        public string[] Types;
        public NodeReflection[] NodeTypes;

        [System.Serializable]
        public struct NodeReflection
        {
            public int AssemblyIndex;
            public int NameIndex;
        }

        public string GetQualifiedName(int nodeIndex)
        {
            ref NodeReflection nodeReflection = ref NodeTypes[nodeIndex];
            return string.Format("{0}, {1}", Types[nodeReflection.NameIndex], Assemblies[nodeReflection.AssemblyIndex]);
        }

        public static readonly GraphReflection Empty = new GraphReflection()
        {
            Assemblies = new string[0],
            Types = new string[0],
            NodeTypes = new NodeReflection[0]
        };

        public static GraphReflection FromArray(BaseNode[] Nodes)
        {
            List<string> assemblies = new List<string>();
            List<string> names = new List<string>();
            NodeReflection[] nodes = new NodeReflection[Nodes.Length];

            for(int i = 0; i < Nodes.Length; ++i)
            {
                Type nodeType = Nodes[i].GetType();
                int assIdx = assemblies.IndexOf(nodeType.Assembly.FullName);
                if ( assIdx == -1 )
                {
                    assIdx = assemblies.Count;
                    assemblies.Add(nodeType.Assembly.FullName);
                }
                int nameIdx = names.IndexOf(nodeType.FullName);
                if ( nameIdx == -1 )
                {
                    nameIdx = names.Count;
                    names.Add(nodeType.FullName);
                }

                nodes[i] = new NodeReflection() { AssemblyIndex = assIdx, NameIndex = nameIdx };
            }

            return new GraphReflection()
            {
                Assemblies = assemblies.ToArray(),
                Types = names.ToArray(),
                NodeTypes = nodes
            };
        }
    }
}