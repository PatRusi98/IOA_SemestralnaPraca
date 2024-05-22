using static IOA_SemestralnaPraca.Utils.Enums;

namespace IOA_SemestralnaPraca.Essentials
{
    public class Node
    {
        public NodeType Type { get; set; }
        public int ID { get; set; }
        public double Capacity { get; set; }
        public Coordinates? Coordinates { get; set; }
    }
}