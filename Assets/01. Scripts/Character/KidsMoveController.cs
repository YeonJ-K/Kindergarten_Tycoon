using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infos;
using Random = UnityEngine.Random;

public class KidsMoveController : CharacterBase
{
    [SerializeField] private GameObject RequestBubble;

    private Transform currentPos;
    
    private int moveRate;
    private float waitMinTime;
    private float waitMaxTime;
    
    private KidState state;
    Coroutine stateRoutine;
    
    protected override void Awake()
    {
        base.Awake();
        currentPos = GetComponent<Transform>();
        // 임시 값 //
        moveRate = 70;
        waitMinTime = 2f;
        waitMaxTime = 4f;
        moveTime = 0.65f;
        // ---- //
    }

    protected override void Start()
    {
        ChangeState(KidState.Wandering);
    }

    private void ChangeState(KidState next)
    {
        state = next;
        StopCoroutine(stateRoutine);
        
        switch (state)
        {
            case KidState.Entering: // 등원
            {
                //stateRoutine = StartCoroutine();
                break;
            }

            case KidState.Wandering: // 메인 룸 돌아다니기
            {
                stateRoutine = StartCoroutine(KidsMoveAround());
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
                
                break;
            }
        }
    }

    private bool TryPickWanderTarget(out Vector2Int target)
    {
        GridMap.instance.GetZoneBounds(ZoneType.MainRoom, out var min, out var max);
        int x = Random.Range(min.x, max.x);
        int y = Random.Range(min.y, max.y);
        
        if (!GridMap.instance.InBounds(out target.x, y)) return false;
        if (GridMap.instance.GetCell(x, y).zone != ZoneType.MainRoom) return false;
        if (GridMap.instance.IsDoor(x, y)) return false;
        if (!GridMap.instance.IsWalkable(x, y)) return false;
        if (currentPos != new Vector2Int(x,y)) return false;
    }

    IEnumerator KidsMoveAround()
    {
        while (true)
        {
            // 이동
            int rate = Random.Range(0, 100);

            
            if (rate < moveRate && TryPickWanderTarget(out var target))
            {


                // 메인 룸 안에서 랜덤으로 목적지 고르기 (걸을 수 있어야함, door 아니어야 함, 현재 칸이 아니어야 함)
                //var path = pathManager.FindPath()

            }
                
            // 잠시 대기
            float waitRate = Random.Range(waitMinTime, waitMaxTime);
            yield return new WaitForSeconds(waitRate);
        }

        yield break;
    }
    
    
}
