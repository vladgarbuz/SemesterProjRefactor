namespace ProjectAurora.Domain.Commands
{
    public interface ICommand
    {
        void Execute(string[] args, GameEngine engine);
    }
}
