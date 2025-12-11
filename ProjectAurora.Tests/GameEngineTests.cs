using ProjectAurora.Domain;
using Xunit;

namespace ProjectAurora.Tests
{
    public class GameEngineTests
    {
        [Fact]
        public void Move_ChangesRoomAndAutoLook()
        {
            var engine = new GameEngine();
            engine.ClearOutput();

            engine.Move("west"); // hub -> solarDesert

            Assert.Equal("SolarDesert", engine.Player.CurrentRoom.ID);
            Assert.Contains("Exits:", engine.OutputMessage);
            Assert.Contains("You move to", engine.OutputMessage);
            // Verify auto look printed the description
            Assert.Contains("Solar Desert", engine.OutputMessage);
        }

        [Fact]
        public void Back_UsesPreviousRoomAndAutoLook()
        {
            // The 'back' command has been removed; parsing 'back' should be unknown
            var engine = new GameEngine();
            var parser = new CommandParser();
            engine.ClearOutput();

            parser.ParseAndExecute("back", engine);

            Assert.Contains("I don't know that command", engine.OutputMessage);
        }

        [Fact]
        public void UseItem_WrongPlace_PrintsMessage()
        {
            var engine = new GameEngine();
            engine.ClearOutput();

            // Give the player a lever but not at the Control Room
            engine.Player.AddItem(new ProjectAurora.Data.Items.RepairItem("lever", "A heavy, stainless steel lever."));
            engine.ClearOutput();
            engine.UseItem("lever");
            Assert.Contains("You can't use that here.", engine.OutputMessage);
            // Lever remains in inventory
            Assert.True(engine.Player.HasItem("lever"));
        }

        [Fact]
        public void SolarQuiz_Success_MovesToTentAndGivesKey_AutoLook()
        {
            var engine = new GameEngine();
            engine.ClearOutput();

            // Allow access to the tent and stub the quiz to return 1 correct answer (pass)
            engine.State.MarkTalkedToLiora();
            engine.RunQuiz = (q) => 1;

            // Navigate to the maintenance tent: hub -> solarDesert -> desertHub -> maintTent (west)
            engine.Move("west");
            engine.Move("west");
            engine.Move("west");

            Assert.Equal("MaintTent", engine.Player.CurrentRoom.ID);
            Assert.Contains("Correct! You enter the tent.", engine.OutputMessage);
            // Auto-look should also have printed a description
            Assert.Contains("Exits:", engine.OutputMessage);
            
            // Key should be in the room as an item
            engine.ClearOutput();
            engine.TakeItem("Desert Key");
            Assert.True(engine.Player.HasItem("Desert Key"));
            Assert.Contains("You took the Desert Key", engine.OutputMessage);
        }

        [Fact]
        public void SolarQuiz_NotAsked_AfterCompletion()
        {
            var engine = new GameEngine();
            engine.ClearOutput();

            // Allow access to the tent and stub the quiz to return 1 correct answer (pass)
            engine.State.MarkTalkedToLiora();
            int called = 0;
            engine.RunQuiz = (q) => { called++; return 1; };

            // Navigate to the maintenance tent: hub -> solarDesert -> desertHub -> maintTent (west)
            engine.Move("west");
            engine.Move("west");
            engine.Move("west");

            // After successful quiz, player should be in MaintTent
            Assert.Equal("MaintTent", engine.Player.CurrentRoom.ID);
            Assert.True(engine.State.QuizCompleted);
            Assert.Equal(1, called);

            // Move out of the tent and re-enter the same room
            engine.Move("east"); // back to DesertHub
            engine.ClearOutput();
            engine.Move("west"); // attempt to re-enter

            // Verify we entered successfully and the quiz function wasn't called again
            Assert.Equal("MaintTent", engine.Player.CurrentRoom.ID);
            // Quiz shouldn't be called again
            Assert.Equal(1, called);
        }

        [Fact]
        public void SolarQuiz_Failure_DoesNotGiveKey()
        {
            var engine = new GameEngine();
            engine.ClearOutput();

            engine.State.MarkTalkedToLiora();
            engine.RunQuiz = (q) => 0; // fail - 0 correct answers

            engine.Move("west");
            engine.Move("west");
            engine.Move("west");

            // Player should not be moved to the tent, and not receive the key
            Assert.Equal("DesertHub", engine.Player.CurrentRoom.ID);
            Assert.False(engine.Player.HasItem("Desert Key"));
        }

        [Fact]
        public void Map_ShowsSingleTentNode()
        {
            var engine = new GameEngine();
            engine.ClearOutput();
            engine.PrintMap();
            Assert.Contains(" TENT ", engine.OutputMessage);
        }

        [Fact]
        public void MaintTent_Goes_East_To_DesertHub()
        {
            var engine = new GameEngine();
            engine.ClearOutput();

            engine.State.MarkTalkedToLiora();
            
            engine.RunQuiz = (q) => 1; // pass quiz to get into tent
            engine.Move("west");
            engine.Move("west");
            engine.Move("west"); // into tent
            Assert.Equal("MaintTent", engine.Player.CurrentRoom.ID);

            engine.ClearOutput();
            engine.Move("east"); // should go back to DesertHub
            Assert.Equal("DesertHub", engine.Player.CurrentRoom.ID);
        }
    }
}
