using Infos;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : CharacterBase
{
    private Camera cam;

    protected override void Awake()
    {
        base.Awake();
        cam = Camera.main;
        moveTime = 0.5f;
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
            if (ViewController.instance.currentMode != ViewMode.MainRoom) return;
            Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int targetCell = GridMap.instance.WorldToGrid(world);

            var cell = GridMap.instance.GetCell(targetCell.x, targetCell.y);
            
            if (cell == null) return;
            if (cell.zone != ZoneType.MainRoom) return;
            if (!cell.IsWalkable) return;
            if (GridMap.instance.IsDoor(targetCell.x, targetCell.y)) return;
            
            
            
            var path = PathManager.instance.FindPath(currentCell, targetCell);
            if (path != null && path.Count > 0)
            {
                StopAllCoroutines();
                StartCoroutine(FollowPath(path));
            }
            
            // 플레이어가 유치원생을 누르면 유치원생한테 이동하고 상태창이 나오게 한다.

        }
//#endif
    }
} 
