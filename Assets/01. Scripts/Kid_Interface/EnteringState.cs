using Infos;
using UnityEngine;

public class EnteringState : IKidState
{
    public KidState Id => KidState.Entering;
    public void Enter(KidsContext context)
    {
        var occupied = KidsManager.instance.GetOccupied(context.agent);
        if (context.agent.FindDestination(ZoneType.MainRoom, occupied, out var des))
            context.agent.RequestPath(des, null);
    }

    public void Tick(KidsContext context, float deltaTime)
    {
        if (!context.agent.IsMove)
        {
            context.machine.ChangeState(new WanderingState());
        }
    }

    public void Exit(KidsContext context)
    {
        
    }
}
