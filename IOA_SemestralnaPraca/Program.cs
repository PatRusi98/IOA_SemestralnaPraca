using IOA_SemestralnaPraca.Utils;

namespace IOA_SemestralnaPraca
{
    public static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            var nodesFile = "nodes.txt";
            var edgesFile = "edges.txt";

            //FileHandler handler = new FileHandler();
            //var nodes = handler.LoadNodes(nodesFile);
            //var edges = handler.LoadConnections(edgesFile, nodes);

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}