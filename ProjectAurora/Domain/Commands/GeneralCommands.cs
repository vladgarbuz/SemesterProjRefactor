using System.Linq;

namespace ProjectAurora.Domain.Commands
{
    public class MoveCommand : ICommand
    {
        public void Execute(string[] args, GameEngine engine)
        {
            if (args.Length == 0)
            {
                engine.Print("Move where?");
                return;
            }
            engine.Move(args[0]);
        }
    }

    public class LookCommand : ICommand
    {
        public void Execute(string[] args, GameEngine engine)
        {
            engine.Look();
        }
    }

    public class TakeCommand : ICommand
    {
        public void Execute(string[] args, GameEngine engine)
        {
            if (args.Length == 0)
            {
                engine.Print("Take what?");
                return;
            }
            // Join args in case item name has spaces (though parser splits by space, we might need to handle multi-word items)
            string itemName = string.Join(" ", args);
            engine.TakeItem(itemName);
        }
    }

    public class UseCommand : ICommand
    {
        public void Execute(string[] args, GameEngine engine)
        {
            if (args.Length == 0)
            {
                engine.Print("Use what?");
                return;
            }
            string itemName = string.Join(" ", args);
            engine.UseItem(itemName);
        }
    }

    public class InventoryCommand : ICommand
    {
        public void Execute(string[] args, GameEngine engine)
        {
            engine.Inventory();
        }
    }

    public class TalkCommand : ICommand
    {
        public void Execute(string[] args, GameEngine engine)
        {
            engine.Talk();
        }
    }
    


    public class HelpCommand : ICommand
    {
        public void Execute(string[] args, GameEngine engine)
        {
            engine.Help();
        }
    }
}
