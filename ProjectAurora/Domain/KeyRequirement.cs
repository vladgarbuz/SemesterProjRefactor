using ProjectAurora.Data;

namespace ProjectAurora.Domain
{
    public class KeyRequirement : IEntryRequirement
    {
        private readonly string _keyName;
        public KeyRequirement(string keyName)
        {
            _keyName = keyName;
        }

        public string DeniedMessage => $"The door is locked. You need a {_keyName}.";

        public bool CanEnter(Player player, GameState state)
        {
            return player.HasItem(_keyName);
        }
    }
}
