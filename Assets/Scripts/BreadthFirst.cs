using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadthFirst : IPathfinding
{
    public IList<Node> FindPath(Node start, Node end)
    {
        List<Node> foundPath = new List<Node>();

        Queue<Node> frontier = new Queue<Node>();
        frontier.Enqueue(start);
        Dictionary<Node, Node> directionMap = new Dictionary<Node, Node>();
        directionMap.Add(start, null);

        while (frontier.Count > 0)
        {
            var currentNode = frontier.Dequeue();

            if (currentNode == end)
            {
                break;
            }

            foreach (var neighbour in currentNode.Neighbours)
            {
                if (!directionMap.ContainsKey(neighbour))
                {
                    Debug.DrawLine(currentNode.transform.position, neighbour.transform.position, Color.green, 5f);
                    directionMap.Add(neighbour, currentNode);

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
