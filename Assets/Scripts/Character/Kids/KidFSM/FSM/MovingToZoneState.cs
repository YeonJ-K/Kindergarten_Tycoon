
using System.Collections.Generic;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class MovingToZoneState : IKidState
    {
        public KidState Id => KidState.MovingToZone;

        public void Enter(KidsContext context)
        {
            if (context.playerProcessNeed == NeedType.None)
            {
                // 놀이 행동 정하기
                return;
            }
            
            if (context.playerProcessNeed == NeedType.None)
            {
                Debug.Log("None이라 빠져나감");
                return;
            }

            ActiveFurniture furniture = InGameCore.FUR.FindNeedsFurniture(context.playerProcessNeed);
            Debug.Log($"찾은 가구: {(furniture == null ? "없음" : furniture.name)}");

            if (furniture == null)
            {
                context.machine.ChangeState(new WanderingState());
                return;
            }
            context.usingFurniture = furniture;
            var occupied = InGameCore.KIDS.GetOccupied(context.agent);
            if (furniture.TryOccupy(context.agent, out var target))
            {
                Debug.Log($"슬롯 잡음, 목적지: {target}");

                context.agent.RequestPath(target, occupied);
                context.desCell = target;
            }
            else
            {
                // 대기 로직 만들기 
                Debug.Log("슬롯 못 잡음");

                context.machine.ChangeState(new WanderingState());
                return;
            }
        }

        public void Tick(KidsContext context, float dt)
        {
            if (context.agent.IsMove) return;
            if (context.agent.CurrentCell == context.desCell)
            {
                context.machine.ChangeState(new DoingState());
                return;
            }
            
            context.timer -= dt;
            if (context.timer <= 0)
            {
                var occupied = InGameCore.KIDS.GetOccupied(context.agent);
                context.agent.RequestPath(context.desCell, occupied);
                context.timer = 0.5f;
            }

        }

        public void Exit(KidsContext context) { }

    }
}