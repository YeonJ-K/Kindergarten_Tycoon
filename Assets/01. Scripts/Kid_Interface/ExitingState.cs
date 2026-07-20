using Infos;
using UnityEngine;

public class ExitingState : IKidState
{
    public KidState Id => KidState.Exiting;
    public void Enter(KidsContext context)
    {
        Vector2Int exitPos = RoundManager.instance.kidsSpawnPos;
        context.agent.RequestPath(exitPos, null);
    }

    public void Tick(KidsContext context, float deltaTime)
    {
        if (!context.agent.IsMove)
        {  
            if (context.agent.CurrentCell == RoundManager.instance.kidsSpawnPos)
                context.wantExit = true;
        }
    }

    public void Exit(KidsContext context) { }
}
