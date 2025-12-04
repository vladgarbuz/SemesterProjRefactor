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
        public void AutoPickupRoom_AddsItemOnEnter()
        {
            var room = new AutoPickupRoom("TestRoom", "Test", "Contains item", "Water Hose");
            room.AddItem(new RepairItem("Water Hose", "Hose"));
            var player = new Player(room);
            var engine = new GameEngine();
            engine.ClearOutput();
            var state = new GameState();
            // OnEnter should add the item to player inventory
            room.OnEnter(player, state, engine);
            Assert.True(player.HasItem("Water Hose"));
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
        }
    }
}
