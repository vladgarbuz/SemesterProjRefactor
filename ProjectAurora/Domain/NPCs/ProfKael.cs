using ProjectAurora.Data;

namespace ProjectAurora.Domain.NPCs
{
    public class ProfKael : ProjectAurora.Domain.ITalkable
    {
        public void Talk(Player player, GameState state, System.Action<string> print, ProjectAurora.Domain.GameEngine engine)
        {
            if (player.HasItem("anemometer") && player.HasItem("control board") && player.HasItem("power cables"))
            {
                state.MarkStep1Complete();
                print("Prof. Kael: 'Great! You have the parts. Let's fix this.' (Step 1 Complete)");
            }
            else if (player.HasItem("anemometer") && player.HasItem("control board") && player.HasItem("flimsy cables"))
            {
                print("Prof. Kael: 'These cables are too flimsy...' ");
            }
            else
            {
                if (!player.HasItem("Shed Key"))
                {
                        player.AddItem(new ProjectAurora.Data.Item("Shed Key", "Key to the shed."));
                    print("Prof. Kael: 'I need parts. Here is the Shed Key.'");
                }
                else
                {
                    print("Prof. Kael: 'Bring me the parts!'");
                }
            }
        }
    }
}
