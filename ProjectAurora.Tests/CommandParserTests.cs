using ProjectAurora.Domain;
using Xunit;

namespace ProjectAurora.Tests
{
    public class CommandParserTests
    {
        [Fact]
        public void InsideCommand_IsUnknown()
        {
            var engine = new GameEngine();
            var parser = new CommandParser();
            engine.ClearOutput();

            parser.ParseAndExecute("inside", engine);

            Assert.Contains("I don't know that command", engine.OutputMessage);
        }

        [Fact]
        public void OutsideCommand_IsUnknown()
        {
            var engine = new GameEngine();
            var parser = new CommandParser();
            engine.ClearOutput();

            parser.ParseAndExecute("outside", engine);

            Assert.Contains("I don't know that command", engine.OutputMessage);
        }

        [Fact]
        public void BackCommand_IsUnknown()
        {
            var engine = new GameEngine();
            var parser = new CommandParser();
            engine.ClearOutput();

            parser.ParseAndExecute("back", engine);

            Assert.Contains("I don't know that command", engine.OutputMessage);
        }

        [Fact]
        public void NorthCommand_MovesPlayer()
        {
            var engine = new GameEngine();
            var parser = new CommandParser();
            engine.ClearOutput();

            parser.ParseAndExecute("north", engine);

            Assert.Equal("HydroHub", engine.Player.CurrentRoom.ID);
            // Should also auto-look
            Assert.Contains("Exits:", engine.OutputMessage);
            Assert.Contains("Hydro Hub", engine.OutputMessage);
        }

        [Fact]
        public void HelpCommand_DoesNotDisplayMap()
        {
            var engine = new GameEngine();
            var parser = new CommandParser();
            engine.ClearOutput();

            parser.ParseAndExecute("help", engine);

            Assert.DoesNotContain("PROJECT AURORA MAP", engine.OutputMessage);
            Assert.Contains("Type 'map' to display the world map", engine.OutputMessage);
        }

        [Fact]
        public void MapCommand_DisplaysMap()
        {
            var engine = new GameEngine();
            var parser = new CommandParser();
            engine.ClearOutput();

            parser.ParseAndExecute("map", engine);

            Assert.Contains("PROJECT AURORA MAP", engine.OutputMessage);
        }
    }
}
