using ProjectAurora.Data;

namespace ProjectAurora.Domain.NPCs
{
    public class TurbinesOperator : ProjectAurora.Domain.ITalkable
    {
        public void Talk(Player player, GameState state, System.Action<string> print, ProjectAurora.Domain.GameEngine engine)
        {
            if (state.Step2Complete)
            {
                state.MarkWindyRestored();
                print("Prof. Kael: 'They're working! Thank you!' ");
                engine.ReturnToHub();
            }
            else
            {
                print("The turbines are spinning slowly.");
            }
        }
    }
}
