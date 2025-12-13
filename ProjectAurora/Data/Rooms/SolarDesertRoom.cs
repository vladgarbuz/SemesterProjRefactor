using ProjectAurora.Data;
using ProjectAurora.Domain;

namespace ProjectAurora.Data.Rooms
{
    public class SolarDesertRoom : Room
    {
        private const string DesolateDescription = "After walking for hours you find yourself in a desolate land. The desert stretches before you. Towers of sand cover the solar field. Heat shimmers across the horizon. You find a small hub that looks like it could have life(west)";
        private const string NeutralDescription = "You are in the vast Solar Desert. The sun beats down on the sand. To the west lies the Desert Hub, and to the east is the Aurora Control Hub.";

        public SolarDesertRoom(string id, string name, string description) : base(id, name, description) { }

        public override bool OnEnter(Player player, GameState state, GameEngine engine)
        {
            if (player.CurrentRoom.ID == "SolarFields")
            {
                Description = DesolateDescription;
            }
            else
            {
                Description = NeutralDescription;
            }
            return base.OnEnter(player, state, engine);
        }
    }
}
