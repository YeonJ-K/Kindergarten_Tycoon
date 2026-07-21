using Infos;
using UnityEngine;

public class ExitingState : IKidState
{
    public KidState Id => KidState.Exiting;
    public void Enter(KidsContext context)
    {
        context.needs.isActive = false;
        Vector2Int exitPos = RoundManager.instance.kidsSpawnPos;
        context.agent.RequestPath(exitPos, null);
    }

    public void Tick(KidsContext context, float deltaTime)
    {
        if (!context.agent.IsMove)
        {
            if (context.agent.CurrentCell == RoundManager.instance.kidsSpawnPos)
            {
                context.wantExit = true;
                return;
            }
            
            context.timer -= deltaTime;
            if (context.timer <= 0)
            {
                var occupied = KidsManager.instance.GetOccupied(context.agent);
                context.agent.RequestPath(RoundManager.instance.kidsSpawnPos, occupied);
                context.timer = 0.5f;
            }


        }
    }

    public void Exit(KidsContext context) { }
}
