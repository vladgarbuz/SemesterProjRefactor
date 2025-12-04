using System.Linq;
using ProjectAurora.Data;
using ProjectAurora.Domain;

namespace ProjectAurora.Data.Rooms
{
    public class TowerRoom : Room
    {
        public TowerRoom(string id, string name, string description) : base(id, name, description)
        {
        }

        public override bool OnTakeItem(string itemName, Player player, GameState state, GameEngine engine)
        {
            if (itemName.ToLower() == "flimsy cables")
            {
                if (player.HasItem("Shed Key"))
                {
                    if (!player.HasItem("flimsy cables"))
                    {
                        var item = new ProjectAurora.Data.Items.RepairItem("flimsy cables", "A bundle of thin, worn cables.");
                        player.AddItem(item);
                        engine.Print($"You took the {item.Name}.");
                        return true;
                    }
                    else
                    {
                        engine.Print("You already have flimsy cables.");
                        return true;
                    }
                }
                else
                {
                    engine.Print("You can't get that just yet.");
                    return true;
                }
            }

            return false;
        }
    }
}
