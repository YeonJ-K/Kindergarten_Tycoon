using Unity.VisualScripting;

namespace YEONJI.Kindergarten
{
    public class DoingState : IKidState
    {
        public KidState Id => KidState.Doing;
        public void Enter(KidsContext context)
        {
            context.agent.RequestProcessing();
            context.usingFurniture.SetUsingKidProfile(context.agent);
            context.timer = context.usingFurniture.GetUsingSec();
        }
        public void Tick(KidsContext context, float delTime)
        {
            context.timer -= delTime;
            if (context.timer <= 0)
            {
                context.needs.Recovery(context.playerProcessNeed);
                context.usingFurniture.Release(context.agent);
                context.agent.RequestProcessingFinish();
                context.agent.RequestClear();
                context.machine.ChangeState(new WanderingState());
            }
        }
        public void Exit(KidsContext context)
        {
            context.usingFurniture = null;
        }
    }
}