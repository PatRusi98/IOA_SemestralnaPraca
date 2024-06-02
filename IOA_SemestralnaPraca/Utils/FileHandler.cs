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

            StreamReader file = new StreamReader(path);

            while ((line = file.ReadLine()) != null)
            {
                if (counter % 2 == 0)
                {
                    nodeNumber = int.Parse(line);
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
                        Type = NodeType.Unspecified,
                        Capacity = 0,
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

        public (List<Node>, List<Edge>) LoadMatrixDistance(string path)
        {
            //TODO
            var nodes = new List<Node>();
            var edges = new List<Edge>();

            throw new NotImplementedException();

            //return (nodes, edges);
        }

        public void SaveNodes(string path, List<Node> nodes)
        {
            //TODO
            throw new NotImplementedException();
        }

        public void SaveConnections(string path, List<Edge> edges)
        {
            //TODO
            throw new NotImplementedException();
        }

        public void SaveMatrixDistance(string path, List<Node> nodes, List<Edge> edges)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
