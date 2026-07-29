using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YEONJI.Kindergarten
{
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
        public bool hasDoor;
        public Vector2Int door;
        public int x, y;
        public int width = 3, height = 3;
    }

    public class GridManager : BaseManager
    {
        [Header("맵 크기")] 
        public int mapWidth = 11; // 추후에 값은 데이터 시트로 받아온다.
        public int mapHeight = 7;
        public float cellSize = 1f;

        [Header("Sections")] 
        public List<ZoneRect> presetZones = new();
        private GridCell[,] cells;
        private HashSet<(Vector2Int, Vector2Int)> doorPairs = new();
        private HashSet<Vector2Int> doors = new();
        
        [SerializeField] private WallBuilder wallBuilder;
        [SerializeField] private RoomDecorate roomDecorate;
        [SerializeField] private CameraFilter camFilter;

        private static readonly Vector2Int[] dirs =
        {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };
        
        public override async UniTask Task_Init()
        {
            await base.Task_Init();
        }

        public override void Init()
        {
            BuildGrid();
            BuildDoorPairs();
            wallBuilder.Init();
            roomDecorate.Init();
            camFilter.Init();
            base.Init();
        }

        private void BuildGrid()
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

        private void BuildDoorPairs()
        {
            doors.Clear();
            doorPairs.Clear();
            foreach (var z in presetZones)
            {
                if (!z.hasDoor)
                    continue;

                foreach (var dir in dirs)
                {
                    Vector2Int doorNeighbor = z.door + dir;

                    if (GetCell(doorNeighbor.x, doorNeighbor.y) == null)
                        continue;

                    if (GetCell(doorNeighbor.x, doorNeighbor.y).zone == z.type)
                    {
                        doors.Add(z.door);
                        doorPairs.Add((z.door, doorNeighbor));
                        doorPairs.Add((doorNeighbor, z.door));
                    }
                }
            }
        }

        public GridCell GetCell(int x, int y)
        {
            if (!InBounds(x, y)) return null;
            return cells[x, y];
        }

        public bool IsWalkable(int x, int y)
        {
            return InBounds(x, y) && cells[x, y].IsWalkable;
        }

        public bool IsDoor(int x, int y)
        {
            return doors.Contains(new Vector2Int(x, y));
        }

        public bool CanEnter(Vector2Int from, Vector2Int to)
        {
            var fromCell = GetCell(from.x, from.y);
            var toCell = GetCell(to.x, to.y);

            if (fromCell == null || toCell == null) return false;
            if (!toCell.IsWalkable) return false;

            if (cells[from.x, from.y].zone == cells[to.x, to.y].zone) return true;

            return doorPairs.Contains((from, to));
        }

        // 특정 구역(ZoneType)이 차지하는 칸 범위를 구한다
        // 반환: 최소~최대 칸 좌표. 해당 구역이 없으면 found=false
        public bool GetZoneBounds(ZoneType zone, out Vector2Int min, out Vector2Int max)
        {
            min = new Vector2Int(int.MaxValue, int.MaxValue);
            max = new Vector2Int(int.MinValue, int.MinValue);
            bool found = false;

            for (int x = 0; x < mapWidth; x++)
            for (int y = 0; y < mapHeight; y++)
            {
                if (cells[x, y].zone != zone) continue;
                found = true;
                min.x = Mathf.Min(min.x, x);
                min.y = Mathf.Min(min.y, y);
                max.x = Mathf.Max(max.x, x);
                max.y = Mathf.Max(max.y, y);
            }

            return found;
        }

        // 특정 구역의 월드 중심 좌표
        public Vector3 GetZoneCenter(ZoneType zone)
        {
            if (!GetZoneBounds(zone, out var min, out var max))
                return WorldCenter; // 없으면 맵 전체 중심

            float cx = (min.x + max.x + 1) / 2f * cellSize;
            float cy = (min.y + max.y + 1) / 2f * cellSize;
            return transform.position + new Vector3(cx, cy, 0);
        }

        // 특정 구역의 월드 크기 (가로, 세로 유닛)
        public Vector2 GetZoneSize(ZoneType zone)
        {
            if (!GetZoneBounds(zone, out var min, out var max))
                return new Vector2(WorldWidth, WorldHeight);

            float w = (max.x - min.x + 1) * cellSize;
            float h = (max.y - min.y + 1) * cellSize;
            return new Vector2(w, h);
        }

        // 특정 구역의 입구
        public void SetZoneDoor(ZoneType zone, Vector2Int doorPos)
        {
            if (GetCell(doorPos.x, doorPos.y) == null)
                return;

            if (GetCell(doorPos.x, doorPos.y).zone != ZoneType.MainRoom)
                return;

            foreach (var z in presetZones)
            {
                if (z.type == zone)
                {
                    z.hasDoor = true;
                    z.door = doorPos;
                    break;
                }
            }

            BuildDoorPairs();
        }

        // 가구 배치 가능한지
        public bool CanPlace(List<Vector2Int> cells)
        {
            foreach (Vector2Int c in cells)
            {
                var cell = GetCell(c.x, c.y);
                                    
                if (cell == null) return false;
                if (!cell.IsWalkable) return false;
                if (IsDoor(c.x, c.y)) return false;
                
                // 만약에 현관문의 위,아래,우측에 가구가 2방향에 있을때도 놓지 못하게 막는다. (게임 라운드 시작 방해하는 걸 막기)
            }

            return true;
        }

        public void Place(List<Vector2Int> cells, int id)
        {
            foreach (Vector2Int c in cells)
            {
                var cell = GetCell(c.x, c.y);
                if (cell != null)
                    cell.objectId = id;
            }
        }

        public void RemoveFurniture(int id)
        {
            
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

            foreach (var z in presetZones)
            {
                if (z.hasDoor)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawCube(GridToWorld(z.door.x, z.door.y), Vector3.one * cellSize * 0.9f);
                }
            }
        }

        Color ZoneColor(ZoneType zone) => zone switch
        {
            ZoneType.None => new Color(1, 1, 1, 0.05f),
            ZoneType.MainRoom => new Color(0.6f, 0.4f, 0.8f, 0.4f),
            ZoneType.RestRoom => new Color(0.4f, 0.7f, 1f, 0.4f),
            ZoneType.PlayRoom => new Color(1f, 0.8f, 0.4f, 0.4f),
            ZoneType.SleepRoom => new Color(1f, 0.5f, 0.7f, 0.4f),
            ZoneType.DiningRoom => new Color(0.5f, 1f, 0.5f, 0.4f),
            ZoneType.Enterance => new Color(1f, 1f, 1f, 0.6f),
            ZoneType.WallPaper => new Color(1f, 0f, 0f, 0.4f),
            _ => Color.clear,
        };
    }
}