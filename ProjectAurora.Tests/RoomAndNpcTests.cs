using ProjectAurora.Data;
using ProjectAurora.Data.Rooms;
using ProjectAurora.Domain;
using ProjectAurora.Domain.NPCs;
using ProjectAurora.Data.Items;
using System.Linq;
using Xunit;

namespace ProjectAurora.Tests
{
    public class RoomAndNpcTests
    {
        [Fact]
        public void KeyRequirement_AllowsEntryWithKey()
        {
            var player = new Player(new Room("X", "X", "X"));
            var state = new GameState();
            var req = new KeyRequirement("Desert Key");
            Assert.False(req.CanEnter(player, state));
            player.AddItem(new KeyItem("Desert Key", "A desert key"));
            Assert.True(req.CanEnter(player, state));
        }

        [Fact]
        public void KeyRequirement_ConsumesKeyAndUnlocksPermanently()
        {
            var engine = new GameEngine();
            var player = engine.Player;
            var state = engine.State;
            
            // Create a room with consumable key requirement
            var keyReq = new KeyRequirement("Desert Key", consumeKey: true);
            var lockedRoom = new Room("LockedRoom", "Locked Room", "A locked room") { EntryRequirement = keyReq };
            
            // Player doesn't have key, cannot enter
            Assert.False(keyReq.CanEnter(player, state));
            
            // Give player the key
            player.AddItem(new KeyItem("Desert Key", "A desert key"));
            Assert.True(player.HasItem("Desert Key"));
            Assert.True(keyReq.CanEnter(player, state));
            
            // Simulate entering the room (OnEnter should consume the key)
            keyReq.OnEnter(player, state, engine);
            
            // Key should be removed from inventory
            Assert.False(player.HasItem("Desert Key"));
            
            // But door should stay unlocked
            Assert.True(keyReq.CanEnter(player, state));
            
            // Verify unlock message was printed
            Assert.Contains("unlocks the door", engine.OutputMessage);
        }

        [Fact]
        public void KeyRequirement_ReusableKeyNotConsumed()
        {
            var engine = new GameEngine();
            var player = engine.Player;
            var state = engine.State;
            
            // Create a room with reusable key requirement
            var keyReq = new KeyRequirement("Shed Key", consumeKey: false);
            var lockedRoom = new Room("LockedRoom", "Locked Room", "A locked room") { EntryRequirement = keyReq };
            
            // Give player the key
            player.AddItem(new KeyItem("Shed Key", "A shed key"));
            Assert.True(keyReq.CanEnter(player, state));
            
            // Simulate entering the room
            keyReq.OnEnter(player, state, engine);
            
            // Key should still be in inventory
            Assert.True(player.HasItem("Shed Key"));
            
            // Can still enter
            Assert.True(keyReq.CanEnter(player, state));
        }

        [Fact]
        public void DrLiora_Talks_ChangesState()
        {
            var player = new Player(new Room("X", "X", "X"));
            var engine = new GameEngine();
            var state = engine.State;
            var dr = new DrLiora();
            Assert.False(state.TalkedToLiora);
            dr.Talk(player, state, engine.Print, engine);
            Assert.True(state.TalkedToLiora);
        }

        [Fact]
        public void ControlRoom_UseBerries_TriggersQteAndCompletes()
        {
            var controlRoom = new ControlRoom("ControlRoom", "Control", "desc");
            var engine = new GameEngine();
            engine.RunHydroQTE = (q) => true; // auto-complete
            var player = new Player(controlRoom);
            var state = engine.State;
            // Give the player required items
            player.AddItem(new ConsumableItem("berries", ""));
            player.AddItem(new ConsumableItem("pinecone", ""));
            // Use berries
            var berries = player.Inventory.First(i => i.Name == "berries");
            Assert.False(state.QteComplete);
            var result = controlRoom.OnUseItem(berries, player, state, engine);
            Assert.True(state.QteComplete);
            // Berries and pinecone are consumed
            Assert.False(player.HasItem("berries"));
            Assert.False(player.HasItem("pinecone"));
        }

        [Fact]
        public void ControlRoom_UseLever_ConsumesLever()
        {
            var controlRoom = new ControlRoom("ControlRoom", "Control", "desc");
            var engine = new GameEngine();
            var player = new Player(controlRoom);
            var state = engine.State;
            // Give the player lever
            player.AddItem(new ProjectAurora.Data.Items.RepairItem("lever", ""));

            var lever = player.Inventory.First(i => i.Name == "lever");
            var handled = controlRoom.OnUseItem(lever, player, state, engine);
            Assert.True(state.LeverRepaired);
            Assert.True(handled);
            // Lever should be removed
            Assert.False(player.HasItem("lever"));
        }

        [Fact]
        public void TentsRoom_UseSnack_FeedsRaccoonAndGivesControlBoard()
        {
            var tents = new TentsRoom("Tents", "Tents", "desc");
            var engine = new GameEngine();
            var player = new Player(tents);
            var state = engine.State;
            // add snack
            player.AddItem(new ConsumableItem("snack", ""));

            var snack = player.Inventory.First(i => i.Name == "snack");
            var handled = tents.OnUseItem(snack, player, state, engine);
            Assert.True(state.FedRaccoon);
            Assert.True(player.HasItem("control board"));
            Assert.True(handled);
        }

        [Fact]
        public void Raccoon_Talk_BeforeFeeding_PrintsHint()
        {
            var tents = new TentsRoom("Tents", "Tents", "desc");
            var engine = new GameEngine();
            var player = new Player(tents);
            var state = engine.State;
            var raccoon = new ProjectAurora.Domain.NPCs.Raccoon();

            engine.ClearOutput();
            raccoon.Talk(player, state, engine.Print, engine);

            Assert.Contains("snack", engine.OutputMessage.ToLower());
        }

        [Fact]
        public void Raccoon_Talk_AfterFeeding_PrintsContent()
        {
            var tents = new TentsRoom("Tents", "Tents", "desc");
            var engine = new GameEngine();
            var player = new Player(tents);
            var state = engine.State;
            var raccoon = new ProjectAurora.Domain.NPCs.Raccoon();

            state.MarkFedRaccoon();
            engine.ClearOutput();
            raccoon.Talk(player, state, engine.Print, engine);

            Assert.DoesNotContain("snack", engine.OutputMessage.ToLower());
            Assert.Contains("content", engine.OutputMessage.ToLower());
        }

        [Fact]
        public void ProfKael_Talk_ConsumesPartsAndMarksStep1()
        {
            var prof = new ProjectAurora.Domain.NPCs.ProfKael();
            var engine = new GameEngine();
            var player = new Player(new Room("Office", "Office", "desc"));
            var state = engine.State;

            // Give parts
            player.AddItem(new ProjectAurora.Data.Items.RepairItem("anemometer", ""));
            player.AddItem(new ProjectAurora.Data.Items.RepairItem("control board", ""));
            player.AddItem(new ProjectAurora.Data.Items.RepairItem("power cables", ""));

            prof.Talk(player, state, engine.Print, engine);
            Assert.True(state.Step1Complete);
            Assert.False(player.HasItem("anemometer"));
            Assert.False(player.HasItem("control board"));
            Assert.False(player.HasItem("power cables"));
        }

        [Fact]
        public void SolarFieldsRoom_UseWaterHose_FixesSolarAndSetsState()
        {
            var solar = new SolarFieldsRoom("SolarFields", "Solar", "desc");
            var engine = new GameEngine();
            var player = new Player(solar);
            var state = engine.State;
            player.AddItem(new RepairItem("Water Hose", ""));

            var item = player.Inventory.First(i => i.Name == "Water Hose");
            var handled = solar.OnUseItem(item, player, state, engine);
            Assert.True(state.SolarFixed);
            Assert.True(handled);
            Assert.False(player.HasItem("Water Hose"));
        }

        [Fact]
        public void SolarFieldsRoom_UseRoboticParts_ConsumesParts()
        {
            var solar = new SolarFieldsRoom("SolarFields", "Solar", "desc");
            var engine = new GameEngine();
            var player = new Player(solar);
            var state = engine.State;
            player.AddItem(new ProjectAurora.Data.Items.RepairItem("Robotic Parts 1", ""));
            player.AddItem(new ProjectAurora.Data.Items.RepairItem("Robotic Parts 2", ""));

            var item = player.Inventory.First(i => i.Name == "Robotic Parts 1");
            var handled = solar.OnUseItem(item, player, state, engine);
            Assert.True(state.SolarFixed);
            Assert.True(handled);
            Assert.False(player.HasItem("Robotic Parts 1"));
            Assert.False(player.HasItem("Robotic Parts 2"));
        }

        // --- Geothermal Zone Tests ---

        [Fact]
        public void ChiefRodriguez_Talk_GivesPermitAndMarksState()
        {
            var chief = new ProjectAurora.Domain.NPCs.ChiefRodriguez();
            var engine = new GameEngine();
            var plantRoom = new ProjectAurora.Data.Rooms.PlantExteriorRoom("PlantExterior", "Plant", "desc");
            var player = new Player(plantRoom);
            var state = engine.State;

            Assert.False(state.TalkedToRodriguez);
            chief.Talk(player, state, engine.Print, engine);
            
            Assert.True(state.TalkedToRodriguez);
            Assert.Contains("permit", plantRoom.Items.Select(i => i.Name.ToLower()));
            Assert.Contains("50 megawatts", engine.OutputMessage);
        }

        [Fact]
        public void ChiefRodriguez_TalkAgain_GivesAdditionalInfo()
        {
            var chief = new ProjectAurora.Domain.NPCs.ChiefRodriguez();
            var engine = new GameEngine();
            var plantRoom = new ProjectAurora.Data.Rooms.PlantExteriorRoom("PlantExterior", "Plant", "desc");
            var player = new Player(plantRoom);
            var state = engine.State;

            state.MarkTalkedToRodriguez();
            engine.ClearOutput();
            chief.Talk(player, state, engine.Print, engine);
            
            Assert.Contains("complete cycle", engine.OutputMessage);
        }

        [Fact]
        public void SteamVentsRoom_TakeThermalData_MarksStateAndAddsToInventory()
        {
            var steamVents = new ProjectAurora.Data.Rooms.SteamVentsRoom("SteamVents", "Vents", "desc");
            var engine = new GameEngine();
            var player = new Player(steamVents);
            var state = engine.State;
            
            steamVents.AddItem(new Item("thermal data", "Thermal readings showing 180°C at 2km depth."));
            
            Assert.False(state.HasThermalData);
            var handled = steamVents.OnTakeItem("thermal data", player, state, engine);
            
            Assert.True(handled);
            Assert.True(state.HasThermalData);
            Assert.True(player.HasItem("thermal data"));
            Assert.Contains("180°C at 2km depth", engine.OutputMessage);
        }

        [Fact]
        public void SteamVentsRoom_TakeThermalData_SecondTime_PrintsAlreadyTakenMessage()
        {
            var steamVents = new ProjectAurora.Data.Rooms.SteamVentsRoom("SteamVents", "Vents", "desc");
            var engine = new GameEngine();
            var player = new Player(steamVents);
            var state = engine.State;
            
            steamVents.AddItem(new Item("thermal data", "Thermal readings showing 180°C at 2km depth."));
            
            // First take
            var handled1 = steamVents.OnTakeItem("thermal data", player, state, engine);
            Assert.True(handled1);
            Assert.True(player.HasItem("thermal data"));
            
            // Second take attempt
            engine.ClearOutput();
            var handled2 = steamVents.OnTakeItem("thermal data", player, state, engine);
            
            Assert.True(handled2);
            Assert.Contains("no thermal data to take", engine.OutputMessage.ToLower());
        }

        [Fact]
        public void DrElenaVoss_Talk_WithoutThermalData_AsksForData()
        {
            var elena = new ProjectAurora.Domain.NPCs.DrElenaVoss();
            var engine = new GameEngine();
            var player = new Player(new Room("Observatory", "Observatory", "desc"));
            var state = engine.State;

            elena.Talk(player, state, engine.Print, engine);
            
            Assert.Contains("Steam Vents", engine.OutputMessage);
            Assert.Contains("thermal data", engine.OutputMessage.ToLower());
        }

        [Fact]
        public void DrElenaVoss_Talk_WithThermalData_GivesPositiveResponse()
        {
            var elena = new ProjectAurora.Domain.NPCs.DrElenaVoss();
            var engine = new GameEngine();
            var player = new Player(new Room("Observatory", "Observatory", "desc"));
            var state = engine.State;
            
            player.AddItem(new Item("thermal data", ""));
            elena.Talk(player, state, engine.Print, engine);
            
            Assert.Contains("decades", engine.OutputMessage);
            Assert.Contains("sustainable", engine.OutputMessage.ToLower());
        }

        [Fact]
        public void ThermalObservatoryRoom_UsePermit_TriggersQuizAndGivesCertificate()
        {
            var observatory = new ProjectAurora.Data.Rooms.ThermalObservatoryRoom("Observatory", "Observatory", "desc");
            var engine = new GameEngine();
            var player = new Player(observatory);
            var state = engine.State;
            
            player.AddItem(new Item("permit", "A permit"));
            engine.RunQuiz = (q) => q.PassThreshold; // Pass the quiz
            
            Assert.False(state.GeothermalCertified);
            var permit = player.Inventory.First(i => i.Name == "permit");
            var handled = observatory.OnUseItem(permit, player, state, engine);
            
            Assert.True(handled);
            Assert.True(state.GeothermalCertified);
            Assert.True(player.HasItem("geothermal_certificate"));
            Assert.False(player.HasItem("permit"));
            Assert.Contains("Congratulations", engine.OutputMessage);
        }

        [Fact]
        public void ThermalObservatoryRoom_UsePermit_FailQuiz_DoesNotGiveCertificate()
        {
            var observatory = new ProjectAurora.Data.Rooms.ThermalObservatoryRoom("Observatory", "Observatory", "desc");
            var engine = new GameEngine();
            var player = new Player(observatory);
            var state = engine.State;
            
            player.AddItem(new Item("permit", "A permit"));
            engine.RunQuiz = (q) => 0; // Fail the quiz
            
            var permit = player.Inventory.First(i => i.Name == "permit");
            var handled = observatory.OnUseItem(permit, player, state, engine);
            
            Assert.True(handled);
            Assert.False(state.GeothermalCertified);
            Assert.False(player.HasItem("geothermal_certificate"));
            Assert.True(player.HasItem("permit"));
            Assert.Contains("Try again", engine.OutputMessage);
        }

        [Fact]
        public void ThermalObservatoryRoom_UsePermit_AlreadyCertified_PrintsMessage()
        {
            var observatory = new ProjectAurora.Data.Rooms.ThermalObservatoryRoom("Observatory", "Observatory", "desc");
            var engine = new GameEngine();
            var player = new Player(observatory);
            var state = engine.State;
            
            state.MarkGeothermalCertified();
            player.AddItem(new Item("permit", "A permit"));
            
            var permit = player.Inventory.First(i => i.Name == "permit");
            engine.ClearOutput();
            var handled = observatory.OnUseItem(permit, player, state, engine);
            
            Assert.True(handled);
            Assert.Contains("already completed", engine.OutputMessage);
        }

        [Fact]
        public void Kenji_Talk_SharesSpringPhilosophy()
        {
            var kenji = new ProjectAurora.Domain.NPCs.Kenji();
            var engine = new GameEngine();
            var player = new Player(new Room("HotSprings", "Springs", "desc"));
            var state = engine.State;

            kenji.Talk(player, state, engine.Print, engine);
            
            Assert.Contains("generations", engine.OutputMessage);
            Assert.Contains("balance", engine.OutputMessage.ToLower());
        }

        [Fact]
        public void James_Talk_ExplainsSeparatorOperation()
        {
            var james = new ProjectAurora.Domain.NPCs.James();
            var engine = new GameEngine();
            var player = new Player(new Room("Separator", "Separator", "desc"));
            var state = engine.State;

            james.Talk(player, state, engine.Print, engine);
            
            Assert.Contains("separator", engine.OutputMessage.ToLower());
            Assert.Contains("24/7", engine.OutputMessage);
        }

        [Fact]
        public void PlantExteriorRoom_OnEnter_FirstTime_AnnouncesChiefRodriguez()
        {
            var plant = new ProjectAurora.Data.Rooms.PlantExteriorRoom("PlantExterior", "Plant", "desc") 
            { 
                Occupant = new ProjectAurora.Domain.NPCs.ChiefRodriguez() 
            };
            var engine = new GameEngine();
            var player = new Player(plant);
            var state = engine.State;
            
            engine.ClearOutput();
            plant.OnEnter(player, state, engine);
            
            Assert.Contains("Rodriguez", engine.OutputMessage);
        }

        [Fact]
        public void PlantExteriorRoom_OnEnter_AfterTalking_NoAnnouncement()
        {
            var plant = new ProjectAurora.Data.Rooms.PlantExteriorRoom("PlantExterior", "Plant", "desc") 
            { 
                Occupant = new ProjectAurora.Domain.NPCs.ChiefRodriguez() 
            };
            var engine = new GameEngine();
            var player = new Player(plant);
            var state = engine.State;
            
            state.MarkTalkedToRodriguez();
            
            engine.ClearOutput();
            plant.OnEnter(player, state, engine);
            
            Assert.DoesNotContain("Rodriguez", engine.OutputMessage);
        }
    }
}
