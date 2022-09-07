using UnityEngine;
using System.Collections;

[System.Serializable]
public class Node {
	
	public Vector3 worldPosition;
	public Vector2Int gridPosition;

	public int gCost;
	public int hCost;
	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}

	/// <summary>
	/// Height of the node on the heightmap
	/// </summary>
	public int height;

	/// <summary>
	/// Movement penalty (higher if the height of target node is greater than that of current node)
	/// </summary>
	public int movementPenalty;

	public Node parent;
	
	public Node(Vector3 _worldPos, Vector2Int _gridPosition, int _height) {
		worldPosition = _worldPos;
		gridPosition = _gridPosition;
		height = _height;
	}
	
}
