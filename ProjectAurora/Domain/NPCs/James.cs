using ProjectAurora.Data;

namespace ProjectAurora.Domain.NPCs
{
    public class James : ITalkable
    {
        public void Talk(Player player, GameState state, System.Action<string> print, GameEngine engine)
        {
            print("James: 'This separator is the heart of the operation. We extract steam, generate power, then reinject water to maintain the reservoir. The system runs 24/7, unlike solar or wind.'");
        }
    }
}
