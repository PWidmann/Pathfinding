using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class GameInterface : MonoBehaviour
{
    [Header("Interface References")]
    [SerializeField] GameObject newGridPanel;
    [SerializeField] InputField gridXsize;
    [SerializeField] InputField gridYsize;
    [SerializeField] GameObject logPanel;
    [SerializeField] Text logText;
    [SerializeField] Dropdown dropDownMenu;
    [SerializeField] Toggle diagonalToggle;

    [Header("Materials")]
    [SerializeField] Material startMat;
    [SerializeField] Material endMat;
    [SerializeField] public Material pathMat;
    [SerializeField] Material wallMat;
    [SerializeField] Material defaultMat;

    [Header("Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button endButton;
    [SerializeField] Button wallButton;

    [Header("Pathfinder reference")]
    [SerializeField] Pathfinder pathFinder;

    [HideInInspector]
    public bool isSettingStart = false;
    [HideInInspector]
    public bool isSettingEnd = false;
    [HideInInspector]
    public bool isDrawingWalls = false;

    // Node raycasting
    Camera cam;
    Ray ray;
    RaycastHit hit;
    GameObject lastEditedStart;
    GameObject lastEditedEnd;
    Node lastNode;

    Stopwatch watch;
    float dropDownValue;

    private void Start()
    {
        cam = Camera.main;
        watch = Stopwatch.StartNew();
        newGridPanel.SetActive(false);
        GridManager.Instance.HasDiagonalNeighbours = diagonalToggle.isOn;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        DrawMapFunctions();
        ButtonColors();
    }

    private void ButtonColors()
    {
        // Set start button
        if (isSettingStart)
            startButton.image.color = Color.green;
        else
            startButton.image.color = Color.white;

        // Set end button
        if (isSettingEnd)
            endButton.image.color = Color.cyan;
        else
            endButton.image.color = Color.white;

        // Set walls button
        if (isDrawingWalls)
            wallButton.image.color = Color.grey;
        else
            wallButton.image.color = Color.white;
    }

    private void DrawMapFunctions()
    {
        // Set start point
        if (isSettingStart && Input.GetMouseButtonDown(0))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                // start can only exist once, reset other start nodes
                if (lastEditedStart != null)
                    lastEditedStart.transform.GetComponent<MeshRenderer>().material = defaultMat;

                GridManager.Instance.startNode = hit.transform.GetComponent<Node>();
                hit.transform.GetComponent<MeshRenderer>().material = startMat;
                hit.transform.GetComponent<Node>().Passability = 0.5f;
                lastEditedStart = hit.transform.gameObject;

                isSettingStart = false;
            }
        }

        // Set end point
        if (isSettingEnd && Input.GetMouseButtonDown(0))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                // end can only exist once, reset other start nodes
                if (lastEditedEnd != null)
                    lastEditedEnd.transform.GetComponent<MeshRenderer>().material = defaultMat;

                GridManager.Instance.endNode = hit.transform.GetComponent<Node>();
                hit.transform.GetComponent<MeshRenderer>().material = endMat;
                hit.transform.GetComponent<Node>().Passability = 0.5f;
                lastEditedEnd = hit.transform.gameObject;

                isSettingEnd = false;
            }
        }

        // Set walls
        if (isDrawingWalls && Input.GetMouseButton(0))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.GetComponent<Node>() != lastNode)
                {
                    if (hit.transform.GetComponent<Node>().Passability == 1) // if it's not a wall
                    {
                        hit.transform.GetComponent<MeshRenderer>().material = wallMat;
                        hit.transform.GetComponent<Node>().Passability = 0;
                        lastNode = hit.transform.GetComponent<Node>();
                    }
                    else if (hit.transform.GetComponent<Node>().Passability == 0.5f) // if it is a start/end point
                    {
                        // Do nothing
                    }
                    else // if it is a wall
                    {
                        hit.transform.GetComponent<MeshRenderer>().material = defaultMat;
                        hit.transform.GetComponent<Node>().Passability = 1;
                        lastNode = hit.transform.GetComponent<Node>();
                    }
                }
            }
        }
    }

    public void OpenNewGridPanel()
    {
        newGridPanel.SetActive(!newGridPanel.activeSelf);
    }

    public void ConfirmNewGrid()
    {
        if (gridXsize != null && gridYsize != null)
        {
            // Grid x size
            if (gridXsize.text == "")
                GridManager.Instance.SizeX = 20;
            else
                GridManager.Instance.SizeX = int.Parse(gridXsize.text);

            // Grid y size
            if (gridYsize.text == "")
                GridManager.Instance.SizeY = 20;
            else
                GridManager.Instance.SizeY = int.Parse(gridYsize.text);

            // Create grid
            GridManager.Instance.CreateGrid();

            // empty values
            gridXsize.text = "";
            gridYsize.text = "";
            newGridPanel.SetActive(false);

            isDrawingWalls = false;
            isSettingStart = false;
            isSettingEnd = false;

            logPanel.SetActive(false);
        }
    }

    public void SetStart()
    {
        isSettingStart = true;
        isSettingEnd = false;
        isDrawingWalls = false;
    }

    public void SetEnd()
    {
        isSettingEnd = true;
        isSettingStart = false;
        isDrawingWalls = false;
    }

    public void SetDrawWalls()
    {
        isDrawingWalls = !isDrawingWalls;
    }

    public void ToggleChange()
    {
        GridManager.Instance.HasDiagonalNeighbours = diagonalToggle.isOn;
        GridManager.Instance.AssignNeighbours();
    }

    private void ResetPathWay()
    {
        if (pathFinder)
        {
            foreach (Node node in GridManager.Instance.AllNodes)
            {
                if (node != GridManager.Instance.startNode && node != GridManager.Instance.endNode && node.Passability != 0)
                {
                    node.gameObject.GetComponent<MeshRenderer>().material = defaultMat;
                }
            }
        }
    }

    public void StartSearch()
    {
        if (GridManager.Instance.startNode != null && GridManager.Instance.endNode != null)
        {
            ResetPathWay();

            dropDownValue = dropDownMenu.value;
            Pathfinder.PathfindingMethod method = new Pathfinder.PathfindingMethod();
            switch (dropDownValue)
            {
                case 0: // A*
                    method = Pathfinder.PathfindingMethod.AStar;
                    break;
                case 1: // Dijkstra
                    method = Pathfinder.PathfindingMethod.Dijkstra;
                    break;
                case 2: // BreadthFirst
                    method = Pathfinder.PathfindingMethod.BreadthFirst;
                    break;
                case 3: // A* Reversed (longest path)
                    method = Pathfinder.PathfindingMethod.AStarReversed;
                    break;
            }
            
            // Run pathfinding algorithm and count task time
            watch.Restart();
            pathFinder.FindPath(method, GridManager.Instance.startNode, GridManager.Instance.endNode, pathMat);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            logPanel.SetActive(true);

            if (GridManager.Instance.noPathFound)
            {
                logText.text = "No Path found!";
                GridManager.Instance.noPathFound = false;
            }
            else
            {
                logText.text = "Time to calculate path: " + elapsedMs + " ms.";
            }

            isDrawingWalls = false;
        }
    }
}
