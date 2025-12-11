using ProjectAurora.Data;
using ProjectAurora.Domain;

namespace ProjectAurora.Data.Rooms
{
    public class SteamVentsRoom : Room
    {
        public SteamVentsRoom(string id, string name, string description) : base(id, name, description) { }

        public override bool OnTakeItem(string itemName, Player player, GameState state, GameEngine engine)
        {
            if (itemName.ToLower() == "thermal_data")
            {
                var item = Items[0]; // thermal_data should be the only item
                if (item != null && item.Name.ToLower() == "thermal_data")
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
