using ProjectAurora.Data;

namespace ProjectAurora.Domain.NPCs
{
    public class Raccoon : ProjectAurora.Domain.ITalkable
    {
        public void Talk(Player player, GameState state, System.Action<string> print, ProjectAurora.Domain.GameEngine engine)
        {
            print("You found a raccoon! It seems to be playing with a control board.");
        }
    }
}
