using ProjectAurora.Data;
using ProjectAurora.Data.Items;
using ProjectAurora.Data.Rooms;
using ProjectAurora.Domain;
using ProjectAurora.Domain.NPCs;
using System.Linq;
using Xunit;

namespace ProjectAurora.Tests
{
    public class GameCompletionTests
    {
        [Fact]
        public void FullGameWalkthrough_CompletesGame()
        {
            // Setup
            var engine = new GameEngine();
            var player = engine.Player;
            var state = engine.State;

            // Mock QTE and Quiz
            engine.RunHydroQTE = (qte) => true;
            engine.RunQuiz = (quiz) => quiz.PassThreshold;

            // --- Solar Region ---
            engine.Move("west"); // Solar Desert
            engine.Move("west"); // Desert Hub
            engine.Talk(); // Talk to Dr. Liora
            
            // The quiz is handled by the UI in the real game, but here we need to simulate the state change
            // if the Move command doesn't trigger it. 
            // Actually, let's see how the quiz is triggered.
            // In WorldBuilder: desertHub.AddExit("west", maintTent);
            // In GameEngine.Move: if (HandleEntryEvents(nextRoom)) ...
            // In MaintTentRoom.OnEnter: triggers quiz.
            
            engine.Move("west"); // Maintenance Tent (triggers quiz)
            Assert.Equal("MaintTent", player.CurrentRoom.ID);
            engine.TakeItem("Desert Key");
            Assert.True(player.HasItem("Desert Key"));

            engine.Move("east"); // Back to Desert Hub
            engine.Move("south"); // Junkyard (consumes Desert Key)
            Assert.Equal("Junkyard", player.CurrentRoom.ID);
            Assert.False(player.HasItem("Desert Key"));

            engine.Move("west"); // Water Supplies
            engine.TakeItem("Water Hose");
            Assert.True(player.HasItem("Water Hose"));

            engine.Move("east"); // Back to Junkyard
            engine.Move("north"); // Back to Desert Hub
            engine.Move("north"); // Solar Fields
            engine.UseItem("Water Hose");
            Assert.True(state.SolarFixed);

            engine.Move("south"); // Back to Desert Hub
            engine.Move("east"); // Back to Solar Desert
            engine.Move("east"); // Back to Hub

            // --- Hydro Region ---
            engine.Move("north"); // Hydro Hub
            engine.Move("north"); // Research Center
            engine.Move("east"); // Cafeteria
            engine.TakeItem("key");
            Assert.True(player.HasItem("key"));

            engine.Move("west"); // Back to Research Center
            engine.Move("south"); // Back to Hydro Hub
            engine.Move("west"); // Tundra Forest
            engine.TakeItem("berries");
            engine.TakeItem("pinecone");
            Assert.True(player.HasItem("berries"));
            Assert.True(player.HasItem("pinecone"));

            engine.Move("north"); // Top of Hill
            engine.TakeItem("lever");
            Assert.True(player.HasItem("lever"));

            engine.Move("south"); // Back to Tundra Forest
            engine.Move("east"); // Back to Hydro Hub
            engine.Move("east"); // Dam Plant
            engine.Move("north"); // Control Room (requires key)
            Assert.Equal("ControlRoom", player.CurrentRoom.ID);

            engine.UseItem("lever");
            engine.UseItem("berries");
            Assert.True(state.QteComplete);

            engine.Move("south"); // Back to Dam Plant
            engine.Move("west"); // Back to Hydro Hub
            engine.Move("south"); // Back to Hub

            // --- Windy Region ---
            engine.Move("south"); // Mount Boreal
            engine.Move("south"); // Cabin
            engine.TakeItem("snack");
            Assert.True(player.HasItem("snack"));

            engine.Move("east"); // Garden
            engine.Move("south"); // Turbines
            engine.Move("east"); // Tower
            engine.Move("east"); // Office
            engine.Talk(); // Talk to Prof. Kael, get Shed Key
            engine.TakeItem("Shed Key");
            Assert.True(player.HasItem("Shed Key"));

            engine.Move("west"); // Back to Tower
            engine.Move("west"); // Back to Turbines
            engine.Move("north"); // Back to Garden
            engine.Move("north"); // Shed (requires Shed Key)
            Assert.Equal("Shed", player.CurrentRoom.ID);
            engine.TakeItem("power cables");
            Assert.True(player.HasItem("power cables"));

            engine.Move("south"); // Back to Garden
            // Visit Metal Box to make code appear in Garden
            engine.Move("south"); // Turbines
            engine.Move("south"); // Metal Box
            Assert.True(state.BoxVisited);

            engine.Move("north"); // Back to Turbines
            engine.Move("north"); // Back to Garden
            engine.TakeItem("code");
            Assert.True(player.HasItem("code"));

            engine.Move("south"); // Back to Turbines
            engine.Move("west"); // Stream
            engine.Move("north"); // Tents
            engine.UseItem("snack"); // Feed raccoon
            engine.TakeItem("control board");
            Assert.True(player.HasItem("control board"));

            engine.Move("south"); // Back to Stream
            engine.Move("east"); // Back to Turbines
            engine.Move("south"); // Metal Box
            // We need to simulate the code entry. 
            engine.ReadInput = () => "Rigby";
            engine.UseItem("code");
            engine.TakeItem("anemometer");
            Assert.True(player.HasItem("anemometer"));

            engine.Move("north"); // Back to Turbines
            engine.Move("east"); // Tower
            engine.Move("east"); // Office
            engine.Talk(); // Talk to Prof. Kael, Step 1 Complete
            Assert.True(state.Step1Complete);

            engine.Move("west"); // Back to Tower
            engine.Move("north"); // Computers
            engine.Talk(); // Step 2 Complete
            Assert.True(state.Step2Complete);

            engine.Move("south"); // Back to Tower
            engine.Move("west"); // Turbines
            engine.Talk(); // Windy Restored, returns to Hub
            Assert.True(state.WindyRestored);
            Assert.Equal("Hub", player.CurrentRoom.ID);

            // --- Geothermal Region ---
            engine.Move("east"); // Hot Springs
            engine.Move("east"); // Plant Exterior
            engine.Talk(); // Talk to Chief Rodriguez, get permit
            engine.TakeItem("permit");
            Assert.True(player.HasItem("permit"));

            engine.Move("north"); // Steam Vents
            engine.TakeItem("thermal data");
            Assert.True(player.HasItem("thermal data"));

            engine.Move("south"); // Back to Plant Exterior
            engine.Move("west"); // Back to Hot Springs
            engine.Move("south"); // Observatory
            engine.UseItem("thermal data");
            Assert.True(state.ThermalDataSubmitted);
            engine.UseItem("permit"); // Triggers quiz
            Assert.True(state.GeothermalCertified);

            engine.Move("north"); // Back to Hot Springs
            engine.Move("west"); // Back to Hub

            // --- Final Check ---
            Assert.True(state.SolarFixed);
            Assert.True(state.QteComplete);
            Assert.True(state.WindyRestored);
            Assert.True(state.GeothermalCertified);
            Assert.Equal("Hub", player.CurrentRoom.ID);
            
            engine.CheckWinCondition();
            Assert.False(engine.IsRunning);
            Assert.Contains("CONGRATULATIONS", engine.OutputMessage);
        }
    }
}
