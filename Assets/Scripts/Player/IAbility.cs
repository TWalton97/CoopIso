public interface IAbility
{
    void OnEnter(NewPlayerController controller);
    void OnExit();
    bool CanUse(ResourceController resourceController);
}
