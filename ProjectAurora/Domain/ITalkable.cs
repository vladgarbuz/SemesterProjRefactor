using ProjectAurora.Data;

namespace ProjectAurora.Domain
{
    public interface ITalkable
    {
        void Talk(Player player, GameState state, System.Action<string> print, GameEngine engine);
    }
}
