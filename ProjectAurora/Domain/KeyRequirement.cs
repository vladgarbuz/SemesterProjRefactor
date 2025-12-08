using ProjectAurora.Data;

namespace ProjectAurora.Domain
{
    public class KeyRequirement : IEntryRequirement
    {
        private readonly string _keyName;
        private readonly bool _consumeKey;
        private bool _isUnlocked;

        public KeyRequirement(string keyName, bool consumeKey = true)
        {
            _keyName = keyName;
            _consumeKey = consumeKey;
            _isUnlocked = false;
        }

        public string DeniedMessage => $"The door is locked. You need a {_keyName}.";

        public bool CanEnter(Player player, GameState state)
        {
            // If already unlocked (key was consumed), allow entry
            if (_isUnlocked)
                return true;
            
            // Otherwise, check if player has the key
            return player.HasItem(_keyName);
        }

        public void OnEnter(Player player, GameState state, GameEngine engine)
        {
            if (_consumeKey && !_isUnlocked && player.HasItem(_keyName))
            {
                player.RemoveItem(_keyName);
                engine.Print($"The {_keyName} unlocks the door and breaks in the lock.");
                _isUnlocked = true;
            }
        }
        public bool IsUnlocked => _isUnlocked;
    }
}
