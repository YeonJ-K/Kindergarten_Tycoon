using Infos;
using UnityEngine;

public class WaitingState : IKidState
{
    public KidState Id => KidState.Waiting;

    public void Enter(KidsContext context)
    {
        context.agent.StopMove();
    }

    public void Tick(KidsContext context, float dt)
    {
        if (context.releaseWaiting)
        {
            context.releaseWaiting = false;
            context.machine.ChangeState(new WanderingState());
        }
    }

    public void Exit(KidsContext context)
    {
        
    }
}
