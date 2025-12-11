using ProjectAurora.Data;
using ProjectAurora.Domain;

namespace ProjectAurora.Data.Rooms
{
    public class SteamVentsRoom : Room
    {
        public SteamVentsRoom(string id, string name, string description) : base(id, name, description) { }

        public override bool OnTakeItem(string itemName, Player player, GameState state, GameEngine engine)
        {
            if (itemName.ToLower() == "thermal data")
            {
                // Check if item still exists in the room
                if (Items.Count == 0)
                {
                    engine.Print("There is no thermal data to take here.");
                    return true;
                }

                var item = Items.FirstOrDefault(i => i.Name.ToLower() == "thermal data");
                if (item != null)
                {
                    player.AddItem(item);
                    RemoveItem(item);
                    state.MarkHasThermalData();
                    engine.Print("You collect thermal data. Readings show 180°C at 2km depth.");
                    return true;
                }
            }
            return false;
        }
    }
}
