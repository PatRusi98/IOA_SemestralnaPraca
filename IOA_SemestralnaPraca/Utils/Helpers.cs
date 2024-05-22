using IOA_SemestralnaPraca.Essentials;

namespace IOA_SemestralnaPraca.Utils
{
    public class Helpers
    {
        public static double EuclideanDistance(Coordinates a, Coordinates b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
    }
}
