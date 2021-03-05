using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour, IComparable<Node>
{
    public List<Node> Neighbours = new List<Node>();
    public Vector3 Position;
    public float Passability = 1;
    public float Distance = 0;
    public bool isStart = false;
    public bool isEnd = false;

    public float Weight { get { return Distance * -1 + Passability; } }
    public int CompareTo(Node other)
    {
        if (Weight > other.Weight)
        {
            return -1;
        }
        else if (Weight < other.Weight)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
