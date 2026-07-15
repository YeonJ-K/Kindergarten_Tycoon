using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterBase : MonoBehaviour
{
    [SerializeField] protected PathManager pathManager;
    protected float moveTime;
    
    protected Animator animator;
    protected bool isMove;
    public bool IsMove => isMove;
    protected Vector2Int currentCell;
    public Vector2Int CurrentCell => currentCell;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        isMove = false;
    }

    protected virtual void Start()
    {
        currentCell = GridMap.instance.WorldToGrid(transform.position);
        
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveY", -1);
    }

    protected IEnumerator FollowPath(List<Vector2Int> path)
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
    
    protected IEnumerator GridSmoothMovement(Vector3 end)
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
}
