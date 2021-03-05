using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public enum PathfindingMethod
    {
        BreadthFirst,
        Dijkstra,
        AStar,
        AStarReversed
    }

    [HideInInspector]
    public PathfindingMethod Method;
    public GridManager Grid;

    public IList<Node> currentPath;

    public void FindPath(PathfindingMethod method, Node startNode, Node endNode, Material pathMat)
    {
        IPathfinding pathfinder = null;

        switch (method)
        {
            case PathfindingMethod.BreadthFirst:
                pathfinder = new BreadthFirst();
                break;
            case PathfindingMethod.Dijkstra:
                pathfinder = new Dijkstra();
                break;
            case PathfindingMethod.AStar:
                pathfinder = new AStar();
                break;
            case PathfindingMethod.AStarReversed:
                pathfinder = new AStarReversed();
                break;
        }

        try
        {
            currentPath = pathfinder.FindPath(startNode, endNode);

            if (currentPath != null)
            {
                foreach (var node in currentPath)
                {
                    if (node != startNode && node != endNode)
                        node.gameObject.GetComponent<MeshRenderer>().material = pathMat;
                }
            }
        }
        catch
        {
            // No path available
            GridManager.Instance.noPathFound = true;
        } 
    }
}
