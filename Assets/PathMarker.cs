using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathMarker : MonoBehaviour
{
    [Tooltip("A link to the terrain to snap to.")]
    [SerializeField] private Terrain _terrain;
    
    [Tooltip("The colour of the debug cube.")]
    [SerializeField] Color _color = Color.red;

    private void Awake()
    {
        _terrain = FindObjectOfType<Terrain>();
    }
    private void OnDrawGizmos()
    {
        if (_terrain != null)
        {
            // Find the position under the marker on the terrain.
            Vector3 surfacePosition = _terrain.WorldPositionToSnappedSurfaceWorldPosition(transform.position);

            // Draw a cube on this position for debugging.
            Gizmos.color = _color;
            Gizmos.DrawCube(surfacePosition, new Vector3(20.0f, 20.0f, 20.0f));
        }
    }
}
