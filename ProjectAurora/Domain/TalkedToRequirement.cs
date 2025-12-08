using ProjectAurora.Data;

namespace ProjectAurora.Domain
{
    public class TalkedToRequirement : IEntryRequirement
    {
        private readonly string _npcName;
        public TalkedToRequirement(string npcName)
        {
            _npcName = npcName;
        }

        public string DeniedMessage => $"Access denied. You should talk to {_npcName} first.";

        public bool CanEnter(Player player, GameState state)
        {
            // Compatibility: check dynamic flag based on NPC name OR existing concrete property
            return state.GetFlag($"TalkedTo{_npcName}") || state.TalkedToLiora;
        }

        public void OnEnter(Player player, GameState state, GameEngine engine)
        {
            // No action needed for talked-to requirements
        }
    }
}
