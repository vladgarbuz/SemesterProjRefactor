using ProjectAurora.Data;

namespace ProjectAurora.Domain.NPCs
{
    public class DrElenaVoss : ITalkable
    {
        public void Talk(Player player, GameState state, System.Action<string> print, GameEngine engine)
        {
            if (player.HasItem("thermal_data"))
            {
                print("Dr. Elena Voss: 'Excellent! The data shows this reservoir can provide sustainable energy for decades. Geothermal heat keeps regenerating naturally.'");
            }
            else
            {
                print("Dr. Elena Voss: 'I study thermal activity here. The Steam Vents show incredible heat at 2km depth! Collect thermal data from there to help our research.'");
            }
        }
    }
}
