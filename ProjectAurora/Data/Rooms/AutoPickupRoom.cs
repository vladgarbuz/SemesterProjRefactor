using System.Linq;
using ProjectAurora.Domain;
using ProjectAurora.Data;

namespace ProjectAurora.Data.Rooms
{
    public class AutoPickupRoom : Room
    {
        public string ItemNameToAutoPickup { get; }

        public AutoPickupRoom(string id, string name, string description, string itemToPickup) : base(id, name, description)
        {
            ItemNameToAutoPickup = itemToPickup;
        }

        public override bool OnEnter(Player player, GameState state, GameEngine engine)
        {
            if (!player.HasItem(ItemNameToAutoPickup))
            {
                var item = Items.FirstOrDefault(i => i.Name == ItemNameToAutoPickup);
                if (item != null)
                {
                    player.AddItem(item);
                    RemoveItem(item);
                    engine.Print($"You found a {ItemNameToAutoPickup} and took it.");
                }
            }
            return true;
        }
    }
}
