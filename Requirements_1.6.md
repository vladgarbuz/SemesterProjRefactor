# Section 1.6 – Requirements

## 1.6.1 Program Specification
**Project Aurora** is a text-based adventure game where the player acts as an engineer tasked with restoring four major renewable energy sources: Solar, Hydro, Wind, and Geothermal. The game is set in a post-outage world where the "Aurora" initiative is the last hope for sustainable power.

The player starts in the **Aurora Control Hub** and must navigate through different regions, each presenting unique challenges:
- **Solar Region**: The player must assist Dr. Liora by passing a solar energy quiz, retrieving robotic parts from a junkyard, and clearing sand from solar panels.
- **Hydro Region**: The player must find a key to access the Dam Plant, obtain a lever (or use creative solutions like berries and pinecones), and complete a Quick Time Event (QTE) to restart the turbines.
- **Windy Region**: The player interacts with various NPCs, solves a code-based puzzle to open a metal box, and retrieves power cables to fix the wind turbines.
- **Geothermal Region**: The player collects thermal data and passes a geothermal knowledge quiz to restore the volcanic power plant.

The game features a command-line interface where players type commands like `go north`, `take key`, `talk`, and `use lever`. Success is achieved when all four regions are fully operational.

## 1.6.2 Verb–Noun Analysis
Applying the Verb–Noun technique to the specification:

| Nouns (Classes) | Verbs (Responsibilities) |
| :--- | :--- |
| `GameEngine` | Manage game loop, track state, handle movement |
| `Player` | Store inventory, track current location |
| `Room` | Store exits, items, and NPCs; handle entry events |
| `Item` | Define properties of collectable objects |
| `NPC` | Provide dialogue and trigger events |
| `CommandParser` | Parse user input into commands |
| `WorldBuilder` | Construct the game world and connections |
| `GameState` | Track global progress flags |
| `Quiz` / `QTE` | Execute mini-game logic |

## 1.6.3 CRC Cards

### Class: GameEngine
| **Responsibilities** | **Collaborators** |
| :--- | :--- |
| Coordinates game flow and state transitions | `Player`, `Room`, `GameState`, `WorldBuilder` |
| Processes movement and interaction logic | `CommandParser`, `ICommand` |
| Checks for win/loss conditions | `GameState` |
| Provides output messages to the UI | `ConsoleUI` |

### Class: Player
| **Responsibilities** | **Collaborators** |
| :--- | :--- |
| Maintains the player's current location | `Room` |
| Manages a list of collected items | `Item` |
| Handles adding/removing items from inventory | `Item` |

### Class: Room
| **Responsibilities** | **Collaborators** |
| :--- | :--- |
| Stores descriptions and available exits | `Room` |
| Holds items and NPCs present in the location | `Item`, `NPC` |
| Enforces entry requirements | `IEntryRequirement` |
| Triggers specific events upon entry | `GameEngine` |

### Class: CommandParser
| **Responsibilities** | **Collaborators** |
| :--- | :--- |
| Tokenizes and interprets user string input | `GameEngine` |
| Maps input to specific command actions | `ICommand` |

### Class: WorldBuilder
| **Responsibilities** | **Collaborators** |
| :--- | :--- |
| Instantiates all rooms, items, and NPCs | `Room`, `Item`, `NPC` |
| Defines the spatial relationships (exits) between rooms | `Room` |
| Sets up initial game state | `GameState` |

---

## 1.6.4 UML Class Diagram
The following diagram illustrates the one-directional relationships between the core classes and interfaces of Project Aurora.

```mermaid
classDiagram
    class Program {
        +Main(args: string[])
    }
    class ConsoleUI {
        -GameEngine _engine
        -CommandParser _parser
        +Run()
    }
    class GameEngine {
        +Player Player
        +GameState State
        +Move(direction: string)
        +Look()
        +CheckWinCondition()
    }
    class CommandParser {
        +ParseAndExecute(input: string, engine: GameEngine)
    }
    class Player {
        +Room CurrentRoom
        +List~Item~ Inventory
        +AddItem(item: Item)
    }
    class Room {
        +string Name
        +string Description
        +Dictionary~string, Room~ Exits
        +List~Item~ Items
        +NPC Occupant
        +IEntryRequirement EntryRequirement
    }
    class Item {
        +string Name
        +string Description
    }
    class NPC {
        +string Name
        +GetDialogue() string
    }
    class GameState {
        +bool SolarRestored
        +bool HydroRestored
        +bool WindRestored
        +bool GeothermalRestored
    }
    interface IEntryRequirement {
        +CanEnter(player: Player, state: GameState) bool
    }

    Program --> ConsoleUI : creates
    ConsoleUI --> GameEngine : uses
    ConsoleUI --> CommandParser : uses
    GameEngine --> Player : manages
    GameEngine --> GameState : updates
    GameEngine --> WorldBuilder : uses to initialize
    Player --> Room : occupies
    Player --> Item : carries
    Room --> Item : contains
    Room --> NPC : contains
    Room --> IEntryRequirement : has
    CommandParser --> GameEngine : executes on
```
