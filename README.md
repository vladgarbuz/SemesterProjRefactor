# Project Aurora - Console Text Adventure

[![.NET CI](https://github.com/<YOUR_USERNAME>/<YOUR_REPOSITORY>/actions/workflows/dotnet.yml/badge.svg)](https://github.com/<YOUR_USERNAME>/<YOUR_REPOSITORY>/actions/workflows/dotnet.yml)

An open-source, console-based text-adventure where you play an engineer trying to restore power across three regions: Solar Desert, Hydro Hub, and Windy Highlands.

## 1. Overview
Project Aurora is a text-based adventure game where the player acts as an engineer in the year 2075. The goal is to restore power to three regions: Solar Desert, Hydro Hub, and Windy Highlands. The game runs in a console environment and uses a natural language parser for user input.

## 2. Global Mechanics

### 2.1 Input Parser
*   **Format**: The game accepts one or two-word commands (e.g., "north", "take key").
*   **Case Sensitivity**: All inputs are converted to lowercase/handled case-insensitively.
*   **Invalid Commands**: If a command is not recognized, the game prints "I don't know that command."

### 2.2 Inventory System
The game uses a list-based inventory system to track items the player has collected.
*   **Taking Items**: `take [item name]` adds the item to the player's inventory and removes it from the room.
*   **Checking Inventory**: `inventory` lists all items currently held by the player.
*   **Using Items**: `use [item name]` triggers specific events if the player is in the correct room and has the item in their inventory.

### 2.3 Navigation
*   **Movement**: `north`, `south`, `east`, `west`.
    * When you move using a navigation command, the game now automatically prints the room description.
*   **Look**: `look` still prints the current room's long description (including exits and items) on demand.
*   **Note**: The `back` command has been removed; move using cardinal or relative directions instead.

### 2.4 NPC State Logic
*   **Cyclic Dialogue**: NPCs repeat their primary hint or instruction until a specific game state changes (e.g., an item is received or a task is completed).
*   **State-Dependent Dialogue**: After a quest is completed (e.g., `talkedToLiora = true`), the NPC's dialogue changes to reflect progress (e.g., "Good luck with the panels!").

### 2.5 Game Loop & UI Specifics
*   **Screen Clearing**: The console clears (`Console.Clear()`) upon moving to a new room to keep the interface clean.
*   **Color Coding**:
    *   **Room Names**: Magenta
    *   **Items**: Cyan
    *   **NPCs**: Green
    *   **Exits**: Yellow
    *   **Error Messages**: Red

### 2.6 Win/Loss State Definitions
*   **Victory**: The game ends when all three regions are restored and the player returns to the Aurora Control Hub.
*   **Game Over / Setbacks**:
    *   **QTE Failure**: Failing the Hydro QTE results in a 10-second timeout before retrying. There is no permanent "Game Over" screen.
    *   **Quiz Failure**: Answering incorrectly in the Solar Desert moves the player back to the previous room, requiring them to try again.

## 3. Region Specifications

### 3.0 World Map
```mermaid
graph TD
    Hub[Aurora Control Hub] -->|North| Hydro[Hydro Hub]
    Hub -->|West| Solar[Solar Desert]
    Hub -->|South| Windy[Mount Boreal]
    
    subgraph Solar Region
    Solar --> DesertHub
    DesertHub --> Tent[Maintenance Tent]
    DesertHub --> Fields[Solar Panel Fields]
    DesertHub --> Junk[Junkyard]
    Junk --> Water[Water Supplies]
    Junk --> Scrap1[Scrapyard 1]
    Junk --> Scrap2[Scrapyard 2]
    end

    subgraph Hydro Region
    Hydro --> Dam[The Dam Plant]
    Hydro --> Research[Research Center]
    Hydro --> Forest[Tundra Forest]
    Research --> Lib[Library]
    Research --> Cafe[Cafeteria]
    Forest --> Hill[Top of Hill]
    Dam --> Control[Control Room]
    end

    subgraph Windy Region
    Windy --> Cabin
    Cabin --> Garden
    Garden --> Shed
    Garden --> Turbines
    Turbines --> Tower
    Turbines --> MetalBox
    Turbines --> Stream
    Stream --> Tents
    Tower --> Office
    Tower --> Computers
    end
```

---

## Getting Started

Prerequisites:
- .NET 9.0 SDK (or later)

Build and run:

```powershell
dotnet build
dotnet run --project ProjectAurora/ProjectAurora.csproj
```

Run unit tests:

```powershell
dotnet test
```

## License

Project Aurora is available under the MIT License. See the `LICENSE` file in the root of this repository.

---

## Map Characters & Unicode

The in-game map uses Unicode box-drawing characters for the best visual appearance in modern terminals. If you notice the map rendering incorrectly, try the following:

- Use a Unicode-capable font (e.g., Consolas, Fira Code, or DejaVu Sans Mono).
- Make sure the terminal encoding is set to UTF-8.
- If you prefer plain ASCII, update the `mapTemplate` in `ProjectAurora/Domain/GameEngine.cs` with an ASCII version.


### 3.1 Hub: Aurora Control Hub
*   **Description**: The starting point.
*   **Exits**:
    *   `north` -> Hydro Hub
    *   `west` -> Solar Desert
    *   `south` -> Mount Boreal (Windy Highlands)

---

### 3.2 Region 1: Solar Desert (West)

#### Room Map
1.  **Solar Desert** (Entry)
    *   `west` -> Desert Hub
2.  **Desert Hub**
    *   `east` -> Solar Desert
    *   `west` -> Maintenance Tent (Only after talking to NPC)
    *   `north` -> Solar Panel Fields (Only after talking to NPC)
    *   `south` -> Junkyard (Requires **Desert Key**)
    *   **NPC**: Dr. Liora Sunvale.
        *   Action: `talk` sets `talkedToLiora = true`, unlocking West and North exits.
3.  **Maintenance Tent**
    *   `east` -> Desert Hub
    *   **Event**: Upon entry, a quiz is triggered (if not already completed).
        *   Question: "What happens if solar panels overheat?"
        *   Options: (1) More energy, (2) Less efficiency, (3) Catch fire.
        *   Input `2`: Success. Player enters the tent.
        *   Input `1` or `3`: Failure. Entry denied, player remains in Desert Hub.
    *   **Item**: **Desert Key** - Must be taken manually using `take Desert Key`.
4.  **Junkyard**
    *   `north` -> Desert Hub
    *   `west` -> Water Supplies
    *   `south` -> Scrapyard 1
    *   `east` -> Scrapyard 2
    *   **Entry Requirement**: Requires **Desert Key**. The key is consumed on first entry and the door remains unlocked permanently.
5.  **Water Supplies**
    *   `east` -> Junkyard
    *   **Item**: **Water Hose** - Visible in the room, must be taken manually using `take Water Hose`.
6.  **Scrapyard 1**
    *   `north` -> Junkyard
    *   **Item**: **Robotic Parts 1** - Visible in the room, must be taken manually.
7.  **Scrapyard 2**
    *   `west` -> Junkyard
    *   **Item**: **Robotic Parts 2** - Visible in the room, must be taken manually.
    *   **Item**: **Robotic Parts 2** - Visible in the room, must be taken manually.
8.  **Solar Panel Fields**
    *   **Event**: Upon entry, the player must choose a repair method.
        *   Input `1` (Water Hose):
            *   Condition: Requires **Water Hose** in inventory.
            *   Outcome: "Temporary fix". Game prints success message. Player returned to **Aurora Control Hub**.
        *   Input `2` (Robotic Maintenance):
            *   Condition: Requires **Robotic Parts 1** AND **Robotic Parts 2** in inventory.
            *   Outcome: "Saved the Solar Desert". Game prints success message. Player returned to **Aurora Control Hub**.

---

### 3.3 Region 2: Hydro Hub (North)

#### Room Map
1.  **Hydro Hub** (Entry)
    *   `south` -> Aurora Control Hub
    *   `east` -> The Dam Plant
    *   `north` -> Research Center
    *   `west` -> Tundra Forest
2.  **Research Center**
    *   `south` -> Hydro Hub
    *   `north` -> Library
    *   `east` -> Cafeteria
3.  **Library**
    *   `south` -> Research Center
    *   **NPC**: Dr. Amara Riversong. Dialog provides hints about the acid (berries + pinecone).
4.  **Cafeteria**
    *   `west` -> Research Center
    *   **Item**: `key` (Dam Key).
        *   Action: `take key` -> Adds 'key' to inventory.
5.  **Tundra Forest**
    *   `east` -> Hydro Hub
    *   `north` -> Top of the Hill
    *   **Description**: "You have entered the Tundra Forest. Towering trees extend in every direction. On the ground you notice a few pinecones and a nearby bush with ripe berries. Be careful wandering — it's easy to get lost here."
    *   **Item**: `berries`.
        *   Action: `take berries` -> Adds 'berries' to inventory.
    *   **Item**: `pinecone`.
        *   Action: `take pinecone` -> Adds 'pinecone' to inventory.
6.  **Top of the Hill**
    *   `south` -> Tundra Forest
    *   **Item**: `lever`.
        *   Action: `take lever` -> Adds 'lever' to inventory.
7.  **The Dam Plant**
    *   `west` -> Hydro Hub
    *   `north` -> Control Room
        *   **Condition**: Requires **Dam Key** in inventory. If false, prints "Entrance sealed".
8.  **Control Room**
    *   `south` -> The Dam Plant
    *   **Interaction**:
        *   `use lever`:
            *   Condition: Requires **lever** in inventory.
            *   Outcome: Sets `leverRepaired = true`. Prints success message.
        *   `use berries` OR `use pinecone`:
            *   Condition: Requires **berries** AND **pinecone** in inventory.
            *   Outcome: Triggers **QTE Minigame**.

#### QTE Minigame (Hydro)
*   **Trigger**: Using berries/pinecone in Control Room without repairing the lever.
*   **Mechanic**:
    *   The game displays a random key from: `A, S, D, J, K, L`.
    *   The player has **3000ms (3 seconds)** to press the correct key.
    *   This repeats **5 times**.
*   **Win Condition**: 5 successful presses.
    *   Result: "Hydroelectric Dam back to full power". Sets `qteComplete = true`.
*   **Fail Condition**: Pressing wrong key or timeout.
    *   Result: "Failure". Must wait 10 seconds (simulated by `Thread.Sleep`) before retrying.
*   **Alternative Win**: If `leverRepaired` is true, the QTE is skipped and the dam is fixed instantly.

---

### 3.4 Region 3: Windy Highlands (South)

#### Room Map
1.  **Mount Boreal** (Entry)
    *   `south` -> Cabin
2.  **Cabin**
    *   `north` -> Mount Boreal
    *   `east` -> Garden
    *   **Item**: `snack`.
        *   Action: `take snack` -> Adds 'snack' to inventory.
3.  **Garden**
    *   `north` -> Shed
    *   `south` -> Turbines
    *   `west` -> Cabin
    *   **Item**: `code`.
        *   Condition: Only appears/takable if `box` flag is true (visited Metal Box).
        *   Action: `take code` -> Adds 'code' to inventory.
4.  **Shed**
    *   `south` -> Garden
    *   **Lock Condition**: Requires **Shed Key** in inventory to enter.
    *   **Item**: `note` (Hint Note). `take note` -> Adds 'note' to inventory.
    *   **Item**: `power cables`. `take power cables` -> Adds 'power cables' to inventory.
5.  **Turbines**
    *   `north` -> Garden
    *   `east` -> Tower
    *   `south` -> Metal Box
    *   `west` -> Stream
    *   **NPC**: Prof. KaelStormwright (Endgame interaction).
        *   Action: `talk`. If `step2` is true -> **Region Complete**. Moves player to **Aurora Control Hub**.
6.  **Stream**
    *   `north` -> Tents
    *   `east` -> Turbines
    *   **Item**: `control board`.
        *   Action: `take control board` or `take board`.
        *   Condition: Requires `fedRaccoon = true`. If false, raccoon steals it back.
7.  **Tents**
    *   **NPC**: Raccoon.
    *   **Interaction**: `use snack`.
        *   Condition: Requires **snack** in inventory.
        *   Outcome: Sets `fedRaccoon = true`. Adds **control board** to inventory (auto-gives item logic in code implies raccoon drops it, but `use` command sets flag directly).
8.  **Metal Box**
    *   `north` -> Turbines
    *   **Event**: Entering sets `box = true` (allows finding code in Garden).
    *   **Item**: `anemometer`.
        *   Condition: Requires **code** in inventory to open box.
        *   Action: `take anemometer` -> Adds 'anemometer' to inventory.
9.  **Tower**
    *   `north` -> Computers
    *   `east` -> Office
    *   `south` -> Turbines
    *   **Item**: `flimsy cables`.
        *   Condition: Only visible if **Shed Key** is in inventory.
        *   Action: `take flimsy cables` -> Adds 'flimsy cables' to inventory.
10. **Office**
    *   `west` -> Tower
    *   **NPC**: Prof. Kael Stormwright.
    *   **Interaction**: `talk`.
        *   If **anemometer** AND **control board** AND **power cables** in inventory: Sets `step1 = true`. (Good Ending path).
        *   If **anemometer** AND **control board** AND **flimsy cables** in inventory (and NO power cables): Dialog about flimsy cables.
        *   If missing items: Gives **Shed Key** (Adds 'Shed Key' to inventory).
11. **Computers**
    *   `south` -> Tower
    *   **Interaction**: `talk`.
        *   Condition: Requires `step1 = true`.
        *   Outcome: Sets `step2 = true`.

## 4. Architecture & Design

### 4.1 Three-Layer Architecture
The project follows a strict separation of concerns using a 3-layer architecture:

1.  **Presentation Layer (UI)**
    *   **Responsibility**: Handles all user input and console output.
    *   **Principles**:
        *   The UI layer should only be responsible for displaying information to the user and capturing raw input.
        *   It should not contain any game logic or state management.
        *   Input parsing should convert raw text into structured data before passing it to the logic layer.

2.  **Business Logic Layer (Domain)**
    *   **Responsibility**: Executes game rules, processes commands, and manages game flow.
    *   **Principles**:
        *   This layer contains the core mechanics of the game (navigation, interaction, combat, etc.).
        *   It coordinates interactions between the player and the game world.
        *   It should be independent of the specific UI implementation.

3.  **Data Layer (State & Persistence)**
    *   **Responsibility**: Stores the current state of the world, player inventory, and static game data.
    *   **Principles**:
        *   This layer encapsulates all mutable state (player location, inventory, world flags).
        *   It provides a clean interface for the logic layer to query and modify state.
        *   It separates the "save data" from the runtime logic.

### 4.2 OOP Principles
The design emphasizes the following Object-Oriented principles:

*   **Encapsulation**:
    *   Game state should not be global. State should be encapsulated within the relevant objects (e.g., a Room object should know if it is locked).
    *   Objects should manage their own internal state and expose methods to modify it, rather than allowing direct external modification.

*   **Polymorphism (Command Pattern)**:
    *   Commands should be treated as objects rather than simple strings handled by a large switch statement.
    *   An abstract base class or interface should define the contract for execution, allowing different command types to implement specific logic.

*   **Single Responsibility Principle (SRP)**:
    *   Each class should have one specific job. For example, a parser should only parse text, not execute commands. A room object should only hold data about the location, not handle player input.

## 5. Implementation Details

### 5.1 Domain State Models
To support the architecture, state is distributed to relevant objects:

#### Room State
*   **MaintenanceTent**: `IsQuizCompleted` (bool)
*   **MetalBox**: `IsVisited` (bool)
*   **ControlRoom**: `IsLeverRepaired` (bool), `IsLeverDerusted` (bool)
*   **TheDamPlant**: `IsDoorLocked` (bool)
*   **Shed**: `IsLocked` (bool)

#### NPC State
*   **Dr. Liora Sunvale**: `HasTalked` (bool)
*   **Raccoon**: `IsFed` (bool)
*   **Prof. Kael Stormwright**: `QuestStage` (Enum)

#### Global/Quest State
*   **HydroRegion**: `IsQTEComplete` (bool)
*   **WindyRegion**: `IsPowerRestored` (bool)

### 5.2 Items (Inventory Data)
*   `Desert Key`, `Water Hose`, `Robotic Parts 1`, `Robotic Parts 2`
*   `Dam Key`, `New Lever`, `Berries`, `Pinecone`
*   `Snack`, `Code`, `Hint Note`, `Power Cables`, `Control Board`, `Anemometer`, `Flimsy Cables`, `Shed Key`

## 6. Script & Text Assets

### 6.1 Room Descriptions
*   **Aurora Control Hub**: "You are in the Aurora Control Hub, the heart of the last renewable energy initiative. The air hums with faint backup power. Screens flicker, showing maps of four darkened regions. A workbench lies in the corner with scattered tools."
*   **Solar Desert**: "After walking for hours you find yourself in a desolate land. The desert stretches before you. Towers of sand cover the solar field. Heat shimmers across the horizon. You find a small hub that looks like it could have life(west)"
*   **Desert Hub**: "You notice a map in front of the hub with the areas in the desert: Maintenance tent (west), Aurora Hub (east), Solar panel field (north), Junkyard (south). You decide to enter and there you find Dr. Liora Sunvale. She welcomes you and is ready to answer your questions. (talk)"
*   **Maintenance Tent**: "You are inside the maintenance tent. Various tools are scattered about. In the corner sits a wooden box labeled 'Junkyard'."
*   **Solar Panel Fields**: "You find yourself in the Solar Panel Fields and notice a lot of piles of sand. You try to dig into one and you find a solar panel. There are thousands of them. How will you clean up the piles: (1) Water Hose (unreliable) (2) Robotic maintenece"
*   **Junkyard**: "You use the key to enter the Junkyard and there you find 3 exits labeled: Water Supplies (west), Scrapyard 1 (south), Scrapyard 2 (east)"
*   **Scrapyard 1**: "You've entered a scrapyard filled with piles of old parts and debris. Searching through the scraps, you spot some robotic parts that might be useful."
*   **Scrapyard 2**: "Another scrapyard section spreads before you. Mountains of discarded equipment clutter the area. You notice more robotic parts among the junk."
*   **Water Supplies**: "You are in the water supplies storage. A huge pile of materials lies before you. Among the supplies, you can see a long water hose with a portable tank."
*   **Hydro Hub**: "You start walking toward the river and arrive at a junction. A sign reads: ==Welcome to the Hydro Hub== Aurora Hub (south), Research Center (north), Hydroelectric Dam (east), Tundra Forest (west)"
*   **The Dam Plant**: "After a short stroll you arrive at the riverside with no bridge leading across. A large Hydroelectric Plant sits here; an entrance leads to the Control Room (north). Use the Dam Key to unlock the Control Room if necessary."
*   **Research Center**: "You step into the lobby of an Aurora outpost. This building supports engineers on their missions and has two main sections: a Library (up) and the Cafeteria (east). The way back to the Hydro Hub is south."
*   **Tundra Forrest**: "You have entered the Tundra forrest. There are giant trees as far as you can see. You see some items you that you can move, a few pinecones on the ground(take pinecone) and some berries on a nearby bush(take berries), you can try to wander around but it might get you lost"
*   **Library**: "Loads of heavy shelves hold thousands of technical theory, documents and old logbooks. A single chair is occupied by a person reading one of the books, you can approach ther (talk). The lobby is downstrairs(down)"
*   **Cafeteria**: "A regular cafeteria. Near the serving station, you see a small, out of place item lying among the cutlery. It looks like a key — do you take it? (take key) It might come in handy later... The lobby is to the west."
*   **Top of the Hill**: "After climbing up the hill you find a forgotten toolbox. You see a box labeled levers, take one?(take lever). The only way down is back to the Tundra (south)."
*   **Control Room**: "You walk deep into the dam to the Control Room. Directly ahead is the emergency restart control panel with the restart lever marked; however, the lever is completely rusted and jammed shut. You can try `use lever` if you have a lever, or `use berries` and `use pinecone` together to derust it. The only way back is south to The Dam Plant."
*   **Mount Boreal**: "You're standing atop the peak of Mount Boreal. To the south is a ridge path leading to an abandoned cabin."
*   **Cabin**: "You've entered an old, abandoned cabin once used by maintenance crews. You can see old papers and spare parts scattered on the ground. To the east is a door which seems to lead to the garden. In a corner you can see a snack bar with wild berries, it could be useful. (take snack)"
*   **Garden**: "You're standing in the garden, now overgrown with weeds and bushes. You can feel the cold wind on your face. To the north is an old, half-broken shed. To the south you can see the turbines turning faintly in the distance."
*   **Shed**: "You are standing in the old shed. You can see a note on a desk nearby. (take note) You can see a bundle of cables under some boxes, they seem pretty sturdy. (take power cables)"
*   **Turbines**: "You are standing between the wind turbines. Some are turned off while others spin slowly. To the east is a control tower connected to the turbines; to the west you can hear a stream of water; to the south is a locked metal box that may contain something useful."
*   **Tower**: "You've entered the control tower. You can hear a faint static sound in the background. To the north are some old computers faintly flickering. To the east is an office."
*   **Office**: "You've entered what seems to be an administration office. You can see blueprints and written entries scattered across the floor. You can hear rustling from behind a bookshelf, maybe you should see who it is (talk)."
*   **Stream**: "You are standing next to a stream of water. To the north you can see an abandoned bonfire with a few tents nearby."
*   **Tents**: "You can see something moving in one of the tents. Mybe you should see what it is. (talk)"
*   **Metal Box**: "You're standing in front of the locked box. It seems to require a code to open."

### 6.2 NPC Dialogues
*   **Dr. Liora Sunvale (Desert Hub)**: "Welcome young scientist! Our mission is to save the 'Solar panel field' and find all burried solar panels! We have thought of 2 methods of doing it and you're more than welcome to chose which one you preffer. 1 of them is a temporary fix, so choose wisely! Visit the maintenece tent to get more information.(west)"
*   **Dr. Amara Riversong (Library)**: "Oh, hello. Looking for information on the Dam? It's truly bad luck, due to climate change the weather became even more extreme up here North as a result the dam's pipes have frozen and the lever that could reboot the pipes has became rusty and stuck shut. To add to the troubles, apparently the dam's key was misplaced by a previous Aurora member, now we can't enter the Control Room even if we had the acid to derust the lever but I have some good news aswell, I have worked out an acid that could derusting the lever and restart the energy production of the dam, it requires berry juice and pinecone dust. You should find both around the Tundra forrest. This is all the information you should need to save the Dam"
*   **Prof. Kael Stormwright (Office)**: "'Huh? Who are you?' 'Whatever, we don't have time for that, I'm sure you've seen the turbines nearby; we need to fix them.' 'I've been trying to do it on my own, but I am missing some key components.' 'I need you to bring me some power cables, a control board and an anemometer. You can find these items scattered around the map.'"
*   **Prof. Kael Stormwright (Computers)**: "'Okay, with all the missing components the computers should finally turn on..' 'Yes! We did it! Let's go see the turbines.'"
*   **Prof. Kael Stormwright (Turbines)**: "'Ah, they're working as intended. Good.' 'Thank you for the help, good luck in your adventures!'"
*   **Raccoon (Tents)**: "You found a raccoon! It seems to be playing with what seems to be a control board."

### 6.3 Item Descriptions
*   **berries**: "A cluster of edible-looking berries. Maybe they could be useful."
*   **pinecone**: "A large, pinecone."
*   **key (Dam Key)**: "A small metal tool with the Aurora symbol on it. It doesn't quite fit the shape of the rest of the spoons and forks, maybe you should investigate? (take key)"
*   **lever**: "A heavy, stainless steel lever. It looks like it could replace a rusted, jammed control."
*   **control board**: "A small control board. Can be used to fix the turbines."
*   **code**: "A small piece of paper with the name 'Rigby' on it. Maybe it's someone's name?"
*   **snack**: "A small packaged snack. Raccoons love these."
*   **note**: "It reads: 'One of the turbine parts was lost near the stream... I saw something furry running off with it.'"
*   **flimsy cables**: "A bundle of thin, worn cables. They *might* work, but probably won’t last."
*   **power cables**: "A solid set of insulated power cables — perfect for repairing the turbines."
*   **anemometer**: "A wind measurement device needed for turbine calibration."

### 6.4 Generic Messages
*   **Invalid Move**: "You can't go that way."
*   **Item Not Found**: "I don't see that here."
*   **Inventory Empty**: "You are not carrying anything."
*   **Unknown Command**: "I don't know that command."

## 7. Testing Strategy / Quality Assurance
Since the game relies heavily on state changes, the following key scenarios should be tested:

*   **Test Case A (Access Control)**: Attempt to enter the Solar Panel Fields without talking to Dr. Liora.
    *   *Expected Result*: Access denied message.
*   **Test Case B (Inventory Validation)**: Attempt to use the Wrench on the Solar Panel without having it in inventory.
    *   *Expected Result*: "You don't have that item" or similar error.
*   **Test Case C (State Persistence)**: Complete the Hydro QTE, leave the room, and return.
    *   *Expected Result*: The QTE should not trigger again; the dam should remain fixed.
*   **Test Case D (Win Condition)**: Complete all 3 regions and return to the Hub.
    *   *Expected Result*: Game displays victory message and exits/resets.

## 8. Configuration & Constants
The game uses several "magic numbers" that should be defined as constants for easy balancing:

*   **QTE_TIMEOUT_MS**: `3000` (Time allowed to press a key in the Hydro minigame).
*   **QTE_ROUNDS**: `5` (Number of successful key presses required).
*   **QTE_PENALTY_MS**: `10000` (Wait time after failing the QTE).
*   **QUIZ_CORRECT_OPTION**: `2` (The correct index for the Solar Desert quiz).

## 9. Extension Guidelines
To add new content using the established architecture:

1.  **Add a Region**: Create a new `Room` node in the graph and ensure it connects to the Hub via a cardinal direction.
2.  **Add an Item**: Instantiate a new `Item` object and add it to the `World` initialization logic.
3.  **Add a Quest**:
    *   Define a new state property in `GameState` (e.g., `IsBridgeFixed`).
    *   Add a condition in the relevant `Room` or `NPC` interaction logic to check/set this property.

## 10. Development Setup
1.  **Prerequisites**: .NET 8.0 SDK.
2.  **Build**: Run `dotnet build` in the project directory.
3.  **Run**: Run `dotnet run --project "c:\Users\User\Project Copilot\ProjectAurora\ProjectAurora.csproj"` to start the game.
