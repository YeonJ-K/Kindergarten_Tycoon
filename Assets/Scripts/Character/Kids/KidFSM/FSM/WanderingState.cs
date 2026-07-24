using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class WanderingState : IKidState
    {
        private readonly float moveRate = 70f;
        private readonly float waitMin = 1f;
        private readonly float waitMax = 3f;

        public KidState Id => KidState.Wandering;

        public void Enter(KidsContext context)
        {
            context.needs.isActive = true;
            context.timer = Random.Range(waitMin, waitMax);
        }

        public void Tick(KidsContext context, float deltaTime)
        {
            if (context.needs.wantPlay)
                InGameCore.ROUND.RequestPlaying(context.agent);

            // Moving To Zone에서 PlayRoom 가면 wantPlay = fasle 처리 하기

            if (context.needs.levelChanged)
            {
                context.needs.levelChanged = false;
                NeedLevel worst = context.needs.GetWorst();
                KidEmotion newEmotion = (worst == NeedLevel.VeryBad) ? KidEmotion.Angry
                    : (worst <= NeedLevel.Normal) ? KidEmotion.Tired
                    : KidEmotion.Normal;

                if (newEmotion != context.currentEmotion)
                {
                    context.currentEmotion = newEmotion;
                    if (newEmotion == KidEmotion.Angry) context.agent.Angry();
                    else if (newEmotion == KidEmotion.Tired) context.agent.RequestWait();
                    else context.agent.RequestClear();
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
                    var occupied = InGameCore.KIDS.GetOccupied(context.agent);
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
}