using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using System;

public class FindPath : MonoBehaviour
{
    public bool updateInPlayMode = true;
    [Header("Scene Links")]
    
    [Tooltip("Link to the starting position of the path.")]
    public Transform _locationStart;

    [Tooltip("Link to the target position of the path.")]
    public Transform _locationTarget;
    
    //[Tooltip("Link to the terrain to be used to calculate costs.")]
    //public Terrain _terrain;

    [Header("Generated Path Points")]

    [SerializeField] [ReadOnly] List<Vector2Int> _pathPoint = new List<Vector2Int>();


    private Grid grid;

    [Header("The higher the multiplier the stricter the movement penalty on higher terrain")]
    /// <summary>
    /// Multiplier added to movementPenalty if threshold is breached
    /// </summary>
    public float penaltyMultiplier;
    [Header("The lower the threshold the stricter the movement penalty on higher terrain")]
    /// <summary>
    /// Threshold between height values before adding a penalty multiplier
    /// </summary>
    public int penaltyThreshold;

    private Node startNode, targetNode;
    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        if (updateInPlayMode)
        {
            DoFindPath();
        }
    }

    public void DoFindPath()
    {
        startNode = grid.NodeFromWorldPoint(_locationStart.transform.position);
        targetNode = grid.NodeFromWorldPoint(_locationTarget.transform.position);

        startNode.parent = startNode;

        if (startNode != null && targetNode != null)
        {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];

                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost)
                    {
                        if (openSet[i].hCost < currentNode.hCost)
                            currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    _pathPoint = RetracePath(startNode, targetNode);
                    break;
                }

                CalculateMovement(currentNode, targetNode, openSet, closedSet);
            }
        }
    }

    private void CalculateMovement(Node _curNode, Node _target, List<Node> _openSet, HashSet<Node> _closeSet)
    {
        foreach (Node neighbour in grid.GetNeighbours(_curNode))
        {
            CalculateMovementPenalty(_curNode, neighbour);

            if (_closeSet.Contains(neighbour))
                continue;

            CalculateMovementCost(_curNode, neighbour, _target, _openSet);
        }
    }


    private void CalculateMovementCost(Node _curNode, Node _neighbour, Node _target, List<Node> _openSet)
    {
        int newMovementCostToNeighbour = _curNode.gCost + GetDistance(_curNode, _neighbour) + _neighbour.movementPenalty;
        if (newMovementCostToNeighbour < _neighbour.gCost || !_openSet.Contains(_neighbour))
        {
            _neighbour.gCost = newMovementCostToNeighbour;
            _neighbour.hCost = GetDistance(_neighbour, _target);
            _neighbour.parent = _curNode;

          //  Debug.Log("Cost to N: " + newMovementCostToNeighbour + " N gCost: " + _neighbour.gCost);

            if (!_openSet.Contains(_neighbour))
                _openSet.Add(_neighbour);
            else
                _openSet.Remove(_neighbour);
        }
    }

    /// <summary>
    /// Compares the height of 2 nodes. If the neighbour node is taller than the curNode but within our upper limit, we add a small increment, else add a large incriment.
    /// This will allow us to take the path with the least upward travel
    /// </summary>
    /// <param name="_curNode"></param>
    /// <param name="_neighbour"></param>
    private void CalculateMovementPenalty(Node _curNode, Node _neighbour)
    {
        _curNode.movementPenalty = 0;
        _neighbour.movementPenalty = _neighbour.height - _curNode.height;

        if (_neighbour.height >= _curNode.height)
        {
            if (_neighbour.movementPenalty > penaltyThreshold)
                _neighbour.movementPenalty = (int)(_neighbour.movementPenalty * penaltyMultiplier);
        }
        else
        {
            if (_neighbour.movementPenalty < -penaltyThreshold)
                _neighbour.movementPenalty = (int)(_neighbour.movementPenalty * penaltyMultiplier);
        }
    }

    List <Vector2Int> RetracePath(Node startNode, Node endNode)
    {
        _pathPoint.Clear();
        List<Vector2Int> vectorPath = new List<Vector2Int>();
        List<Node> nodePath = new List<Node>();

        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            if (!vectorPath.Contains(currentNode.gridPosition))
                vectorPath.Add(currentNode.gridPosition);

            if (!nodePath.Contains(currentNode))
                nodePath.Add(currentNode);

            currentNode = currentNode.parent;
        }

        vectorPath.Reverse();
        nodePath.Reverse();

        return vectorPath;

    }

    /// <summary>
    /// Return distance between 2 nodes
    /// </summary>
    /// <param name="_nodeA"></param>
    /// <param name="_nodeB"></param>
    /// <returns></returns>
    private int GetDistance(Node _nodeA, Node _nodeB)
    {
        int distX = Mathf.Abs(_nodeA.gridPosition.x - _nodeB.gridPosition.x);
        int distY = Mathf.Abs(_nodeA.gridPosition.y - _nodeB.gridPosition.y);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        else
            return 14 * distX + 10 * (distY - distX);
    }


    private void OnDrawGizmos()
    {
        // Draw the path as a series of line segments for debugging.

        if (_pathPoint.Count > 0)
        {
            Vector2Int gridPos = _pathPoint[0];

            for (int r = 1; r < _pathPoint.Count; r++)
            {
                Vector2Int nextGridPos = _pathPoint[r];
                Handles.color = Color.yellow;
                Handles.DrawLine(grid.terrain.HeightmapToSurfaceWorldPosition(gridPos),
                    grid.terrain.HeightmapToSurfaceWorldPosition(nextGridPos), 5);

                gridPos = nextGridPos;
            }
        }
    }
    
}

[CustomEditor(typeof(FindPath))]
public class FindPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Create a button to call the DoFindPath() method from the inspector panel.
        FindPath findPath = (FindPath)target;
        if (GUILayout.Button("Calculate Path"))
        {
            findPath.DoFindPath();
        }

        // Draw the data items exposed from the class as normal.
        DrawDefaultInspector();
    }
}
