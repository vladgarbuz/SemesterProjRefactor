using System.Collections.Generic;
using ProjectAurora.Data;

namespace ProjectAurora.Domain
{
    public class WorldBuilder : IWorldBuilder
    {
        private readonly List<Room> _rooms;
        public IReadOnlyList<Room> Rooms => _rooms.AsReadOnly();
        public Room? StartingRoom { get; private set; }

        public WorldBuilder()
        {
            _rooms = new List<Room>();
        }

        public ProjectAurora.Data.Room BuildWorld()
        {
            // --- Create Rooms ---
            
            // Hub
            var hub = new Room("Hub", "Aurora Control Hub", "You are in the Aurora Control Hub, the heart of the last renewable energy initiative. The air hums with faint backup power. Screens flicker, showing maps of four darkened regions. A workbench lies in the corner with scattered tools.");

            // Solar Region
            var solarDesert = new Room("SolarDesert", "Solar Desert", "After walking for hours you find yourself in a desolate land. The desert stretches before you. Towers of sand cover the solar field. Heat shimmers across the horizon. You find a small hub that looks like it could have life(west)");
            var desertHub = new Room("DesertHub", "Desert Hub", "You notice a map in front of the hub with the areas in the desert: Maintenance tent (west), Aurora Hub (east), Solar panel field (north), Junkyard (south). You decide to enter and there you find Dr. Liora Sunvale. She welcomes you and is ready to answer your questions. (talk)") { Occupant = new ProjectAurora.Domain.NPCs.DrLiora() };
            var maintTent = new ProjectAurora.Data.Rooms.MaintTentRoom("MaintTent", "Maintenance Tent", "You are inside the maintenance tent. Various tools are scattered about. In the corner sits a wooden box labeled 'Junkyard'.") { EntryRequirement = new TalkedToRequirement("Dr. Liora") };
            var solarFields = new ProjectAurora.Data.Rooms.SolarFieldsRoom("SolarFields", "Solar Panel Fields", "You find yourself in the Solar Panel Fields and notice a lot of piles of sand. You try to dig into one and you find a solar panel. There are thousands of them. How will you clean up the piles: (1) Water Hose (unreliable) (2) Robotic maintenece") { EntryRequirement = new TalkedToRequirement("Dr. Liora") };
            var junkyard = new Room("Junkyard", "Junkyard", "You use the key to enter the Junkyard and there you find 3 exits labeled: Water supplies (west), Scrapyard 1 (south), Scrapyard 2 (east)") { EntryRequirement = new KeyRequirement("Desert Key") };
            var waterSupplies = new ProjectAurora.Data.Rooms.AutoPickupRoom("WaterSupplies", "Water Supplies", "Upon entering the water supplies storage, you see a huge pile of materials. While searching, you find a long water hose with a portable tank and take it. (+Water Hose)", "Water Hose");
            var scrap1 = new ProjectAurora.Data.Rooms.AutoPickupRoom("Scrap1", "Scrapyard 1", "Upon entering the scrapyard you find a huge pile of scraps. While searching you discover robotic parts and take them. (+Robotic Parts)", "Robotic Parts 1");
            var scrap2 = new ProjectAurora.Data.Rooms.AutoPickupRoom("Scrap2", "Scrapyard 2", "Upon entering the scrapyard you find a huge pile of scraps. While searching you discover robotic parts and take them. (+Robotic Parts)", "Robotic Parts 2");

            // Hydro Region
            var hydroHub = new Room("HydroHub", "Hydro Hub", "You start walking toward the river and arrive at a junction. A sign reads: ==Welcome to the Hydro Hub== Aurora Hub (south), Research Center (north), Hydroelectric Dam (east), Tundra Forest (west)");
            var damPlant = new Room("DamPlant", "The Dam Plant", "After a short stroll you arrive at the riverside with no bridge leading across. A large Hydroelectric Plant sits here; an entrance leads to the Control Room (north), but the door is locked.");
            var researchCenter = new Room("ResearchCenter", "Research Center", "You step into the lobby of an Aurora outpost. This building supports engineers on their missions and has two main sections: a Library (up) and the Cafeteria (right). The way back to the Hydro Hub is south.");
            var tundraForest = new Room("TundraForest", "Tundra Forest", "You have entered the Tundra forrest. There are giant trees as far as you can see. You see some items you that you can move, a few pinecones on the ground(take pinecone) and some berries on a nearby bush(take berries), you can try to wander around but it might get you lost");
            var library = new Room("Library", "Library", "Loads of heavy shelves hold thousands of technical theory, documents and old logbooks. A single chair is occupied by a person reading one of the books, you can approach ther (talk). The lobby is downstrairs(down)") { Occupant = new ProjectAurora.Domain.NPCs.DrAmara() };
            var cafeteria = new Room("Cafeteria", "Cafeteria", "A regular cafeteria. Near the serving station, you see a small, out of place item lying among the cutlery. It looks like a key, dou you take it?(take key) It might come in handy later... The lobby is to the Left(left).");
            var topOfHill = new Room("TopOfHill", "Top of the Hill", "After climbing up the hill you find a forgotten toolbox. You see a box labeled levers, take one?(take lever). The only way down is back to the Tundra (south).");
            var controlRoom = new ProjectAurora.Data.Rooms.ControlRoom("ControlRoom", "Control Room", "You walk deep into the dam to the Control Room. Directly ahead is the emergency restart control panel with the restart lever marked, however the lever is completely rusted and jammed shut. You need to derust or replace it if you ever want to restart the plant. The only way back is south to the Dam Plant.") { EntryRequirement = new KeyRequirement("key") };

            // Windy Region
            var mountBoreal = new Room("MountBoreal", "Mount Boreal", "You're standing atop the peak of Mount Boreal. To the south is a ridge path leading to an abandoned cabin.");
            var cabin = new Room("Cabin", "Cabin", "You've entered an old, abandoned cabin once used by maintenance crews. You can see old papers and spare parts scattered on the ground. To the east is a door which seems to lead to the garden. In a corner you can see a snack bar with wild berries, it could be useful. (take snack)");
            var garden = new ProjectAurora.Data.Rooms.GardenRoom("Garden", "Garden", "You're standing in the garden, now overgrown with weeds and bushes. You can feel the cold wind on your face. To the north is an old, half-broken shed. To the south you can see the turbines turning faintly in the distance.");
            var shed = new Room("Shed", "Shed", "You are standing in the old shed. You can see a note on a desk nearby. (take note) You can see a bundle of cables under some boxes, they seem pretty sturdy. (take power cables)") { EntryRequirement = new KeyRequirement("Shed Key") };
            var turbines = new Room("Turbines", "Turbines", "You are standing between the wind turbines. Some are turned off while others spin slowly. To the east is a control tower connected to the turbines; to the west you can hear a stream of water; to the south is a locked metal box that may contain something useful.") { Occupant = new ProjectAurora.Domain.NPCs.TurbinesOperator() };
            var tower = new ProjectAurora.Data.Rooms.TowerRoom("Tower", "Tower", "You've entered the control tower. You can hear a faint static sound in the background. To the north are some old computers faintly flickering. To the east is an office.");
            var office = new Room("Office", "Office", "You've entered what seems to be an administration office. You can see blueprints and written entries scattered across the floor. You can hear rustling from behind a bookshelf, maybe you should see who it is (talk).") { Occupant = new ProjectAurora.Domain.NPCs.ProfKael() };
            var stream = new Room("Stream", "Stream", "You are standing next to a stream of water. To the north you can see an abandoned bonfire with a few tents nearby.");
            var tents = new ProjectAurora.Data.Rooms.TentsRoom("Tents", "Tents", "You can see something moving in one of the tents. Mybe you should see what it is. (talk)") { Occupant = new ProjectAurora.Domain.NPCs.Raccoon() };
            var metalBox = new Room("MetalBox", "Metal Box", "You're standing in front of the locked box. It seems to require a code to open.");
            var computers = new Room("Computers", "Computers", "You see old computers faintly flickering.") { Occupant = new ProjectAurora.Domain.NPCs.ComputersTerminal() };

            // --- Connect Rooms ---

            // Hub Connections
            hub.AddExit("north", hydroHub);
            hub.AddExit("west", solarDesert);
            hub.AddExit("south", mountBoreal);

            // Solar Connections
            solarDesert.AddExit("west", desertHub);
            solarDesert.AddExit("east", hub); // Back to hub
            // desertHub -> solarDesert is already east; solarDesert -> desertHub is west (symmetric)

            desertHub.AddExit("east", solarDesert);
            desertHub.AddExit("west", maintTent); // Quiz handled on entry
            desertHub.AddExit("north", solarFields); // Locked initially
            desertHub.AddExit("south", junkyard); // Locked by key

            maintTent.AddExit("east", desertHub);

            junkyard.AddExit("north", desertHub);
            junkyard.AddExit("west", waterSupplies);
            junkyard.AddExit("south", scrap1);
            junkyard.AddExit("east", scrap2);

            waterSupplies.AddExit("east", junkyard);
            scrap1.AddExit("north", junkyard);
            scrap2.AddExit("west", junkyard);

            // Hydro Connections
            hydroHub.AddExit("south", hub);
            hydroHub.AddExit("east", damPlant);
            hydroHub.AddExit("north", researchCenter);
            hydroHub.AddExit("west", tundraForest);

            researchCenter.AddExit("south", hydroHub);
            researchCenter.AddExit("north", library); // "up" in text, but standardizing on cardinal/relative
            researchCenter.AddExit("up", library);
            researchCenter.AddExit("east", cafeteria);
            researchCenter.AddExit("right", cafeteria);

            library.AddExit("south", researchCenter);
            library.AddExit("down", researchCenter);

            cafeteria.AddExit("west", researchCenter);
            cafeteria.AddExit("left", researchCenter);

            tundraForest.AddExit("east", hydroHub);
            tundraForest.AddExit("north", topOfHill);

            topOfHill.AddExit("south", tundraForest);

            damPlant.AddExit("west", hydroHub);
            // Replace use of 'enter' phrasing with a cardinal direction (north)
            damPlant.AddExit("north", controlRoom); // Locked by key

            // Replace 'exit to exterior' phrasing with the opposing cardinal (south)
            controlRoom.AddExit("south", damPlant);

            // Windy Connections
            mountBoreal.AddExit("south", cabin);
            mountBoreal.AddExit("north", hub); // Back to hub

            cabin.AddExit("north", mountBoreal);
            cabin.AddExit("east", garden);

            garden.AddExit("west", cabin);
            garden.AddExit("north", shed);
            garden.AddExit("south", turbines);

            shed.AddExit("south", garden);

            turbines.AddExit("north", garden);
            turbines.AddExit("east", tower);
            turbines.AddExit("south", metalBox);
            turbines.AddExit("west", stream);

            stream.AddExit("east", turbines);
            stream.AddExit("north", tents);

            tents.AddExit("south", stream);

            metalBox.AddExit("north", turbines);

            tower.AddExit("west", turbines);
            tower.AddExit("north", computers);
            tower.AddExit("east", office);

            office.AddExit("west", tower);
            
            computers.AddExit("south", tower);

            // --- Add Items ---
            // Solar
            // Desert Key in MaintTent (visible after quiz completion)
            maintTent.AddItem(new ProjectAurora.Data.Items.KeyItem("Desert Key", "Key to the Junkyard. Found in a wooden box."));
            // Water Hose in WaterSupplies
            waterSupplies.AddItem(new ProjectAurora.Data.Items.RepairItem("Water Hose", "A long water hose with a portable water tank."));
            // Robotic Parts
            scrap1.AddItem(new ProjectAurora.Data.Items.RepairItem("Robotic Parts 1", "Scrap robotic parts."));
            scrap2.AddItem(new ProjectAurora.Data.Items.RepairItem("Robotic Parts 2", "Scrap robotic parts."));

            // Hydro
            // Dam Key in Cafeteria
            cafeteria.AddItem(new ProjectAurora.Data.Items.KeyItem("key", "A small metal tool with the Aurora symbol on it."));
            // Berries, Pinecone in Tundra
            tundraForest.AddItem(new ProjectAurora.Data.Items.ConsumableItem("berries", "A cluster of edible-looking berries."));
            tundraForest.AddItem(new ProjectAurora.Data.Items.ConsumableItem("pinecone", "A large pinecone."));
            // Lever in TopOfHill
            topOfHill.AddItem(new ProjectAurora.Data.Items.RepairItem("lever", "A heavy, stainless steel lever."));

            // Windy
            // Snack in Cabin
            cabin.AddItem(new ProjectAurora.Data.Items.ConsumableItem("snack", "A small packaged snack."));
            // Note, Power Cables in Shed
            shed.AddItem(new Item("note", "It reads: 'One of the turbine parts was lost near the stream... I saw something furry running off with it.'"));
            shed.AddItem(new ProjectAurora.Data.Items.RepairItem("power cables", "A solid set of insulated power cables."));
            // Code in Garden (conditional, but we can add it and hide it or just add it)
            // Logic says "Only appears if box flag is true". We'll handle visibility in GameEngine or just add it and check flag on take.
            // For simplicity, we add it but maybe make it un-takeable until flag? Or just add it when flag is set.
            // Let's add it to the room but we might filter it out in description if not visible.
            // Actually, let's NOT add it here, and add it dynamically in the engine when the flag is set.
            
            // Control Board (Raccoon/Stream) - Logic says Raccoon has it.
            // Anemometer in Metal Box - Locked.
            // Flimsy Cables in Tower - Visible if Shed Key in inventory.
            
            // Shed Key - Given by Kael.

            StartingRoom = hub;
            
            // Add to list
            _rooms.Add(hub);
            _rooms.Add(solarDesert);
            _rooms.Add(desertHub);
            _rooms.Add(maintTent);
            _rooms.Add(solarFields);
            _rooms.Add(junkyard);
            _rooms.Add(waterSupplies);
            _rooms.Add(scrap1);
            _rooms.Add(scrap2);
            _rooms.Add(hydroHub);
            _rooms.Add(damPlant);
            _rooms.Add(researchCenter);
            _rooms.Add(tundraForest);
            _rooms.Add(library);
            _rooms.Add(cafeteria);
            _rooms.Add(topOfHill);
            _rooms.Add(controlRoom);
            _rooms.Add(mountBoreal);
            _rooms.Add(cabin);
            _rooms.Add(garden);
            _rooms.Add(shed);
            _rooms.Add(turbines);
            _rooms.Add(tower);
            _rooms.Add(office);
            _rooms.Add(stream);
            _rooms.Add(tents);
            _rooms.Add(metalBox);
            _rooms.Add(computers);

            return StartingRoom!;
        }
    }
}
