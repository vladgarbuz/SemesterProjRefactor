using ProjectAurora.Data;
using ProjectAurora.Domain;
using System.Linq;

namespace ProjectAurora.Data.Rooms
{
    public class GardenRoom : Room
    {
        public GardenRoom(string id, string name, string description) : base(id, name, description)
        {
        }

        public override void OnAfterEnter(Player player, GameState state, GameEngine engine)
        {
            if (state.BoxVisited && !player.HasItem("code") && !this.Items.Any(i => i.Name == "code"))
            {
                this.AddItem(new Item("code", "A small piece of paper with the code written on it."));
                engine.Print("You notice a small piece of paper on the ground that you didn't see before. ('take code')");
            }
        }

        public override bool OnTakeItem(string itemName, Player player, GameState state, GameEngine engine)
        {
            if (itemName.ToLower() == "code")
            {
                if (!player.HasItem("code"))
                {
                    var item = new Item("code", "A small piece of paper with the code written on it.");
                    player.AddItem(item);
                    engine.Print("You took the code. It reads: 'Rigby'.");
                    return true;
                }
                else
                {
                    engine.Print("I don't see that here.");
                    return true; // handled as 'not present'
                }
            }

            return false;
        }
    }
}
