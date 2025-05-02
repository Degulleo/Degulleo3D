public interface IPlayerAction {
    void StartAction(PlayerController player);
    void UpdateAction();
    void EndAction();
    bool IsActive { get; }
}