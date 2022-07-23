using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace ProcGenEditor.GraphElems
{
    public class ProcGenGroup : Group
    {
        public ProcGenGroup()
        {
            capabilities |= Capabilities.Ascendable;
        }
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

        public override bool AcceptsElement(GraphElement element, ref string reasonWhyNotAccepted)
        {
            return element is ProcGenGraphNodeView;
        }

        protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
        {
            GraphView gv = GetFirstAncestorOfType<GraphView>();

            if ( gv != null && gv.elementsRemovedFromGroup != null ) 
                gv.elementsRemovedFromGroup(this, elements);

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