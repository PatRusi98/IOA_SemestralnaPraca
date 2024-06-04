using IOA_SemestralnaPraca.Algorithm;
using IOA_SemestralnaPraca.Structures;
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

            // TEST DO KONZOLY
            //var nodesFile = "_data/_defaultNodes.txt";
            //var edgesFile = "_data/_defaultEdges.txt";

            //FileHandler handler = new FileHandler();
            //var nodes = handler.LoadNodes(nodesFile);
            //var edges = handler.LoadConnections(edgesFile, nodes);

            //ForwardStar star = new ForwardStar(nodes, edges);
            //Dijkstra dijkstra = new Dijkstra(star, nodes);
            //ClarkeWright cw = new ClarkeWright(star, nodes, 100);

            //var distances = dijkstra.FindShortestPaths(5);
            //var matrix = dijkstra.CreateDistanceMatrix();
            //var clarkeWright = cw.Execute();

            //Console.WriteLine(cw.ToString());

            //System.Console.WriteLine("done");

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}