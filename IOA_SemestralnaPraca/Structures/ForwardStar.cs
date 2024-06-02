using IOA_SemestralnaPraca.Essentials;

namespace IOA_SemestralnaPraca.Structures
{
    public class ForwardStar
    {
        public List<int> NodePointers { get; }
        public List<Edge> EdgesArray { get; }

        public ForwardStar(List<Node> nodes, List<Edge> edges)
        {
            NodePointers = new List<int>();
            EdgesArray = new List<Edge>();

            Remake(nodes, edges);
        }

        public void Remake(List<Node> nodes, List<Edge> edges)
        {
            nodes = nodes.OrderBy(x => x.ID).ToList();
            edges = edges.OrderBy(x => x.NodeA.ID).ToList();

            NodePointers.Clear();
            EdgesArray.Clear();

            int edgeIndex = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                NodePointers.Add(edgeIndex);
                while ((edgeIndex < edges.Count) && (edges[edgeIndex].NodeA.ID == nodes[i].ID))
                {
                    EdgesArray.Add(edges[edgeIndex]);
                    edgeIndex++;
                }
            }
        }

        public (bool,Edge) FindConnection(int nodeA, int nodeB) 
        {
            if (nodeA > nodeB)
            {
                int tmp = nodeA;
                nodeA = nodeB;
                nodeB = tmp;
            }

            for (int i = NodePointers[nodeA - 1]; i < NodePointers[nodeB - 1]; i++)
            {
                if (EdgesArray[i].NodeA.ID == nodeA && EdgesArray[i].NodeB.ID == nodeB)
                    return (true, EdgesArray[i]);
            }

            return (false, new Edge());
        }
    }
}