using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infos;
using Random = UnityEngine.Random;

public class KidsMoveController : MonoBehaviour
{
    [SerializeField] private GameObject StatusUI;
    [SerializeField] private RectTransform uiRT;
    [SerializeField] private PathManager pathManager;

    [SerializeField] private float moveTime = 0.3f;
    private bool isMove;
    private bool isAngry;
    private bool isTired;
    private float moveChance;
    Animator animator;

    private void Start()
    {
        isMove = false;
        moveChance = 0.5f;
    }

    IEnumerator BehaviorLoop()
    {
        while (true)
        {
            // 일정 시간 대기 (예: 2~4초 랜덤)
            //yield return new WaitForSeconds(/* 랜덤 간격 */);

            // 이미 움직이는 중이면 스킵
            if (isMove) continue;

            // 확률 체크: 움직일까?
            if (Random.value < moveChance)
            {
                // 목표 칸 정하기
                //Vector2Int target = /* 어떻게 정할지? */;

                // 경로 찾아서 이동
                //var path = pathManager.FindPath(currentCell, target, GetOccupied());
                //if (path != null && path.Count > 1)
                //    yield return StartCoroutine(FollowPath(path));
            }
        }
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
            //currentCell = cell;
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
