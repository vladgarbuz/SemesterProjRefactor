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
        public void SolarQuiz_Success_MovesToTentAndGivesKey_AutoLook()
        {
            var engine = new GameEngine();
            engine.ClearOutput();

            // Allow access to the tent and stub the quiz to return '2' (correct)
            engine.State.MarkTalkedToLiora();
            engine.RunSolarQuiz = (q) => 2;

            // Navigate to the maintenance tent outside: hub -> solarDesert -> desertHub -> maintTentOutside
            engine.Move("west");
            engine.Move("west");
            engine.Move("west");

            Assert.Equal("MaintTent", engine.Player.CurrentRoom.ID);
            Assert.True(engine.Player.HasItem("Desert Key"));
            Assert.Contains("Correct! You enter the tent.", engine.OutputMessage);
            Assert.Contains("You received the Desert Key.", engine.OutputMessage);
            // Auto-look should also have printed a description
            Assert.Contains("Exits:", engine.OutputMessage);
        }

        [Fact]
        public void SolarQuiz_NotAsked_AfterCompletion()
        {
            var engine = new GameEngine();
            engine.ClearOutput();

            // Allow access to the tent and stub the quiz to return '2' (correct)
            engine.State.MarkTalkedToLiora();
            int called = 0;
            engine.RunSolarQuiz = (q) => { called++; return 2; };

            // Navigate to the maintenance tent outside: hub -> solarDesert -> desertHub -> maintTentOutside
            engine.Move("west");
            engine.Move("west");
            engine.Move("west");

            // After successful quiz, player should be in MaintTent
            Assert.Equal("MaintTent", engine.Player.CurrentRoom.ID);
            Assert.True(engine.State.QuizCompleted);
            Assert.Equal(1, called);

            // Move out of the tent and re-enter the same room
            engine.Move("south"); // back to MaintTentOutside
            engine.ClearOutput();
            engine.Move("north"); // attempt to re-enter

            // Verify we did not get asked the quiz again
            Assert.DoesNotContain("What happens if solar panels overheat", engine.OutputMessage);
            // Quiz shouldn't be called again
            Assert.Equal(1, called);
        }

        [Fact]
        public void SolarQuiz_Failure_DoesNotGiveKey()
        {
            var engine = new GameEngine();
            engine.ClearOutput();

            engine.State.MarkTalkedToLiora();
            engine.RunSolarQuiz = (q) => 3; // fail

            engine.Move("west");
            engine.Move("west");
            engine.Move("west");

            // Player should not be moved to the tent, and not receive the key
            Assert.Equal("DesertHub", engine.Player.CurrentRoom.ID);
            Assert.False(engine.Player.HasItem("Desert Key"));
        }

        [Fact]
        public void Map_ShowsTwoDistinctTentNodes()
        {
            var engine = new GameEngine();
            engine.ClearOutput();
            engine.PrintMap();
            Assert.Contains("TENT O", engine.OutputMessage);
            Assert.Contains("TENT I", engine.OutputMessage);
        }

        [Fact]
        public void MaintTent_Cannot_Go_East_Directly_To_DesertHub()
        {
            var engine = new GameEngine();
            engine.ClearOutput();

            engine.State.MarkTalkedToLiora();
            
            engine.RunSolarQuiz = (q) => 2; // pass quiz to get into tent
            engine.Move("west");
            engine.Move("west");
            engine.Move("west"); // into tent
            Assert.Equal("MaintTent", engine.Player.CurrentRoom.ID);

            engine.ClearOutput();
            engine.Move("east");
            Assert.Equal("MaintTent", engine.Player.CurrentRoom.ID);
            Assert.Contains("You can't go that way.", engine.OutputMessage);

            // Now go back out and then east from outside should go to DesertHub (symmetric)
            engine.Move("south");
            Assert.Equal("MaintTentOutside", engine.Player.CurrentRoom.ID);
            engine.Move("east");
            Assert.Equal("DesertHub", engine.Player.CurrentRoom.ID);
            // Confirm we did NOT skip to SolarDesert
            Assert.NotEqual("SolarDesert", engine.Player.CurrentRoom.ID);
        }
    }
}
