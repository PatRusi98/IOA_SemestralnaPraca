using IOA_SemestralnaPraca.Algorithm;
using IOA_SemestralnaPraca.Structures;
using IOA_SemestralnaPraca.Utils;

namespace IOA_SemestralnaPraca
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}