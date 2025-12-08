using ProjectAurora.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectAurora.Domain
{
    public class GameEngine
    {
        public Player Player { get; private set; }
        public GameState State { get; private set; }
        public bool IsRunning { get; private set; }
        public string OutputMessage { get; private set; } = "";
        
        // Delegate for UI callbacks (like QTE or Quiz)
        public System.Func<ProjectAurora.Domain.IQte, bool>? RunHydroQTE { get; set; }
        public System.Func<ProjectAurora.Domain.IQuiz, int>? RunSolarQuiz { get; set; }
        
        private Room _hub;

        public GameEngine() : this(new WorldBuilder()) { }

        public GameEngine(IWorldBuilder builder)
        {
            _hub = builder.BuildWorld();
            Player = new Player(_hub);
            State = new GameState();
            IsRunning = true;
        }

        public void Print(string message)
        {
            if (string.IsNullOrEmpty(OutputMessage))
                OutputMessage = message;
            else
                OutputMessage += "\n" + message;
        }

        public void ClearOutput()
        {
            OutputMessage = string.Empty;
        }

        public void Move(string direction)
        {
            
            if (Player.CurrentRoom.TryGetExit(direction, out var nextRoom) && nextRoom != null)
            {
                
                if (!CanEnter(nextRoom))
                {
                    return;
                }

                // Capture the room before attempting entry events (so we know the original room)
                var previousRoom = Player.CurrentRoom;

                if (HandleEntryEvents(nextRoom))
                {

                    // If the handler didn't already change the CurrentRoom, set it to the next room
                    if (Player.CurrentRoom == previousRoom)
                    {
                        Player.MoveTo(nextRoom);
                    }

                    // If the room has actually changed, print a short enter message.
                    if (Player.CurrentRoom != previousRoom)
                    {
                        Print($"You move to {Player.CurrentRoom.Name}.");
                    }

                    // Always display the room description when the player ends up in a different room
                    Look();
                    CheckWinCondition();
                }
            }
            else
            {
                Print("You can't go that way.");
            }
        }

        private bool CanEnter(Room room)
        {
            if (room.EntryRequirement != null)
            {
                if (!room.EntryRequirement.CanEnter(Player, State))
                {
                    Print(room.EntryRequirement.DeniedMessage);
                    return false;
                }
            }
            return true;
        }

        private bool HandleEntryEvents(Room room)
        {
            // Delegate room-specific entry logic to the room itself
                if (!room.OnEnter(Player, State, this))
                {
                    return false;
                }
            // The per-room OnEnter now handles special cases like quizzes, item pickups, and box visits.
            
            // Call entry requirement's OnEnter to handle key consumption, etc.
            room.EntryRequirement?.OnEnter(Player, State, this);
            
            // Additional generic events are now handled by each Room's OnEnter

            return true;
        }

        public void Look()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Player.CurrentRoom.Description);
            
            if (Player.CurrentRoom.Items.Count > 0)
            {
                sb.Append("Items here: ");
                sb.AppendLine(string.Join(", ", Player.CurrentRoom.Items.Select(i => i.Name)));
            }
            
            // For exits, display the cardinal directions and note locked status where applicable
            sb.Append("Exits: ");
            sb.AppendLine(string.Join(", ", Player.CurrentRoom.Exits.Keys));

            // Show any locked/unlocked indications for exits that require a key
            foreach (var kv in Player.CurrentRoom.Exits)
            {
                var direction = kv.Key;
                var dest = kv.Value;
                if (dest.EntryRequirement is ProjectAurora.Domain.KeyRequirement keyReq)
                {
                    if (keyReq.IsUnlocked)
                    {
                        sb.AppendLine($"The path {direction} to {dest.Name} is unlocked.");
                    }
                    else
                    {
                        sb.AppendLine($"The path {direction} to {dest.Name} is locked.");
                    }
                }
            }

            Print(sb.ToString());
        }

        public void Inventory()
        {
            if (Player.Inventory.Count == 0)
            {
                Print("You are not carrying anything.");
            }
            else
            {
                Print("Inventory: " + string.Join(", ", Player.Inventory.Select(i => i.Name)));
            }
        }

        public void TakeItem(string itemName)
        {
            // Let the room handle special take behavior first
            if (Player.CurrentRoom.OnTakeItem(itemName, Player, State, this))
            {
                return;
            }

            var item = Player.CurrentRoom.Items.FirstOrDefault(i => i.Name.ToLower() == itemName.ToLower());

            if (item != null)
            {
                if (item.IsPickupable)
                {
                    Player.AddItem(item);
                    Player.CurrentRoom.RemoveItem(item);
                    Print($"You took the {item.Name}.");
                }
                else
                {
                    Print("You can't take that.");
                }
            }
            else
            {
                Print("I don't see that here.");
            }
        }

        public void UseItem(string itemName)
        {
            if (!Player.HasItem(itemName))
            {
                Print("You don't have that item.");
                return;
            }
            // Attempt to let the room handle item use first.
            var item = Player.Inventory.FirstOrDefault(i => i.Name.ToLower() == itemName.ToLower());
            if (item != null)
            {
                if (Player.CurrentRoom.OnUseItem(item, Player, State, this))
                {
                    return;
                }
            }

            // If we reach here, the item wasn't handled by the room and is not usable here
            Print("You can't use that here.");

            // If not handled by the room, we could place general-use-item logic here. Most special cases are now handled by rooms.
        }

        public void Talk()
        {
            var room = Player.CurrentRoom;
            if (room.Occupant != null)
            {
                room.Occupant.Talk(Player, State, Print, this);
            }
            else
            {
                Print("There is no one here to talk to.");
            }
        }

        public void ReturnToHub()
        {
            Player.MoveTo(_hub);
            Print("You have returned to the Aurora Control Hub.");
            CheckWinCondition();
        }

        public void Help()
        {
            var helpText = @"
================================================================================
                                PROJECT AURORA
                                 INSTRUCTIONS
================================================================================

OBJECTIVE:
  Restore power to the three regions: Solar Desert, Hydro Hub, and Windy Highlands.
  Return to the Aurora Control Hub once all systems are online.

COMMANDS:
    - Navigation:  north, south, east, west, up, down
  - Interaction: look, take [item], use [item], talk, inventory (or inv, i)
  - System:      help, exit (close window)

TIPS:
  - Pay attention to item descriptions.
  - Talk to NPCs for clues.
  - Some areas require specific items to enter.

Type 'map' to display the world map.
";
            Print(helpText);
            // The player can use the 'map' command to display the map manually.
            Print("================================================================================");
        }

        public void PrintMap()
        {
            // Map template with placeholders showing correct room connections
            string mapTemplate = @"
+-----------------------------------------------------------------------------+
|                           PROJECT AURORA MAP                                |
+-----------------------------------------------------------------------------+
|                                              [{LIBR}]                       |
|                                                 |                           |
|                                              [{RSRC}]-[{CAFE}]              |
|                                                 |                           |
|         [{HILL}]                                |          [{CTRL}]         |
|            |                                    |             |             |
|         [{TUND}]-----------------------------[{HHUB}]----[{DAM_}]           |
|                                                 |                           |
|[{TENT}]          [{FILD}]                      |                            |
|    |                 |                          |                           |
|    +-------------[{DHUB}]----[{SOLR}]-------[{HUB_}]                       |
|                      |                          |                           |
|         [{WATR}]-[{JUNK}]-[{SCP2}]              |                           |
|                      |                          |              [{SHED}]     |
|                   [{SCP1}]                   [{BORL}]             |         |
|                                                 |                 |         |
|                                              [{CABN}]----------[{GARD}]     |
|                                                                   |         |
|                                                                   |   [{COMP}]
|                                                                   |      |
|[{TNTS}]                                                        [{TURB}]--[{TOWR}]-[{OFFC}]
|   |                                                               |
|[{STRM}]-----------------------------------------------------------+
|                                                                   |
|                                                                [{MBOX}]
+-----------------------------------------------------------------------------+";

            // Map Room IDs to Placeholders
            var placeholders = new Dictionary<string, string>
            {
                { "Hub", "{HUB_}" },
                { "SolarDesert", "{SOLR}" },
                { "DesertHub", "{DHUB}" },
                { "MaintTent", "{TENT}" },
                { "SolarFields", "{FILD}" },
                { "Junkyard", "{JUNK}" },
                { "WaterSupplies", "{WATR}" },
                { "Scrap1", "{SCP1}" },
                { "Scrap2", "{SCP2}" },
                { "HydroHub", "{HHUB}" },
                { "DamPlant", "{DAM_}" },
                { "ControlRoom", "{CTRL}" },
                { "ResearchCenter", "{RSRC}" },
                { "TundraForest", "{TUND}" },
                { "Library", "{LIBR}" },
                { "Cafeteria", "{CAFE}" },
                { "TopOfHill", "{HILL}" },
                { "MountBoreal", "{BORL}" },
                { "Cabin", "{CABN}" },
                { "Garden", "{GARD}" },
                { "Shed", "{SHED}" },
                { "Turbines", "{TURB}" },
                { "Tower", "{TOWR}" },
                { "Office", "{OFFC}" },
                { "Stream", "{STRM}" },
                { "Tents", "{TNTS}" },
                { "MetalBox", "{MBOX}" },
                { "Computers", "{COMP}" }
            };

            // Define labels for each room ID (6 chars each)
            var labels = new Dictionary<string, string>
            {
                { "Hub", " HUB  " },
                { "SolarDesert", "SOLAR " },
                { "DesertHub", "D.HUB " },
                { "MaintTent", " TENT " },
                { "SolarFields", "FLD*  " },
                { "Junkyard", " JUNK " },
                { "WaterSupplies", "WATER " },
                { "Scrap1", "SCRP 1" },
                { "Scrap2", "SCRP 2" },
                { "HydroHub", "H.HUB " },
                { "DamPlant", " DAM  " },
                { "ControlRoom", "CNTRL " },
                { "ResearchCenter", "REARCH" },
                { "TundraForest", "TUNDRA" },
                { "Library", "LIBRY " },
                { "Cafeteria", " CAFE " },
                { "TopOfHill", " HILL " },
                { "MountBoreal", "BOREAL" },
                { "Cabin", "CABIN " },
                { "Garden", "GARDEN" },
                { "Shed", " SHED " },
                { "Turbines", "TURBIN" },
                { "Tower", "TOWER " },
                { "Office", "OFFICE" },
                { "Stream", "STREAM" },
                { "Tents", "TENTS " },
                { "MetalBox", "M.BOX " },
                { "Computers", "COMPRS" }
            };

            var sb = new StringBuilder(mapTemplate);
            string currentId = Player.CurrentRoom.ID;

            var uniquePlaceholders = placeholders.Values.Distinct();
            
            foreach (var ph in uniquePlaceholders)
            {
                // Find all rooms that map to this placeholder
                var rooms = placeholders.Where(p => p.Value == ph).Select(p => p.Key).ToList();
                
                // Check if any is current
                bool isCurrent = rooms.Contains(currentId);
                
                // Get label
                string label = labels[rooms.First()];
                
                if (isCurrent)
                {
                    sb.Replace(ph, $"__RED__{label}__RESET__");
                }
                else
                {
                    sb.Replace(ph, label);
                }
            }
            
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("  * Your current location is highlighted in RED");
            sb.AppendLine("  * Use cardinal directions (north, south, east, west).");
            sb.AppendLine("  * Rooms marked with * require talking to an NPC first.");
            Print(sb.ToString());
        }
        
        public void CheckWinCondition()
        {
            if (Player.CurrentRoom.ID == "Hub" && 
                State.SolarFixed && 
                State.QteComplete && 
                State.WindyRestored)
            {
                Print("CONGRATULATIONS! You have restored power to all three regions. Project Aurora is a success!");
                IsRunning = false;
            }
        }
    }
}
