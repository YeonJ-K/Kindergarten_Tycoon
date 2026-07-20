using Infos;
using UnityEngine;

/// <summary>
/// 유치원생 객체 하나의 "상태 데이터 + 능력"을 담당
/// State 객체는 Kids가 아닌 이 context만 알고 있는다.
/// 데이터(위치, 타이머, 목표)와 능력(이동 요청, 상태 전환)을 함께 담지만,
/// 실제 이동이나 애니메이션 실행은 KidsAgent에서 담당한다.
///
/// Agent는 판단을 하지 않는다. 어디로 갈지 언제 멈출지에 대한 코드는 State에서 정해서 매개변수로 받아 실행하는 일 뿐
/// </summary>
public class KidsContext
{
    public KidState currentState;
    public float timer;
    public Vector2Int desCell;
    public bool wantExit;

    public KidAgent agent;
    public StateMachine machine;
}
