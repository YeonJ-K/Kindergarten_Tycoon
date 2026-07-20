using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infos;
using Random = UnityEngine.Random;


public class KidsInfo
{
    public KidsID kidId;
    public bool isGirl;
}

public class Kids : CharacterBase
{
    [SerializeField] private GameObject RequestBubble;

    private int moveRate;
    private float waitMinTime;
    private float waitMaxTime;
    
    private KidState state;
    Coroutine stateRoutine;
    
    protected override void Awake()
    {
        base.Awake();
        // 임시 값 //
        moveRate = 45;
        waitMinTime = 2f;
        waitMaxTime = 4f;
        moveTime = 0.65f;
        // ---- //
    }

    protected override void Start()
    {
        base.Start();
        //KidsManager.instance.Register(this);
        RequestBubble.SetActive(false);
        ChangeState(KidState.Entering);
    }

    private void ChangeState(KidState next)
    {
        state = next;
        if (stateRoutine != null)
            StopCoroutine(stateRoutine);
        
        switch (state)
        {
            case KidState.Entering: // 등원
            {
                //stateRoutine = StartCoroutine(EnteringRoutine());
                break;
            }

            case KidState.Wandering: // 메인 룸 돌아다니기
            {
                //stateRoutine = StartCoroutine(KidsMoveAround());
                break;
            }
            case KidState.MovingToZone: // 구역으로 이동
            {
                
                break;
            }
            
            case KidState.Doing: // 구역에서 특별 행동
            {
                break;
            }

            case KidState.Requesting: // 요구사항 발생
            {
                animator.SetBool("isTired", true);
                break;
            }

            case KidState.StressUp: // 요구사항 발생 이후 제한시간 지남
            {
                animator.SetBool("isTired", true);
                break;
            }

            case KidState.Exiting: // 하원
            {
                stateRoutine = StartCoroutine(ExitingRoutine());
                break;
            }
        }
    }

    private bool TryPickWanderTarget(ZoneType zone, HashSet<Vector2Int> occupied, out Vector2Int target)
    {
        GridMap.instance.GetZoneBounds(zone, out var min, out var max);
        
        for (int i = 0; i < 10; i++)
        {
            int x = Random.Range(min.x, max.x+1);
            int y = Random.Range(min.y, max.y+1);
            
            if (GridMap.instance.GetCell(x, y).zone != zone) continue;
            if (GridMap.instance.IsDoor(x, y)) continue;
            if (!GridMap.instance.IsWalkable(x, y)) continue;
            if (currentCell == new Vector2Int(x,y)) continue;
            if (occupied.Contains(new Vector2Int(x, y))) continue;
            
            target = new Vector2Int(x, y);
            return true;
        }

        target = default;
        return false;
    }

    /*
    IEnumerator KidsMoveAround()
    {
        while (true)
        {
            // 이동
            int rate = Random.Range(0, 100);
            
            if (rate < moveRate)
            {
                var occupied = KidsManager.instance.GetOccupied(this);
                if (TryPickWanderTarget(ZoneType.MainRoom, occupied, out var target))
                {
                    var path = PathManager.instance.FindPath(currentCell, target, occupied);
                    if (path != null && path.Count > 1)
                        yield return StartCoroutine(FollowPath(path));    
                }
            }
                
            // 잠시 대기
            float waitRate = Random.Range(waitMinTime, waitMaxTime);
            yield return new WaitForSeconds(waitRate);
        }
    }*/

    /*
    IEnumerator EnteringRoutine()
    {
        var occupied = KidsManager.instance.GetOccupied(this);
        if (TryPickWanderTarget(ZoneType.MainRoom, occupied, out var target))
        {
            var path = PathManager.instance.FindPath(currentCell, target, occupied);
            if (path != null && path.Count > 1)
                yield return StartCoroutine(FollowPath(path));
        }
        ChangeState(KidState.Wandering);
    }*/

    IEnumerator ExitingRoutine()
    {
        yield return new WaitForSeconds(waitMaxTime);
        //KidsManager.instance.UnRegister(this);
    }

}
