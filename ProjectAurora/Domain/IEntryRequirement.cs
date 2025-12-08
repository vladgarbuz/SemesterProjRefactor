namespace ProjectAurora.Domain
{
    public interface IEntryRequirement
    {
        bool CanEnter(ProjectAurora.Data.Player player, ProjectAurora.Data.GameState state);
        string DeniedMessage { get; }
        void OnEnter(ProjectAurora.Data.Player player, ProjectAurora.Data.GameState state, GameEngine engine);
    }
}
