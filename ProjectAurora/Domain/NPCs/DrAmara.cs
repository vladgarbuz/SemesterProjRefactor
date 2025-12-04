using ProjectAurora.Data;

namespace ProjectAurora.Domain.NPCs
{
    public class DrAmara : ProjectAurora.Domain.ITalkable
    {
        public void Talk(Player player, GameState state, System.Action<string> print, ProjectAurora.Domain.GameEngine engine)
        {
            print("Dr. Amara: 'The dam's pipes are frozen. Use berry juice and pinecone dust to derust the lever.'");
        }
    }
}
