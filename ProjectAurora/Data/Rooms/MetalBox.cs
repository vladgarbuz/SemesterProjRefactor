using System.Data;
using ProjectAurora.Data;
using ProjectAurora.Domain;

namespace ProjectAurora.Data.Rooms
{
    public class MetalBox: Room
    {
        public MetalBox(string id, string name, string description) : base(id, name, description)
        {
            
        }

        public override bool OnEnter(Player player, GameState state, GameEngine engine)
        {
            state.MarkBoxVisited();
            return true;
        }
        
        public override bool OnUseItem(Item item, Player player, GameState state, GameEngine engine)
        {
            if (item.Name.ToLower() == "code")
            {
                player.RemoveItem("code");
                player.AddItem(new ProjectAurora.Data.Items.RepairItem("anemometer", "An anemometer."));
                engine.Print("You found the anemometer! (Anemometer added to inventory)");
                return true;
            }
            return false;
        }
    }
}