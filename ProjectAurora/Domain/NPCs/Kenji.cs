using ProjectAurora.Data;

namespace ProjectAurora.Domain.NPCs
{
    public class Kenji : ITalkable
    {
        public void Talk(Player player, GameState state, System.Action<string> print, GameEngine engine)
        {
            print("Kenji: 'These springs have been here for generations. The power plant taps deeper reservoirs - the springs remain untouched. Perfect balance between energy and nature.'");
        }
    }
}
