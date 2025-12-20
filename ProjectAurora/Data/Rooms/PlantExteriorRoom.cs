using ProjectAurora.Data;
using ProjectAurora.Domain;

namespace ProjectAurora.Data.Rooms
{
    public class PlantExteriorRoom : Room
    {
        public PlantExteriorRoom(string id, string name, string description) : base(id, name, description) { }

        public override void OnAfterEnter(Player player, GameState state, GameEngine engine)
        {
            if (!state.TalkedToRodriguez)
            {
                engine.Print("You see Chief Engineer Rodriguez standing near the main entrance.");
            }
        }
    }
}
