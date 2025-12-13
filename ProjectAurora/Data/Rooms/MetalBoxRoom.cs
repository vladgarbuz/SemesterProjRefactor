using ProjectAurora.Data;
using ProjectAurora.Domain;

namespace ProjectAurora.Data.Rooms
{
    public class MetalBoxRoom : Room
    {
        private bool _isOpen = false;

        public MetalBoxRoom(string id, string name, string description) : base(id, name, description) { }

        public override bool OnEnter(Player player, GameState state, GameEngine engine)
        {
            if (!state.BoxVisited)
            {
                state.MarkBoxVisited();
                engine.Print("You notice the metal box has a keypad. You'll need a code to open it. Maybe you should explore the area more...");
            }
            return base.OnEnter(player, state, engine);
        }

        public override bool OnUseItem(Item item, Player player, GameState state, GameEngine engine)
        {
            if (item.Name.ToLower() == "code")
            {
                if (_isOpen)
                {
                    engine.Print("The box is already open.");
                    return true;
                }

                // Display the prompt immediately
                System.Console.WriteLine("Enter the code to open the metal box:");
                System.Console.Write("> ");
                string? userInput = System.Console.ReadLine();
                
                if (userInput?.Trim().Equals("Rigby", System.StringComparison.OrdinalIgnoreCase) == true)
                {
                    engine.Print("The metal box clicks open! Inside, you find an anemometer (take anemometer).");
                    player.RemoveItem("code");
                    _isOpen = true;
                    
                    // Add the anemometer to the room
                    this.AddItem(new ProjectAurora.Data.Items.RepairItem("anemometer", "A precision anemometer for measuring wind speed."));
                }
                else
                {
                    engine.Print("Incorrect code. The box remains locked.");
                }
                
                return true;
            }
            return false;
        }
    }
}
