using IOA_SemestralnaPraca.Essentials;
using static IOA_SemestralnaPraca.Utils.Enums;
using static IOA_SemestralnaPraca.Utils.Helpers;

namespace IOA_SemestralnaPraca.Utils
{
    public class FileHandler
    {
        public List<Node> LoadNodes(string path)
        {
            List<Node> loadedNodes = new List<Node>();
            string? line;
            var counter = 0;
            var nodeNumber = int.MinValue;
            var nodeCapacity = double.MinValue;

            StreamReader file = new StreamReader(path);

            while ((line = file.ReadLine()) != null)
            {
                if (counter % 2 == 0)
                {
                    var lineVal = line.Split('-');
                    nodeNumber = int.Parse(lineVal[0]);
                    nodeCapacity = double.Parse(lineVal[1]);
                }
                else
                {
                    var lineVal = line.Split(',');
                    loadedNodes.Add(new Node
                    {
                        ID = nodeNumber,
                        Coordinates = new Coordinates
                        {
                            X = double.Parse(lineVal[0]),
                            Y = double.Parse(lineVal[1])
                        },
                        Type = nodeNumber == 1 ? NodeType.PrimarySource : NodeType.Customer,
                        Capacity = nodeCapacity,
                        Selected = false
                    });
                }
                counter++;
            }
            file.Close();

            return loadedNodes;
        }

        public List<Edge> LoadConnections(string path, List<Node> nodes)
        {
            if (nodes.Count == 0)
            {
                throw new Exception("Nodes are not loaded yet");
            }

            List<Edge> loadedEdges = new List<Edge>();
            string? line;

            StreamReader file = new StreamReader(path);
            bool twoway = false;
            switch (file.ReadLine())
            {
                case "oneway":
                    twoway = false;
                    break;
                case "twoway":
                    twoway = true;
                    break;
                default:
                    throw new Exception("Invalid file format");
            }

            while ((line = file.ReadLine()) != null)
            {
                var lineVal = line.Split(',');
                var nodeA = nodes.FirstOrDefault(x => x.ID == int.Parse(lineVal[0]));
                var nodeB = nodes.FirstOrDefault(x => x.ID == int.Parse(lineVal[1]));

                if (nodeA == null || nodeB == null)
                {
                    throw new Exception("Node not found");
                }

                loadedEdges.Add(new Edge
                {
                    NodeA = nodeA,
                    NodeB = nodeB,
                    Distance = EuclideanDistance(nodeA.Coordinates, nodeB.Coordinates)
                });

                if (twoway)
                {
                    loadedEdges.Add(new Edge
                    {
                        NodeA = nodeB,
                        NodeB = nodeA,
                        Distance = EuclideanDistance(nodeB.Coordinates, nodeA.Coordinates)
                    });
                }
            }
            file.Close();

            return loadedEdges;
        }

        public void SaveNodes(string path, List<Node> nodes)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (var node in nodes)
                {
                    writer.WriteLine($"{node.ID}-{node.Capacity}");
                    writer.WriteLine($"{node.Coordinates.X},{node.Coordinates.Y}");
                }
            }
        }

        public void SaveConnections(string path, List<Edge> edges)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("twoway");
                foreach (var edge in edges)
                {
                    writer.WriteLine($"{edge.NodeA.ID},{edge.NodeB.ID}");
                }
            }
        }
    }
}
