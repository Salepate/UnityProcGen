using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace ProcGenEditor.GraphElems
{
    public class ProcGenGroup : Group
    {
        protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        {
            base.OnElementsAdded(elements);

            foreach(GraphElement element in elements)
            {
                if ( element is ProcGenGraphNodeView node)
                {
                    node.ParentGroup = this;
                }
            }
        }

        protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
        {
            base.OnElementsRemoved(elements);
            foreach (GraphElement element in elements)
            {
                if (element is ProcGenGraphNodeView node)
                {
                    node.ParentGroup = null;
                }
            }
        }
    }
}