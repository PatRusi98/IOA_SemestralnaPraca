namespace IOA_SemestralnaPraca.Essentials
{
    public class Edge : IComparable<Edge>
    {
        public Node NodeA { get; set; }
        public Node NodeB { get; set; }
        public double Distance { get; set; }

        public int CompareTo(Edge? other)
        {
            int comparison = NodeA.ID.CompareTo(other?.NodeA.ID);
            if (comparison == 0)
            {
                if (NodeB != null && other?.NodeB != null)
                {
                    comparison = NodeB.ID.CompareTo(other?.NodeB.ID);
                }
                else if (NodeB == null && other?.NodeB != null)
                {
                    comparison = -1;
                }
                else if (NodeB != null && other?.NodeB == null)
                {
                    comparison = 1;
                }
                else
                {
                    comparison = 0;
                }
            }
            return comparison;
        }
    }
}