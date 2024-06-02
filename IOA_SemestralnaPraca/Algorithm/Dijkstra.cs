using IOA_SemestralnaPraca.Essentials;
using IOA_SemestralnaPraca.Structures;

namespace IOA_SemestralnaPraca.Algorithm
{
    public class Dijkstra
    {
        private readonly ForwardStar _forwardStar;
        private readonly Dictionary<int, Node> _nodes;

        public Dijkstra(ForwardStar forwardStar, List<Node> nodes)
        {
            _forwardStar = forwardStar;
            _nodes = nodes.ToDictionary(n => n.ID);
        }

        public Dictionary<int, double> FindShortestPaths(int startNodeId)
        {
            var distances = new Dictionary<int, double>();
            var priorityQueue = new SortedSet<(double Distance, int NodeId)>();
            var visited = new HashSet<int>();

            foreach (var node in _nodes.Values)
            {
                distances[node.ID] = double.MaxValue;
            }
            distances[startNodeId] = 0;
            priorityQueue.Add((0, startNodeId));

            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Min;
                priorityQueue.Remove(current);
                int currentNodeId = current.NodeId;

                if (visited.Contains(currentNodeId))
                {
                    continue;
                }

                visited.Add(currentNodeId);
                int startIndex = _forwardStar.NodePointers[currentNodeId - 1]; // Upravené pre ID začínajúce od 1
                int endIndex = (currentNodeId < _forwardStar.NodePointers.Count) ? _forwardStar.NodePointers[currentNodeId] : _forwardStar.EdgesArray.Count;

                for (int i = startIndex; i < endIndex; i++)
                {
                    var edge = _forwardStar.EdgesArray[i];
                    int neighborId = edge.NodeB.ID;
                    double newDist = distances[currentNodeId] + edge.Distance;

                    if (newDist < distances[neighborId])
                    {
                        distances[neighborId] = newDist;
                        priorityQueue.Add((newDist, neighborId));
                    }
                }
            }

            return distances;
        }

        public double[,] CreateDistanceMatrix()
        {
            int size = _nodes.Count;
            double[,] distanceMatrix = new double[size, size];

            foreach (var node in _nodes.Values)
            {
                var distances = FindShortestPaths(node.ID);
                foreach (var targetNode in _nodes.Values)
                {
                    distanceMatrix[node.ID - 1, targetNode.ID - 1] = distances[targetNode.ID]; // Upravené pre ID začínajúce od 1
                }
            }

            return distanceMatrix;
        }
    }
}
