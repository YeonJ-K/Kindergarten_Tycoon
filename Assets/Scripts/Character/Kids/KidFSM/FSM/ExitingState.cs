using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class ExitingState : IKidState
    {
        public KidState Id => KidState.Exiting;

        public void Enter(KidsContext context)
        {
            context.needs.isActive = false;
            Vector2Int exitPos = InGameCore.ROUND.kidsSpawnPos;
            context.agent.RequestPath(exitPos, null);
        }

        public void Tick(KidsContext context, float deltaTime)
        {
            if (!context.agent.IsMove)
            {
                if (context.agent.CurrentCell == InGameCore.ROUND.kidsSpawnPos)
                {
                    context.wantExit = true;
                    return;
                }

                context.timer -= deltaTime;
                if (context.timer <= 0)
                {
                    var occupied = InGameCore.KIDS.GetOccupied(context.agent);
                    context.agent.RequestPath(InGameCore.ROUND.kidsSpawnPos, occupied);
                    context.timer = 0.5f;
                }
            }
        }

        public void Exit(KidsContext context)
        {
        }
    }
}