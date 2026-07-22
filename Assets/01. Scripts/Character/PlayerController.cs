using System.Collections;
using System.Collections.Generic;
using Infos;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private float moveTime;
    Animator animator;
    private bool isMove;
    public bool IsMove => isMove;
    private Vector2Int currentCell;
    public Vector2Int CurrentCell => currentCell;
    private GameRoundUI roundUI;
    
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
        roundUI = GameObject.FindGameObjectWithTag("UI").GetComponent<GameRoundUI>();
        moveTime = 0.5f;
    }

    private void Start()
    {
        currentCell = GridMap.instance.WorldToGrid(transform.position);
        
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveY", -1);
    }

    private void Update()
    {
        
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && RoundManager.instance.roundStart)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (isMove) return;
            if (ViewController.instance.currentMode != ViewMode.MainRoom) return;
            Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int targetCell = GridMap.instance.WorldToGrid(world);
            
            KidAgent kid = KidsManager.instance.GetKidAgent(targetCell);
            if (kid != null)
            {
                kid.GotCaught();
                if (FindAdjacentCell(kid.CurrentCell, out Vector2Int adj))
                {
                    var kidsOccupied = KidsManager.instance.GetOccupied();
                    var desPath = PathManager.instance.FindPath(currentCell, adj, kidsOccupied);
                    if (desPath != null && desPath.Count > 0)
                    {
                        StopAllCoroutines();
                        StartCoroutine(CatchRoutine(desPath, kid));
                    }
                }

                roundUI.OpenStatusBox(kid);
                return;
            }

            var cell = GridMap.instance.GetCell(targetCell.x, targetCell.y);
            
            if (cell == null) return;
            if (cell.zone != ZoneType.MainRoom) return;
            if (!cell.IsWalkable) return;
            if (GridMap.instance.IsDoor(targetCell.x, targetCell.y)) return;
            
            roundUI.CloseStatusBox();
            var occupied = KidsManager.instance.GetOccupied();
            var path = PathManager.instance.FindPath(currentCell, targetCell, occupied);
            if (path != null && path.Count > 0)
            {
                StopAllCoroutines();
                StartCoroutine(FollowPath(path));
            }
        }
        
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;
                if (isMove) return;
                
                if (ViewController.instance.currentMode != ViewMode.MainRoom) return;
                Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
                Vector2Int targetCell = GridMap.instance.WorldToGrid(world);
                
                KidAgent kid = KidsManager.instance.GetKidAgent(targetCell);
                if (kid != null)
                {
                    kid.GotCaught();
                    if (FindAdjacentCell(kid.CurrentCell, out Vector2Int adj))
                    {
                        var kidsOccupied = KidsManager.instance.GetOccupied();
                        var desPath = PathManager.instance.FindPath(currentCell, adj, kidsOccupied);
                        if (desPath != null && desPath.Count > 0)
                        {
                            StopAllCoroutines();
                            StartCoroutine(CatchRoutine(desPath, kid));
                        }
                    }

                    roundUI.OpenStatusBox(kid);
                    return;
                }

                var cell = GridMap.instance.GetCell(targetCell.x, targetCell.y);
                
                if (cell == null) return;
                if (cell.zone != ZoneType.MainRoom) return;
                if (!cell.IsWalkable) return;
                if (GridMap.instance.IsDoor(targetCell.x, targetCell.y)) return;
                
                roundUI.CloseStatusBox();
                var occupied = KidsManager.instance.GetOccupied();
                var path = PathManager.instance.FindPath(currentCell, targetCell, occupied);
                if (path != null && path.Count > 0)
                {
                    StopAllCoroutines();
                    StartCoroutine(FollowPath(path));
                }
            }
        }
#endif
    }

    private IEnumerator FollowPath(List<Vector2Int> path)
    {
        isMove = true;
 
        // 첫 방향 미리 세팅 (path[0]은 현재 위치이므로 path[1] 기준)
        if (path.Count > 1)
        {
            Vector3 firstTarget = GridMap.instance.GridToWorld(path[1].x, path[1].y);
            Vector3 firstDir = firstTarget - transform.position;
            animator.SetFloat("MoveX", firstDir.x);
            animator.SetFloat("MoveY", firstDir.y);
        }
 
        animator.SetBool("isWalk", true);
 
        // path[0]은 현재 위치이므로 1부터
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 target = GridMap.instance.GridToWorld(path[i].x, path[i].y);
            yield return StartCoroutine(GridSmoothMovement(target));
            currentCell = path[i];
        }
 
        animator.SetBool("isWalk", false);
        isMove = false;
    }
    
    private IEnumerator GridSmoothMovement(Vector3 end)
    {
        Vector3 start = transform.position;
        Vector3 dir = end - start;
 
        // 이동 방향 애니메이션
        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveY", dir.y);
 
        float current = 0;
        float percent = 0;
        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / moveTime;
            transform.position = Vector3.Lerp(start, end, percent);
            yield return null;
        }
        transform.position = end;
    }

    private bool FindAdjacentCell(Vector2Int kidCell, out Vector2Int result)
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        result = default;
        bool found = false;
        int bestDist = int.MaxValue;
        
        foreach (var dir in dirs)
        {
            Vector2Int adj = kidCell + dir;
            var cell = GridMap.instance.GetCell(adj.x, adj.y);
            if (cell == null) continue;
            if (cell.zone != ZoneType.MainRoom) continue;
            if (!cell.IsWalkable) continue;
            if (KidsManager.instance.GetKidAgent(adj) != null) continue; 
            
            int dist = Mathf.Abs(adj.x - currentCell.x) + Mathf.Abs(adj.y - currentCell.y);
            if (dist < bestDist)
            {
                bestDist = dist;
                result = adj;
                found = true;
            }
        }
        return found;
    }
    
    void FaceToward(Vector2Int targetCell)
    {
        Vector2Int dir = targetCell - currentCell;
        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveY", dir.y);
        animator.SetBool("isWalk", false);   // 멈춰서 바라만 봄
    }

    IEnumerator CatchRoutine(List<Vector2Int> path, KidAgent kid)
    {
        yield return StartCoroutine(FollowPath(path));
        FaceToward(kid.CurrentCell);
    }
} 
