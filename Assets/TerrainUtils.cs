using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainUtils
{
    public static Vector2Int WorldToHeightmapPosition(this Terrain terrain, Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - terrain.transform.position;

        float xScale = (float)terrain.terrainData.heightmapResolution / terrain.terrainData.size.x;
        float zScale = (float)terrain.terrainData.heightmapResolution / terrain.terrainData.size.z;

        return new Vector2Int((int)(localPosition.x * xScale), (int)(localPosition.z * zScale));
    }

    //public static Vector2Int NodeFromWorldPoint(this Terrain terrain, Vector3 worldPosition)
    //{
    //    return new Vector2Int(terrain.WorldToHeightmapPosition(worldPosition).x, terrain.WorldToHeightmapPosition(worldPosition).y);
    //}

    public static Vector3 HeightmapToWorldPosition(this Terrain terrain, Vector2Int heightmapPosition)
    {
        float xScale = terrain.terrainData.size.x / terrain.terrainData.heightmapResolution;
        float zScale = terrain.terrainData.size.z / terrain.terrainData.heightmapResolution;

        Vector3 worldPosition = terrain.transform.position +
            new Vector3((heightmapPosition.x * xScale), 0.0f, (heightmapPosition.y * zScale));

        return worldPosition;
    }

    public static Vector3 HeightmapToSurfaceWorldPosition(this Terrain terrain, Vector2Int heightmapPosition)
    {
        Vector3 worldPosition = HeightmapToWorldPosition(terrain, heightmapPosition);
        worldPosition.y = terrain.SampleHeight(worldPosition);
        return worldPosition;
    }
    
    // Converts a world space position to a world space position on the surface of the terrain.
    public static Vector3 WorldPositionToSurfaceWorldPosition(this Terrain terrain, Vector3 worldPosition)
    {
        worldPosition.y = terrain.SampleHeight(worldPosition);
        return worldPosition;
    }

    // Converts a world space position to a world space position on the surface of the terrain, but snapped to the
    // corner of a heightmap pixel.
    public static Vector3 WorldPositionToSnappedSurfaceWorldPosition(this Terrain terrain, Vector3 worldPosition)
    {
        Vector2Int heightmapPosition = terrain.WorldToHeightmapPosition(worldPosition);
        return HeightmapToSurfaceWorldPosition(terrain, heightmapPosition);
    }

}
