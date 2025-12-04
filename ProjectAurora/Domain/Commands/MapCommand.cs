using System;

namespace ProjectAurora.Domain.Commands
{
    public class MapCommand : ICommand
    {
        public void Execute(string[] args, GameEngine engine)
        {
            engine.PrintMap();
        }
    }
}
