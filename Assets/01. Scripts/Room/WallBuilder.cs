using System.Collections.Generic;
using UnityEngine;
using Infos;

// ────────────────────────────────────────────────────────────
// 스프라이트 직접 생성 방식 벽 빌더
//
// 원리:
//  - 각 "벽 칸"(방 가장자리 칸)이 상하좌우 경계 상태를 보고 조각을 고른다.
//  - 각 방향의 상태는 3가지: OUTSIDE(바깥) / OTHER(다른 방) / SAME(같은 방=내부)
//  - 방-방 경계(OTHER)에는 공유벽/ T자를 써서 한 겹으로 합친다.
//  - Tilemap 대신 SpriteRenderer 프리팹을 Instantiate로 배치.
// ────────────────────────────────────────────────────────────

public enum EdgeState { Same, Other, Outside }
// Same   = 같은 방 (내부, 벽 없음)
// Other  = 다른 방 (경계, 공유벽/T자)
// Outside= 바깥 (테두리, top/bottom/left/right)

public class WallCellInfo
{
    public Vector2Int cell;
    public EdgeState up, down, left, right;
    public WallPiece piece = WallPiece.None;
}

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
    public Transform wallParent;         // 벽 스프라이트들이 담길 부모 (없으면 자동 생성)

    [Header("벽 조각 스프라이트")]
    public WallPieceSprite[] pieces;
    public int sortingOrder = 10;
    public string sortingLayer = "Default";

    [Header("옵션")]
    public bool buildOnStart = true;
    public bool showDebugLabels = true;  // Scene 뷰에 조각 이름 표시

    Dictionary<Vector2Int, WallCellInfo> walls = new();

    void Start()
    {
        if (buildOnStart) BuildWalls();
    }

    public void BuildWalls()
    {
        if (map == null || map.cells == null)
        {
            Debug.LogWarning("[WallBuilder] GridMap 준비 안 됨. BuildGrid() 이후 호출하세요.");
            return;
        }

        if (wallParent == null)
        {
            var go = new GameObject("Walls");
            go.transform.SetParent(transform);
            wallParent = go.transform;
        }

        walls = Analyze();
        Draw();
    }

    // ────────────────────────────────────────────────
    // 1단계: 각 벽 칸의 상하좌우 경계 상태 분석
    // ────────────────────────────────────────────────
    Dictionary<Vector2Int, WallCellInfo> Analyze()
    {
        var result = new Dictionary<Vector2Int, WallCellInfo>();

        for (int x = 0; x < map.mapWidth; x++)
        for (int y = 0; y < map.mapHeight; y++)
        {
            ZoneType myZone = map.cells[x, y].zone;
            if (myZone == ZoneType.None) continue;  // 방 칸만

            var info = new WallCellInfo
            {
                cell = new Vector2Int(x, y),
                up    = GetEdge(myZone, x, y + 1),
                down  = GetEdge(myZone, x, y - 1),
                left  = GetEdge(myZone, x - 1, y),
                right = GetEdge(myZone, x + 1, y),
            };

            // 사방이 다 같은 방이면 내부 칸 → 벽 아님
            if (info.up == EdgeState.Same && info.down == EdgeState.Same &&
                info.left == EdgeState.Same && info.right == EdgeState.Same)
                continue;

            info.piece = SelectPiece(info);
            result[info.cell] = info;
        }

        return result;
    }

    // 내 방 기준으로 이웃 칸의 상태 판정
    EdgeState GetEdge(ZoneType myZone, int nx, int ny)
    {
        if (!map.InBounds(nx, ny)) return EdgeState.Outside;    // 맵 밖 = 바깥
        ZoneType nz = map.cells[nx, ny].zone;
        if (nz == ZoneType.None) return EdgeState.Outside;      // 빈 칸 = 바깥
        if (nz == myZone) return EdgeState.Same;                // 같은 방
        return EdgeState.Other;                                  // 다른 방
    }

    // ────────────────────────────────────────────────
    // 2단계: 조각 선택
    //  "벽이 필요한 방향" = 바깥(Outside)이거나 다른 방(Other)인 방향
    //  그 방향들의 조합으로 조각을 고른다.
    // ────────────────────────────────────────────────
    WallPiece SelectPiece(WallCellInfo w)
    {
        // 벽이 필요한 방향(테두리 or 경계)
        bool wU = w.up    != EdgeState.Same;
        bool wD = w.down  != EdgeState.Same;
        bool wL = w.left  != EdgeState.Same;
        bool wR = w.right != EdgeState.Same;

        int count = (wU?1:0)+(wD?1:0)+(wL?1:0)+(wR?1:0);

        // ── 4방향: 십자 ──
        if (count == 4) return WallPiece.MiddleCross;

        // ── 3방향: T자 ──
        if (count == 3)
        {
            // 막힌(Same) 방향 기준으로 T자 종류 결정
            if (!wD) return WallPiece.MiddleComboUp;    // 아래가 내부 → ┴
            if (!wU) return WallPiece.MiddleComboDown;  // 위가 내부 → ┬
            if (!wR) return WallPiece.MiddleComboLeft;  // 오른쪽 내부 → ┤
            if (!wL) return WallPiece.MiddleComboRight; // 왼쪽 내부 → ├
        }

        // ── 2방향 ──
        if (count == 2)
        {
            // 마주보는 방향 = 직선
            if (wL && wR)  // 가로 직선
            {
                // 위가 바깥이면 top, 아래가 바깥이면 bottom, 둘 다 방이면 middle2(공유)
                if (w.up == EdgeState.Outside)   return WallPiece.Top;
                if (w.down == EdgeState.Outside) return WallPiece.Bottom;
                return WallPiece.Middle2;
            }
            if (wU && wD)  // 세로 직선
            {
                if (w.left == EdgeState.Outside)  return WallPiece.Left;
                if (w.right == EdgeState.Outside) return WallPiece.Right;
                return WallPiece.Middle;
            }

            // 직각 = 코너 (뻗는 두 방향 = 조각 이름, 픽셀 분석 확정)
            if (wU && wL) return WallPiece.TopLeft;
            if (wU && wR) return WallPiece.TopRight;
            if (wD && wL) return WallPiece.BottomLeft;
            if (wD && wR) return WallPiece.BottomRight;
        }

        // ── 1방향: 끝(외톨이 테두리) ──
        if (count == 1)
        {
            // 그 방향이 바깥이면 그쪽 테두리 조각
            if (wU) return w.up == EdgeState.Outside ? WallPiece.Bottom : WallPiece.Middle2;
            if (wD) return w.down == EdgeState.Outside ? WallPiece.Top : WallPiece.Middle2;
            if (wL) return w.left == EdgeState.Outside ? WallPiece.Right : WallPiece.Middle;
            if (wR) return w.right == EdgeState.Outside ? WallPiece.Left : WallPiece.Middle;
        }

        return WallPiece.None;
    }

    // ────────────────────────────────────────────────
    // 3단계: 스프라이트 배치
    // ────────────────────────────────────────────────
    void Draw()
    {
        ClearWalls();

        foreach (var kv in walls)
        {
            var info = kv.Value;
            Sprite sp = GetSprite(info.piece);
            if (sp == null)
            {
                Debug.LogWarning($"[WallBuilder] {info.cell}: 조각 {info.piece} 스프라이트 미연결");
                continue;
            }

            var go = new GameObject($"Wall_{info.cell.x}_{info.cell.y}_{info.piece}");
            go.transform.SetParent(wallParent);
            go.transform.position = map.GridToWorld(info.cell.x, info.cell.y);

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
            if (Application.isPlaying)
                Destroy(wallParent.GetChild(i).gameObject);
            else
                DestroyImmediate(wallParent.GetChild(i).gameObject);
        }
    }

    Sprite GetSprite(WallPiece piece)
    {
        foreach (var p in pieces)
            if (p.piece == piece) return p.sprite;
        return null;
    }

    // ────────────────────────────────────────────────
    // 디버그: Scene 뷰에 각 벽 칸의 선택 조각 이름 표시
    // ────────────────────────────────────────────────
    void OnDrawGizmos()
    {
        if (!showDebugLabels || walls == null || walls.Count == 0 || map == null) return;

#if UNITY_EDITOR
        var style = new GUIStyle();
        style.normal.textColor = Color.yellow;
        style.fontSize = 9;
        foreach (var kv in walls)
        {
            Vector3 pos = map.GridToWorld(kv.Key.x, kv.Key.y);
            UnityEditor.Handles.Label(pos, kv.Value.piece.ToString(), style);
        }
#endif
    }
}