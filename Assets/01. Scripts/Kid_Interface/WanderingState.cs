using Infos;
using UnityEngine;

public class WanderingState : IKidState
{
    private readonly float moveRate = 70f;
    private readonly float waitMin = 1f;
    private readonly float waitMax = 3f;

    public KidState Id => KidState.Wandering;
    public void Enter(KidsContext context)
    {
        context.needs.isActive = true;
        Debug.Log("입장 완료. 요구 상태 단계 하락 기능 시작");
        context.timer = Random.Range(waitMin, waitMax);
    }

    public void Tick(KidsContext context, float deltaTime)
    {
        for (int i = 0; i < context.needs.kidStatus.Length; i++)
        {
            if (context.needs.kidStatus[i] <= NeedLevel.Normal)
            {
                context.requestingNeed = (NeedType)i;
                context.agent.RequestWait();
                return;
            }
        }
        
        if (context.agent.IsMove) return;
        
        context.timer -= deltaTime;
        if (context.timer > 0f)
        {
            return;
        }
        else
        {
            if (Random.Range(0, 100) < moveRate)
            {
                var occupied = KidsManager.instance.GetOccupied(context.agent);
                if (context.agent.FindDestination(ZoneType.MainRoom, occupied, out var des))
                {
                    context.agent.RequestPath(des, occupied);
                }
            }
            else
            {
                context.agent.FaceRandomDir();
            }
        }
        context.timer = Random.Range(waitMin, waitMax);
    }

    public void Exit(KidsContext context)
    {
        
    }
}
