
using System;
using System.Collections;
using System.Collections.Generic;
using Infos;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private ViewController viewController;
    [SerializeField] private PathManager pathManager;

    [SerializeField] private float moveTime = 0.5f;
    
    private bool isMove;
    
    private float moveSpeed = 2f; // 이동 값 데이터 시트로 받기
    private Vector2Int currentCell;
    
    private Animator animator;
    
    private void Awake()
    {
        isMove = false;

        animator = GetComponent<Animator>();
    }

    void Start()
    {
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveY", -1);
        currentCell = GridMap.instance.WorldToGrid(transform.position);
    }

    private void Update()
    {
        
//#if UNITY_ANDROID
        /*
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;

                if (isMove) return;
                
                // View MainRoom인지 확인
                // if (viewController.currentMode != ViewMode.MainRoom)
                
                Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
                Vector2Int targetCell = GridMap.instance.WorldToGrid(world);

                var cell = GridMap.instance.GetCell(targetCell.x, targetCell.y);
                if (cell == null) return;
                if (cell.zone != ZoneType.MainRoom) return;
                if (!cell.IsWalkable) return;

                var path = pathManager.FindPath(currentCell, targetCell);
                if (path != null && path.Count > 0)
                {
                    StopAllCoroutines();
                    StartCoroutine(FollowPath(path));
                }
                 return;
                // 터치 위치가 그리드 좌표인지.
                
                // 목표 칸이 유효한 곳인지

                var path = pathManager.FindPath(new Vector2Int(1,1), new Vector2Int(5,3));

            }
        }
#elif UNITY_EDITOR
*/
        
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (isMove) return;
            //if (viewController.currentMode != ViewMode.MainRoom) return;
            Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int targetCell = GridMap.instance.WorldToGrid(world);

            var cell = GridMap.instance.GetCell(targetCell.x, targetCell.y);
            if (cell == null) return;
            if (cell.zone != ZoneType.MainRoom) return;
            if (!cell.IsWalkable) return;

            var path = pathManager.FindPath(currentCell, targetCell);
            if (path != null && path.Count > 0)
            {
                StopAllCoroutines();
                StartCoroutine(FollowPath(path));
            }
        }
//#endif
    }


    IEnumerator FollowPath(List<Vector2Int> path)
    {
        isMove = true;
        Vector2Int firstMoveCell = path.Count > 1 ? path[1] : path[0];
        Vector3 firstTarget = GridMap.instance.GridToWorld(firstMoveCell.x, firstMoveCell.y);
        Vector3 firstDir = (firstTarget - transform.position);

        animator.SetFloat("MoveX", firstDir.x);
        animator.SetFloat("MoveY", firstDir.y);
        animator.SetBool("isWalk", true);
        for(int i = 1; i< path.Count; i++)
        {
            Vector2Int cell = path[i];
            // 월드 좌표 구하기
            Vector3 target = GridMap.instance.GridToWorld(cell.x, cell.y);
            
            // 부드럽게 이동
            yield return StartCoroutine(GridSmoothMovement(target));
            
            // 도착했으니 칸 갱신
            currentCell = cell;
        }
        isMove = false;
        animator.SetBool("isWalk", false);
    }
    
    private IEnumerator GridSmoothMovement(Vector3 end)
    {
        Vector3 start = transform.position;
        Vector3 dir = (end - start);

        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveY", dir.y);

        float	current = 0;
        float	percent = 0;

        while ( percent < 1 )
        {
            current += Time.deltaTime;
            percent = current / moveTime;

            transform.position = Vector3.Lerp(start, end, percent);
            yield return null;
        }
        transform.position = end;
    }
} 
