using System;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager instance { get; private set; }

    // 이동 비용은 1로 지정한다.
    class Node
    {
        public Vector2Int pos;
        public int g; // 출발점에서 현재 위치상의 거리
        public int h; // 목표까지의 예상 거리
        public int f => g + h; // 총 비용

        public Node parent; // 계산한 경로에 대해서 움직일 수 있게 저장
    }

    // 갈 수 있는 방향 지정
    private static readonly Vector2Int[] dirs =
    {
        Vector2Int.left, Vector2Int.right,
        Vector2Int.up, Vector2Int.down,
    };

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal, HashSet<Vector2Int> occupied = null)
    {
        var open = new List<Node>();
        var close = new HashSet<Vector2Int>();
        
        // 이미 open에 있는 칸을 빠르게 찾기 위해서 Dictionary 사용
        var openLookup = new Dictionary<Vector2Int, Node>();
        
        // 시작 노드를 Open에 넣기
        var startNode = new Node { pos = start, g = 0, h = Heuristic(start, goal) };
        open.Add(startNode);
        openLookup[start] = startNode;

        while (open.Count > 0)
        {
            // open에서 f가 가장 작은 노드 찾기

            Node current = open[0];
            foreach (var node in open)
            {
                if (node.f < current.f)
                {
                    current = node;
                }
            }

            // 목표에 도달했는지 확인
            if (current.pos == goal)
                return Reconstruct(current);

            // 현재 시점에서 open을 closed 로 넣기
            open.Remove(current);
            openLookup.Remove(current.pos);
            close.Add(current.pos);

            // 이웃된 4 방향을 검사하기
            foreach (var dir in dirs)
            {
                Vector2Int next = current.pos + dir;
                
                // 갈수 없으면 걸러낸다.
                // 1. 지정된 구역 밖인지 확인
                // 2. 걸을 수 없는 칸 (벽/오브젝트)
                // 3. 이미 closed 된 칸
                // 4. 다른 유치원생이 있는 칸
                // 5. 입구칸
                if (!GridMap.instance.CanEnter(current.pos, next)) continue; 
                if (occupied != null && occupied.Contains(next)) continue;
                if (close.Contains(next)) continue;
                
                int newG = current.g + 1; // 이동 비용 1
                
                // next가 open에 없으면 새로 추가한다.
                if (!openLookup.TryGetValue(next, out var neighbor))
                {
                    var newNode = new Node { pos = next, g = newG, h = Heuristic(next, goal), parent = current};
                    
                    open.Add(newNode);
                    openLookup[next] = newNode;
                }
                // 이미 open에 있는데 newG가 더 작으면 갱신한다.
                else
                {
                    if (neighbor.g > newG)
                    {
                        neighbor.g = newG;
                        neighbor.parent = current;
                    }
                }
            }
        }

        return null; // 경로 없음
    }

    // 맨헤튼 거리 계산용
    // 맨해튼 거리 |목표.x - 현재.x| + |목표.y -  현재.y|
    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
    
    // 목표 노드에서 부모까지 거꾸로 따라가서 경로를 만들어 움직이게 한다.
    List<Vector2Int> Reconstruct(Node goalNode)
    {
        var path = new List<Vector2Int>();
        Node n = goalNode;

        while (n != null)
        {
            path.Add(n.pos);
            n = n.parent;
        }
        path.Reverse(); // 출발지에서 목표 순서로 돌리기 위해서 순서 바꾸기

        return path;
    }
    
}

