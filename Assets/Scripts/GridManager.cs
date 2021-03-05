using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Singleton
    public static GridManager Instance;

    
    [SerializeField] GameObject NodePreafab;

    [HideInInspector]
    public bool HasDiagonalNeighbours = true;
    [HideInInspector]
    public int SizeX;
    [HideInInspector]
    public int SizeY;
    [HideInInspector]
    public Node startNode;
    [HideInInspector]
    public Node endNode;
    [HideInInspector]
    public bool noPathFound = false;
    [HideInInspector]
    public Node[,] AllNodes { get; protected set; }
    
    private GameObject tempObject;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        CreateGrid();
    }

    public void CreateGrid()
    {
        DestroyPreviousGrid();

        AllNodes = null;
        AllNodes = new Node[SizeX, SizeY];

        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                GameObject nodeGo = Instantiate(NodePreafab);
                nodeGo.transform.parent = gameObject.transform;
                nodeGo.name = "Node " + x + ", " + y;
                var node = nodeGo.AddComponent<Node>();
                node.Position = new Vector3(x, y, 0);
                node.transform.position = new Vector3(x - SizeX / 2, 0, y - SizeY / 2);
                AllNodes[x, y] = node;
            }
        }

        AssignNeighbours();
    }

    public void AssignNeighbours()
    {
        
        //Assign neighbours
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                var node = AllNodes[x, y];
                node.Neighbours.Clear();

                bool isLeftEdge = x == 0;
                bool isBottomEdge = y == 0;
                bool isRightEdge = x == SizeX - 1;
                bool isTopEdge = y == SizeY - 1;

                if (!isTopEdge)
                {
                    node.Neighbours.Add(AllNodes[x, y + 1]);
                }
                if (!isTopEdge && !isLeftEdge && HasDiagonalNeighbours)
                {
                    node.Neighbours.Add(AllNodes[x - 1, y + 1]);
                }
                if (!isLeftEdge)
                {
                    node.Neighbours.Add(AllNodes[x - 1, y]);
                }
                if (!isBottomEdge && !isLeftEdge && HasDiagonalNeighbours)
                {
                    node.Neighbours.Add(AllNodes[x - 1, y - 1]);
                }
                if (!isBottomEdge)
                {
                    node.Neighbours.Add(AllNodes[x, y - 1]);
                }
                if (!isBottomEdge && !isRightEdge && HasDiagonalNeighbours)
                {
                    node.Neighbours.Add(AllNodes[x + 1, y - 1]);
                }
                if (!isRightEdge)
                {
                    node.Neighbours.Add(AllNodes[x + 1, y]);
                }
                if (!isTopEdge && !isRightEdge && HasDiagonalNeighbours)
                {
                    node.Neighbours.Add(AllNodes[x + 1, y + 1]);
                }
            }
        }
    }

    private void DestroyPreviousGrid()
    {
        // Destroy all node gameobjects
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
