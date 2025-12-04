using ProjectAurora.Data;
using ProjectAurora.Domain;

namespace ProjectAurora.Data.Rooms
{
    public class TentsRoom : Room
    {
        public TentsRoom(string id, string name, string description) : base(id, name, description) { }

        public override bool OnUseItem(Item item, Player player, GameState state, GameEngine engine)
        {
            if (item.Name.ToLower() == "snack")
            {
                state.MarkFedRaccoon();
                player.RemoveItem("snack");
                player.AddItem(new ProjectAurora.Data.Items.RepairItem("control board", "A small control board."));
                engine.Print("You fed the raccoon. It dropped the control board!");
                return true;
            }
            return false;
        }
    }
}
