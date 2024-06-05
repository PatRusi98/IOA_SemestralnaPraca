using IOA_SemestralnaPraca.Essentials;
using IOA_SemestralnaPraca.Structures;
using static IOA_SemestralnaPraca.Utils.Enums;

namespace IOA_SemestralnaPraca.Algorithm
{
    public class ClarkeWright
    {
        private readonly ForwardStar _forwardStar;
        private readonly Dictionary<int, Node> _nodes;
        private readonly Node _depot;
        private readonly double _vehicleCapacity;
        private readonly Dictionary<int, double> _depotDistances;

        public ClarkeWright(ForwardStar forwardStar, List<Node> nodes, double vehicleCapacity)
        {
            _forwardStar = forwardStar;
            _nodes = nodes.ToDictionary(n => n.ID);
            _depot = nodes.First(n => n.Type == NodeType.PrimarySource);
            _vehicleCapacity = vehicleCapacity;
            var dijkstra = new Dijkstra(_forwardStar, nodes);
            _depotDistances = dijkstra.FindShortestPaths(_depot.ID);
        }

        public List<List<int>> Execute()
        {
            var savings = CalculateSavings();
            savings.Sort((s1, s2) => s2.Value.CompareTo(s1.Value));

            var routes = new Dictionary<int, List<int>>();
            var routeCapacities = new Dictionary<int, double>();

            foreach (var node in _nodes.Values)
            {
                if (node.Type == NodeType.Customer)
                {
                    routes[node.ID] = new List<int> { _depot.ID, node.ID, _depot.ID };
                    routeCapacities[node.ID] = node.Capacity;
                }
            }

            foreach (var saving in savings)
            {
                var routeA = routes[saving.NodeA];
                var routeB = routes[saving.NodeB];
                var capacityA = routeCapacities[saving.NodeA];
                var capacityB = routeCapacities[saving.NodeB];

                if (routeA == routeB) continue;

                if (routeA.Last() == _depot.ID && routeB.First() == _depot.ID)
                {
                    if (capacityA + capacityB <= _vehicleCapacity)
                    {
                        routeA.RemoveAt(routeA.Count - 1); // Remove the last depot from routeA
                        routeA.AddRange(routeB.Skip(1)); // Merge routes, skip the first depot in routeB
                        routes[saving.NodeA] = routeA;
                        routes[saving.NodeB] = routeA;

                        foreach (var node in routeB)
                        {
                            routes[node] = routeA;
                        }

                        routeCapacities[saving.NodeA] = capacityA + capacityB;
                        routeCapacities[saving.NodeB] = capacityA + capacityB;
                    }
                }
            }

            // Remove duplicate routes and ensure each route starts and ends with the depot
            var distinctRoutes = routes.Values.Distinct().Select(r =>
            {
                if (r.First() != _depot.ID) r.Insert(0, _depot.ID);
                if (r.Last() != _depot.ID) r.Add(_depot.ID);
                return r;
            }).ToList();

            return distinctRoutes;
        }

        private List<Saving> CalculateSavings()
        {
            var savings = new List<Saving>();

            foreach (var edge in _forwardStar.EdgesArray)
            {
                if (edge.NodeA.ID == _depot.ID || edge.NodeB.ID == _depot.ID) continue;

                double distanceDepotA = _depotDistances[edge.NodeA.ID];
                double distanceDepotB = _depotDistances[edge.NodeB.ID];
                double savingValue = distanceDepotA + distanceDepotB - edge.Distance;

                savings.Add(new Saving(edge.NodeA.ID, edge.NodeB.ID, savingValue));
            }

            return savings;
        }
    }

    public class Saving
    {
        public int NodeA { get; set; }
        public int NodeB { get; set; }
        public double Value { get; set; }

        public Saving(int nodeA, int nodeB, double value)
        {
            NodeA = nodeA;
            NodeB = nodeB;
            Value = value;
        }
    }
}
