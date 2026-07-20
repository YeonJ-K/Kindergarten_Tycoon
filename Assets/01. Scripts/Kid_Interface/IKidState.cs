using Infos;

public interface IKidState
{
    KidState Id { get; }
    void Enter(KidsContext context);
    void Tick(KidsContext context, float delTime);
    void Exit(KidsContext context);
}
