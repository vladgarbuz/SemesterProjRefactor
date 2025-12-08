using ProjectAurora.Data;

namespace ProjectAurora.Domain.NPCs
{
    public class Raccoon : ProjectAurora.Domain.ITalkable
    {
        public void Talk(Player player, GameState state, System.Action<string> print, ProjectAurora.Domain.GameEngine engine)
        {
            if (!state.FedRaccoon)
            {
                // Hint the player the raccoon might want a snack if it hasn't been fed yet
                print("You found a raccoon! It seems to be playing with a control board. It looks hungry — maybe try feeding it a snack?");
            }
            else
            {
                print("You see a raccoon that looks content after being fed earlier.");
            }
        }
    }
}
