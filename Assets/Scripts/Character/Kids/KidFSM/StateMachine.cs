using UnityEngine;

namespace YEONJI.Kindergarten
{
    /// <summary>
    /// 지금 어느 상태인지 들고 있다가, 요청이 오면 바꿔주는 관리자
    /// 1) 유치원생이 하고 있는 지금의 현재 State를 보관합니다.
    /// 2) 상태를 전환시킵니다. 
    /// </summary>
    public class StateMachine
    {
        private IKidState currentState;
        private KidsContext context;

        public StateMachine(KidsContext ctx, IKidState initialState)
        {
            context = ctx;
            ChangeState(initialState);
        }

        public void ChangeState(IKidState newState)
        {
            // 지금 상태가 있으면 그 상태의 Exit를 부른다.
            if (currentState != null)
                currentState.Exit(context);
            // 현재 상태를 newState로 바꾼다.
            currentState = newState;
            // 새 상태의 Enter를 부른다.
            currentState.Enter(context);
        }

        public void Tick(float delTime)
        {
            // 현재 상태의 Tick을 부른다.
            if (currentState != null)
                currentState.Tick(context, delTime);
        }
    }
}
