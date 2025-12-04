using ProjectAurora.Data;

namespace ProjectAurora.Domain.NPCs
{
    public class DrLiora : ProjectAurora.Domain.ITalkable
    {
        public void Talk(Player player, GameState state, System.Action<string> print, ProjectAurora.Domain.GameEngine engine)
        {
            state.MarkTalkedToLiora();
            print("Dr. Liora: 'Welcome! Save the Solar panel field! Visit the maintenance tent (west).'");
        }
    }
}
