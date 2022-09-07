using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[RequireComponent(typeof(FindPath))]
public class Grid : MonoBehaviour
{
	public Terrain terrain;

	/// <summary>
	/// The size of the terrain
	/// </summary>
	private Vector2 terrainSize;

	/// <summary>
	/// The size of the grid i.e [32,32] [64,64] etc.
	/// </summary>
	[SerializeField]
	private Vector2Int gridSize;

	/// <summary>
	/// 2d Array of grid nodes
	/// </summary>
	private Node[,] grid;

	private float nodeDiameter, nodeRadius;

	void Start()
	{
		DefineTerrainSize();

		SetNodeSize();
		DefineGridSize();
		CreateGrid();
	}

	/// <summary>
	/// Gets the node diameter using the terrain size & heightmap resolution
	/// </summary>
	/// <returns></returns>
	private float GetNodeDiameter()
	{
		float xSize = terrainSize.x / (terrain.terrainData.heightmapResolution - 1);
		float zSize = terrainSize.y / (terrain.terrainData.heightmapResolution - 1);
		//float multiplier = (float)terrain.terrainData.heightmapResolution / (float)(terrain.terrainData.heightmapResolution - 1);

		float retVal = (xSize + zSize) / 2;
		//retVal *= multiplier;

		return retVal;
	}

	/// <summary>
	/// Sets the diameter & radius of the nodes
	/// </summary>
	private void SetNodeSize()
	{
		nodeDiameter = GetNodeDiameter();
		nodeRadius = nodeDiameter / 2;
	}

	/// <summary>
	/// Defines the terrain size using the heightmap as a reference
	/// </summary>
	private void DefineTerrainSize()
	{
		terrainSize.x = terrain.terrainData.size.x;
		terrainSize.y = terrain.terrainData.size.z;
	}

	/// <summary>
	/// Defines the grid size 
	/// </summary>
	private void DefineGridSize()
	{
		gridSize.x = Mathf.RoundToInt((terrainSize.x / nodeDiameter));
		gridSize.y = Mathf.RoundToInt((terrainSize.x / nodeDiameter));
	}

	/// <summary>
	/// Creates a grid of nodes on the terrain using the heightmap as a reference
	/// </summary>
	void CreateGrid()
	{
		grid = new Node[gridSize.x, gridSize.y];

		for (int x = 0; x < gridSize.x; x++)
		{
			for (int y = 0; y < gridSize.y; y++)
			{
				Vector3 worldPos = terrain.HeightmapToWorldPosition(new Vector2Int(x, y)) + (Vector3.right * nodeDiameter) + (Vector3.forward * nodeDiameter);
				Vector3 snappedPoint = terrain.WorldPositionToSnappedSurfaceWorldPosition(worldPos);
				grid[x, y] = new Node(worldPos, new Vector2Int(x, y), (int)snappedPoint.y);
			}
		}
	}

	/// <summary>
	/// Get the nearest neighbour nodes the the current node
	/// </summary>
	/// <param name="_node"></param>
	/// <returns></returns>
	public List<Node> GetNeighbours(Node _node)
	{
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = _node.gridPosition.x + x;
				int checkY = _node.gridPosition.y + y;

				if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
				{
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}

		return neighbours;
	}

	/// <summary>
	/// Returns the grid position of the vector 3
	/// </summary>
	/// <param name="_worldPos"></param>
	/// <returns></returns>
	public Node NodeFromWorldPoint(Vector3 _worldPos)
	{
		return grid[terrain.WorldToHeightmapPosition(_worldPos).x, terrain.WorldToHeightmapPosition(_worldPos).y];
	}

	//void OnDrawGizmos()
	//{
	//	if (grid != null)
	//	{
	//		Node startNode = NodeFromWorldPoint(GetComponent<FindPath>()._locationStart.position);
	//		Node targetNode = NodeFromWorldPoint(GetComponent<FindPath>()._locationTarget.position);

	//		foreach (Node n in grid)
	//		{
	//			Gizmos.color = Color.white;
	//			if (n == startNode || n == targetNode)
	//			{
	//				if (n == startNode)
	//					Gizmos.color = Color.blue;
	//				else if (n == targetNode)
	//					Gizmos.color = Color.green;
	//				Gizmos.DrawCube(terrain.WorldPositionToSnappedSurfaceWorldPosition(n.worldPosition), Vector3.one * (nodeDiameter - .5f));

	//			}

	//		}
	//	}
	//}
}