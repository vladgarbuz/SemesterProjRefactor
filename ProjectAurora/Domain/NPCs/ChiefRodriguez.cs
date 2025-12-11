using ProjectAurora.Data;

namespace ProjectAurora.Domain.NPCs
{
    public class ChiefRodriguez : ITalkable
    {
        public void Talk(Player player, GameState state, System.Action<string> print, GameEngine engine)
        {
            if (!state.TalkedToRodriguez)
            {
                state.MarkTalkedToRodriguez();
                // Add permit to the room for player to pick up
                if (!player.CurrentRoom.Items.Any(i => i.Name.ToLower() == "permit"))
                {
                    player.CurrentRoom.AddItem(new Item("permit", "A permit from Chief Rodriguez allowing you to take the geothermal certification quiz."));
                }
                print("Chief Engineer Rodriguez: 'Welcome! This plant generates 50 megawatts constantly. Geothermal never stops, unlike solar or wind. I'm placing a permit here for you - take it and use it at the observatory for the certification quiz.'");
            }
            else
            {
                print("Chief Engineer Rodriguez: 'Geothermal connects everything - natural heat, monitoring, processing, and generation. You're seeing the complete cycle of sustainable energy.'");
            }
        }
    }
}
