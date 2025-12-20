using ProjectAurora.Data;
using System.Linq;

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
                // Remove used items
                player.RemoveItem("anemometer");
                player.RemoveItem("control board");
                player.RemoveItem("power cables");
            }
            else if (player.HasItem("anemometer") && player.HasItem("control board") && player.HasItem("flimsy cables"))
            {
                print("Prof. Kael: 'These cables are too flimsy...' ");
            }
            else
            {
                if (!player.HasItem("Shed Key") && !player.CurrentRoom.Items.Any(i => i.Name == "Shed Key"))
                {
                    player.CurrentRoom.AddItem(new ProjectAurora.Data.Items.KeyItem("Shed Key", "A rusty key labeled 'Shed'."));
                    print("Prof. Kael: 'I need parts to fix the turbine. Here, take this key to the shed, you might find something useful there. Please find an anemometer, a control board, and some sturdy power cables.' (Prof. Kael drops the Shed Key 'take shed key')");
                }
                else if (player.HasItem("Shed Key"))
                {
                    print("Prof. Kael: 'I need parts to fix the turbine. Please find an anemometer, a control board, and some sturdy power cables.'");
                }
                else
                {
                    print("Prof. Kael: 'I need parts to fix the turbine. The key to the shed is on the floor if you need it. Please find an anemometer, a control board, and some sturdy power cables.'");
                }
            }
        }
    }
}
