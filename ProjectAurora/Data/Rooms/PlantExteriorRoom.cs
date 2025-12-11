using ProjectAurora.Data;
using ProjectAurora.Domain;

namespace ProjectAurora.Data.Rooms
{
    public class PlantExteriorRoom : Room
    {
        public PlantExteriorRoom(string id, string name, string description) : base(id, name, description) { }

        public override bool OnEnter(Player player, GameState state, GameEngine engine)
        {
            // Auto-interaction: Chief Rodriguez gives permit on first visit
            // The actual giving of the permit is handled by the NPC's Talk method
            // This room just ensures the player knows to talk
            if (!state.TalkedToRodriguez && Occupant != null)
            {
                engine.Print("Chief Engineer Rodriguez approaches you...");
            }
            return true;
        }
    }
}
