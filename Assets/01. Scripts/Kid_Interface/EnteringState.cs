using Infos;
using UnityEngine;

public class EnteringState : IKidState
{
    public KidState Id => KidState.Entering;
    public void Enter(KidsContext context)
    {
        context.timer = 0f;
        var occupied = KidsManager.instance.GetOccupied(context.agent);
        if (context.agent.FindDestination(ZoneType.MainRoom, occupied, out var des))
            context.agent.RequestPath(des, null);
    }

    public void Tick(KidsContext context, float deltaTime)
    {
        if (context.agent.IsMove) return;
        var cell = GridMap.instance.GetCell(context.agent.CurrentCell.x, context.agent.CurrentCell.y);
        if (cell.zone == ZoneType.MainRoom)
        {
            context.machine.ChangeState(new WanderingState());
            return;
        }
        
        context.timer -= deltaTime;
        if (context.timer <= 0)
        {
            var occupied = KidsManager.instance.GetOccupied(context.agent);
            if (context.agent.FindDestination(ZoneType.MainRoom, occupied, out var des))
                context.agent.RequestPath(des, null);
            context.timer = 0.5f;
        }
    }

    public void Exit(KidsContext context)
    {
        
    }
}
