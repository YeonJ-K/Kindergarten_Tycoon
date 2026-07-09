using System;
using System.Collections.Generic;
using UnityEngine;
using Infos;

public class GridCell
{
    public ZoneType zone = ZoneType.None;
    public bool isWall;
    public int objectId = -1;
    public bool IsWalkable => objectId == -1 && !isWall;
}

[System.Serializable]
public class ZoneRect
{
    public ZoneType type = ZoneType.MainRoom;
    public int x, y;
    public int width = 3, height = 3;
}

public class GridMap : MonoBehaviour
{
    [Header("맵 크기")]
    public int mapWidth = 11; // 추후에 값은 데이터 시트로 받아온다.
    public int mapHeight = 7;
    public float cellSize = 1f;

    [Header("Sections")] 
    public List<ZoneRect> presetZones = new();
    
    public GridCell[,] cells;
    
    void Awake()
    {
        BuildGrid();
    }

    public void BuildGrid()
    {
        cells = new GridCell[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
            for (int y = 0; y < mapHeight; y++)
                cells[x, y] = new GridCell();
        
        foreach (var z in presetZones)
            for (int x = z.x; x < z.x + z.width; x++)
                for (int y = z.y; y < z.y + z.height; y++)
                if (InBounds(x, y))
                    cells[x, y].zone = z.type;
    }   
    
    public Vector3 GridToWorld(int x, int y)
     => new Vector3(x * cellSize + cellSize / 2f, y * cellSize + cellSize / 2f, 0f);
    public Vector2Int WorldToGrid(Vector3 world)
     => new Vector2Int(Mathf.FloorToInt(world.x / cellSize), Mathf.FloorToInt(world.y / cellSize));
    
    public bool InBounds(int x, int y) => x >= 0 && x < mapWidth && y >= 0 && y < mapHeight;

    public float WorldWidth => mapWidth * cellSize;
    public float WorldHeight => mapHeight * cellSize;
    public Vector3 WorldCenter => transform.position + new Vector3(WorldWidth / 2f, WorldHeight / 2f, 0);
    private void OnDrawGizmos()
    {
        if (cells == null)
        {
            DrawEmptyGrid();
            return;
        }
        for (int x = 0; x < mapWidth; x++)
        for (int y = 0; y < mapHeight; y++)
        {
            Gizmos.color = ZoneColor(cells[x, y].zone);
            Vector3 center = GridToWorld(x, y);
            Gizmos.DrawCube(center, Vector3.one * cellSize * 0.9f);
        }
    }

    void DrawEmptyGrid()
    {
        Gizmos.color = Color.gray;
        for (int x = 0; x <= mapWidth; x++)
            Gizmos.DrawLine(new Vector3(x * cellSize, 0), new Vector3(x * cellSize, mapHeight * cellSize));
        for (int y = 0; y <= mapHeight; y++)
            Gizmos.DrawLine(new Vector3(0, y * cellSize), new Vector3(mapWidth * cellSize, y * cellSize));
        
        foreach (var z in presetZones)
            for (int x = z.x; x < z.x + z.width; x++)
                for (int y = z.y; y < z.y + z.height; y++)
                    if (InBounds(x, y))
                    {   
                        Gizmos.color = ZoneColor(z.type);
                        Gizmos.DrawCube(GridToWorld(x, y), Vector3.one * cellSize * 0.9f);
                    }
    }
    
    Color ZoneColor(ZoneType zone) => zone switch
    {
        ZoneType.None      => new Color(1, 1, 1, 0.05f),
        ZoneType.MainRoom  => new Color(0.6f, 0.4f, 0.8f, 0.4f),
        ZoneType.RestRoom => new Color(0.4f, 0.7f, 1f, 0.4f),
        ZoneType.PlayRoom  => new Color(1f, 0.8f, 0.4f, 0.4f),
        ZoneType.SleepRoom => new Color(1f, 0.5f, 0.7f, 0.4f),
        ZoneType.DiningRoom => new Color(0.5f, 1f, 0.5f, 0.4f),
        ZoneType.Entrance  => new Color(1f, 1f, 1f, 0.6f),
        ZoneType.WallPaper => new Color(1f, 0f, 0f, 0.4f),
        _                  => Color.clear,
    };
}
