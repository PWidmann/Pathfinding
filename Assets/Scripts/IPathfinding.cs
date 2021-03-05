using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathfinding
{
    IList<Node> FindPath(Node start, Node end);
}
