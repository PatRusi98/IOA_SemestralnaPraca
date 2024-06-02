using static IOA_SemestralnaPraca.Utils.Enums;

namespace IOA_SemestralnaPraca.Essentials
{
    public class Node : IComparable<Node>
    {
        public int ID { get; set; }
        public NodeType Type { get; set; }
        public double Capacity { get; set; }
        public Coordinates? Coordinates { get; set; }
        public bool Selected { get; set; }

        public int CompareTo(Node? other)
        {
            return ID.CompareTo(other?.ID);
        }
    }
}