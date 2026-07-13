
using System.Collections;
using System.Collections.Generic;
using Infos;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private ViewController viewController;
    [SerializeField] private PathManager pathManager;

    [SerializeField] private float moveTime = 0.5f;
    public Vector3 moveDir { get; private set; } =  Vector3.zero;
    public bool isMove { get; private set; } = false;
    
    private float moveSpeed = 2f;
    private Vector2Int currentCell;

    void Start()
    {
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
        foreach (var cell in path)
        {
            // 월드 좌표 구하기
            Vector3 target = GridMap.instance.GridToWorld(cell.x, cell.y);
            
            // 부드럽게 이동
            yield return StartCoroutine(GridSmoothMovement(target));
            
            // 도착했으니 칸 갱신
            currentCell = cell;
        }

        isMove = false;
    }
    
    private IEnumerator GridSmoothMovement(Vector3 end)
    {
        Vector3 start = transform.position;
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
