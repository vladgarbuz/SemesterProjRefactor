using ProjectAurora.Data;

namespace ProjectAurora.Domain.NPCs
{
    public class ComputersTerminal : ProjectAurora.Domain.ITalkable
    {
        public void Talk(Player player, GameState state, System.Action<string> print, ProjectAurora.Domain.GameEngine engine)
        {
            if (state.Step1Complete)
            {
                state.MarkStep2Complete();
                print("Prof. Kael: 'Computers are on! Let's go to the turbines.'");
            }
            else
            {
                print("The computers are flickering.");
            }
        }
    }
}
