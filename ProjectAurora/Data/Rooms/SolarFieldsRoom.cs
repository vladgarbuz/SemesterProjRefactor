using ProjectAurora.Data;
using ProjectAurora.Domain;
using System.Linq;

namespace ProjectAurora.Data.Rooms
{
    public class SolarFieldsRoom : Room
    {
        public SolarFieldsRoom(string id, string name, string description) : base(id, name, description) { }

        public override bool OnUseItem(Item item, Player player, GameState state, GameEngine engine)
        {
            var name = item.Name.ToLower();
            if (name == "water hose")
            {
                // Consumed when used
                if (player.HasItem("Water Hose"))
                    player.RemoveItem("Water Hose");
                engine.Print("Temporary fix applied. Solar Desert saved (barely).");
                state.SetSolarFixed();
                engine.ReturnToHub();
                return true;
            }
            // Check if this is a repair item for robotic parts (more robust than bare substring)
            if (item is ProjectAurora.Data.Items.RepairItem && name.Contains("robotic parts"))
            {
                if (player.HasItem("Robotic Parts 1") && player.HasItem("Robotic Parts 2"))
                {
                    engine.Print("Robotic maintenance complete. Saved the Solar Desert!");
                    state.SetSolarFixed();
                    engine.ReturnToHub();
                    // Consume parts when used
                    if (player.HasItem("Robotic Parts 1")) player.RemoveItem("Robotic Parts 1");
                    if (player.HasItem("Robotic Parts 2")) player.RemoveItem("Robotic Parts 2");
                }
                else
                {
                    engine.Print("You need both parts.");
                }
                return true;
            }
            return false;
        }
    }
}
