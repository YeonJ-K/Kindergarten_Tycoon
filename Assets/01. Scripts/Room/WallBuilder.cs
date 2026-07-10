using System.Collections.Generic;
using UnityEngine;
using Infos;

// ────────────────────────────────────────────────────────────
// 스캔 방식 벽 빌더 (가로/세로/하단 완성)
//
// 순서:
//  1. 바깥 테두리 (top/bottom/left/right + 코너)
//  2. 세로 방 경계벽 (중앙 기준 좌우 분리)
//     - 왼쪽: (x,y)≠(x+1,y) → 왼쪽칸. 위끝 LeftTop/MiddleRightMiddle, 몸통 MiddleRight
//     - 오른쪽: (x,y)≠(x-1,y) → 오른쪽칸. 위끝 RightTop/MiddleLeftMiddle, 몸통 MiddleLeft
//  3. 가로 방 경계벽 (LeftTop-Middle2-RightTop 스캔)
//     - 세로벽이 이미 있는 칸(교차점)은 덮지 않음
//  4. 하단 경계 + 중복 제거
// ────────────────────────────────────────────────────────────

[System.Serializable]
public class WallPieceSprite
{
    public WallPiece piece;
    public Sprite sprite;
}

public class WallBuilder : MonoBehaviour
{
    [Header("참조")]
    public GridMap map;
    public Transform wallParent;

    [Header("벽 조각 스프라이트")]
    public WallPieceSprite[] pieces;
    public int sortingOrder = 10;
    public string sortingLayer = "Default";

    [Header("옵션")]
    public bool buildOnStart = true;
    public bool showDebugLabels = true;

    Dictionary<Vector2Int, WallPiece> placed = new();

    void Start()
    {
        if (buildOnStart) BuildWalls();
    }

    public void BuildWalls()
    {
        if (map == null || map.cells == null)
        {
            Debug.LogWarning("[WallBuilder] GridMap 준비 안 됨.");
            return;
        }
        if (wallParent == null)
        {
            var go = new GameObject("Walls");
            go.transform.SetParent(transform);
            wallParent = go.transform;
        }

        placed.Clear();
        BuildBorder();
        BuildVertical();
        BuildHorizontal();
        BuildBottom();
        Draw();
    }

    bool IsRoom(int x, int y)
    {
        if (!map.InBounds(x, y)) return false;
        return map.cells[x, y].zone != ZoneType.None;
    }
    ZoneType Zone(int x, int y)
    {
        if (!map.InBounds(x, y)) return ZoneType.None;
        return map.cells[x, y].zone;
    }
    bool SameRoom(int ax, int ay, int bx, int by)
    {
        var za = Zone(ax, ay); var zb = Zone(bx, by);
        if (za == ZoneType.None || zb == ZoneType.None) return false;
        return za == zb;
    }
    bool Has(Vector2Int c) => placed.ContainsKey(c);
    void Put(Vector2Int c, WallPiece p) => placed[c] = p;

    // 세로벽 조각인지 (가로벽이 덮으면 안 되는 교차점 판별)
    bool IsVerticalPiece(WallPiece p)
    {
        return p == WallPiece.MiddleRight || p == WallPiece.MiddleLeft
            || p == WallPiece.MiddleRightMiddle || p == WallPiece.MiddleLeftMiddle
            || p == WallPiece.LeftTop || p == WallPiece.RightTop;
    }

    // ── 1. 바깥 테두리 ──
    void BuildBorder()
    {
        for (int x = 0; x < map.mapWidth; x++)
        for (int y = 0; y < map.mapHeight; y++)
        {
            if (!IsRoom(x, y)) continue;
            bool oU = !IsRoom(x, y + 1);
            bool oD = !IsRoom(x, y - 1);
            bool oL = !IsRoom(x - 1, y);
            bool oR = !IsRoom(x + 1, y);
            if (!oU && !oD && !oL && !oR) continue;

            var c = new Vector2Int(x, y);
            WallPiece p = WallPiece.None;
            if (oU && oL) p = WallPiece.TopCornerLeft;
            else if (oU && oR) p = WallPiece.TopCornerRight;
            else if (oD && oL) p = WallPiece.BottomCornerLeft;
            else if (oD && oR) p = WallPiece.BottomCornerRight;
            else if (oU) p = WallPiece.Top;
            else if (oD) p = WallPiece.Bottom;
            else if (oL) p = WallPiece.Left;
            else if (oR) p = WallPiece.Right;
            if (p != WallPiece.None) placed[c] = p;
        }
    }

    // ── 2. 세로 방 경계벽 ──
    void BuildVertical()
    {
        int midX = map.mapWidth / 2;

        // 왼쪽 절반
        for (int x = 0; x < midX; x++)
        {
            bool active = false;
            for (int y = map.mapHeight - 1; y >= 0; y--)
            {
                bool boundary = IsRoom(x, y) && IsRoom(x + 1, y) && !SameRoom(x, y, x + 1, y);
                if (boundary)
                {
                    var c = new Vector2Int(x, y);
                    if (!active) { Put(c, WallPiece.MiddleRightMiddle); active = true; }
                    else Put(c, WallPiece.MiddleRight);
                }
                else active = false;
            }
        }

        // 오른쪽 절반
        for (int x = midX; x < map.mapWidth; x++)
        {
            bool active = false;
            for (int y = map.mapHeight - 1; y >= 0; y--)
            {
                bool boundary = IsRoom(x, y) && IsRoom(x - 1, y) && !SameRoom(x, y, x - 1, y);
                if (boundary)
                {
                    var c = new Vector2Int(x, y);
                    if (!active) { Put(c, WallPiece.MiddleLeftMiddle); active = true; }
                    else Put(c, WallPiece.MiddleLeft);
                }
                else active = false;
            }
        }
    }

    // ── 3. 가로 방 경계벽 (LeftTop - Middle2 - RightTop) ──
    void BuildHorizontal()
    {
        // 각 가로 경계선: (x,y)와 (x,y+1) 방이 다른 줄
        for (int y = 0; y < map.mapHeight - 1; y++)
        {
            // 이 y줄에서 가로 경계가 있는 구간을 왼→오로 스캔
            for (int x = 0; x < map.mapWidth; x++)
            {
                bool boundary = IsRoom(x, y) && IsRoom(x, y + 1) && !SameRoom(x, y, x, y + 1);
                if (!boundary) continue;

                var c = new Vector2Int(x, y);

                // 교차점: 세로벽 조각이 이미 있으면 덮지 않음
                if (Has(c) && IsVerticalPiece(placed[c])) continue;

                // 왼쪽 끝인가 (왼쪽이 바깥이거나 경계 아님) → LeftTop
                bool leftEnd = !(IsRoom(x - 1, y) && IsRoom(x - 1, y + 1) && !SameRoom(x - 1, y, x - 1, y + 1));
                // 오른쪽 끝인가 → RightTop
                bool rightEnd = !(IsRoom(x + 1, y) && IsRoom(x + 1, y + 1) && !SameRoom(x + 1, y, x + 1, y + 1));

                // 왼쪽 테두리에 닿은 시작점
                bool touchLeftBorder = !IsRoom(x - 1, y) || !IsRoom(x - 1, y + 1);
                bool touchRightBorder = !IsRoom(x + 1, y) || !IsRoom(x + 1, y + 1);

                if (leftEnd && touchLeftBorder) Put(c, WallPiece.LeftTop);
                else if (rightEnd && touchRightBorder) Put(c, WallPiece.RightTop);
                else Put(c, WallPiece.Middle2);
            }
        }
    }

    // ── 4. 하단 경계 (중앙 기준 좌우 분리로 한 겹 보장) ──
    void BuildBottom()
    {
        int y = 0;
        int midX = map.mapWidth / 2;

        // 왼쪽 절반: (x,0)≠(x+1,0) → 왼쪽칸 BottomRightMiddle
        for (int x = 0; x < midX; x++)
        {
            if (IsRoom(x, y) && IsRoom(x + 1, y) && !SameRoom(x, y, x + 1, y))
                Put(new Vector2Int(x, y), WallPiece.BottomRightMiddle);
        }

        // 오른쪽 절반: (x,0)≠(x-1,0) → 오른쪽칸 BottomLeftMiddle
        for (int x = midX; x < map.mapWidth; x++)
        {
            if (IsRoom(x, y) && IsRoom(x - 1, y) && !SameRoom(x, y, x - 1, y))
                Put(new Vector2Int(x, y), WallPiece.BottomLeftMiddle);
        }
    }

    // ── 배치 ──
    void Draw()
    {
        ClearWalls();
        foreach (var kv in placed)
        {
            Sprite sp = GetSprite(kv.Value);
            if (sp == null)
            {
                Debug.LogWarning($"[WallBuilder] {kv.Key}: 조각 {kv.Value} 미연결");
                continue;
            }
            var go = new GameObject($"Wall_{kv.Key.x}_{kv.Key.y}_{kv.Value}");
            go.transform.SetParent(wallParent);
            go.transform.position = map.GridToWorld(kv.Key.x, kv.Key.y);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sp;
            sr.sortingOrder = sortingOrder;
            if (!string.IsNullOrEmpty(sortingLayer))
                sr.sortingLayerName = sortingLayer;
        }
    }

    void ClearWalls()
    {
        if (wallParent == null) return;
        for (int i = wallParent.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying) Destroy(wallParent.GetChild(i).gameObject);
            else DestroyImmediate(wallParent.GetChild(i).gameObject);
        }
    }

    Sprite GetSprite(WallPiece piece)
    {
        foreach (var p in pieces)
            if (p.piece == piece) return p.sprite;
        return null;
    }

    void OnDrawGizmos()
    {
        if (!showDebugLabels || placed == null || placed.Count == 0 || map == null) return;
#if UNITY_EDITOR
        var style = new GUIStyle();
        style.normal.textColor = Color.yellow;
        style.fontSize = 8;
        foreach (var kv in placed)
        {
            Vector3 pos = map.GridToWorld(kv.Key.x, kv.Key.y);
            UnityEditor.Handles.Label(pos, kv.Value.ToString(), style);
        }
#endif
    }
}