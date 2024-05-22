namespace IOA_SemestralnaPraca.Essentials
{
    public class Edge
    {
        public required Node NodeA { get; set; }
        public required Node NodeB { get; set; }
        public double Distance { get; set; }
    }
}