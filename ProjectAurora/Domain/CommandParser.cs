using System.Collections.Generic;
using System.Linq;
using ProjectAurora.Domain.Commands;

namespace ProjectAurora.Domain
{
    public class CommandParser
    {
        private readonly Dictionary<string, ICommand> _commands;
        private readonly HashSet<string> _directions;

        public CommandParser()
        {
            var moveCommand = new MoveCommand();
            _commands = new Dictionary<string, ICommand>
            {
                { "go", moveCommand },
                { "move", moveCommand },
                { "north", moveCommand },
                { "south", moveCommand },
                { "east", moveCommand },
                { "west", moveCommand },
                { "up", moveCommand },
                { "down", moveCommand },
                { "left", moveCommand },
                { "right", moveCommand },
                { "look", new LookCommand() },
                { "take", new TakeCommand() },
                { "use", new UseCommand() },
                { "inventory", new InventoryCommand() },
                { "inv", new InventoryCommand() },
                { "i", new InventoryCommand() },
                { "talk", new TalkCommand() },
                { "help", new HelpCommand() },
                { "map", new MapCommand() }
            };

            _directions = new HashSet<string>
            {
                // Removed 'inside' and 'outside' in favor of using cardinals for all moves
                "north", "south", "east", "west", "up", "down", "left", "right"
            };
        }

        public void ParseAndExecute(string input, GameEngine engine)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            var parts = input.Trim().ToLower().Split(' ');
            var commandName = parts[0];
            var args = parts.Skip(1).ToArray();

            if (_commands.ContainsKey(commandName))
            {
                if (_directions.Contains(commandName) && args.Length == 0)
                {
                    args = new string[] { commandName };
                }

                _commands[commandName].Execute(args, engine);
            }
            else
            {
                engine.Print("I don't know that command.");
            }
        }
    }
}
