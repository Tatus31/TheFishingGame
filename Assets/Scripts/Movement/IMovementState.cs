public interface IMovementState
{
    void EnterState(PlayerMovement player);
    void ExitState();
    void UpdateState();
    void FixedUpdateState();
}
