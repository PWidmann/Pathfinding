using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dijkstra : IPathfinding
{
    public IList<Node> FindPath(Node start, Node end)
    {
        List<Node> foundPath = new List<Node>();
        PriorityQueue<Node> frontier = new PriorityQueue<Node>();
        Dictionary<Node, Node> directionMap = new Dictionary<Node, Node>();
        Dictionary<Node, int> distanceMap = new Dictionary<Node, int>();

        frontier.Enqueue(start);
        directionMap.Add(start, null);
        distanceMap.Add(start, 0);
        start.Distance = 0;

        while (frontier.Count > 0)
        {
            var currentNode = frontier.Dequeue();
            
            if (currentNode == end)
                break;

            foreach (var neighbour in currentNode.Neighbours)
            {
                if (!directionMap.ContainsKey(neighbour))
                {
                    directionMap.Add(neighbour, currentNode);
                    neighbour.Distance = currentNode.Distance + 1;

                    if (neighbour.Passability != 0)
                        frontier.Enqueue(neighbour);
                }
            }
        }

        var pathNode = end;
        while (pathNode != null)
        {
            foundPath.Add(pathNode);
            pathNode = directionMap[pathNode];
        }

        foundPath.Reverse();
        return foundPath;
    }
}
