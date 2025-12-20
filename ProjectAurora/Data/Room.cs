
using System.Collections.Generic;

namespace ProjectAurora.Data
{
    public class Room
    {
        private readonly Dictionary<string, Room> _exits;
        private readonly List<Item> _items;

        public string ID { get; }
        public string Name { get; }
        public string Description { get; protected set; }
        public IReadOnlyDictionary<string, Room> Exits => _exits;
        public IReadOnlyList<Item> Items => _items.AsReadOnly();
        public bool IsLocked { get; private set; }
        public ProjectAurora.Domain.IEntryRequirement? EntryRequirement { get; init; }
        public ProjectAurora.Domain.ITalkable? Occupant { get; init; }

        // Called when the player attempts to enter the room. Return true to allow entry, false to prevent it.
        public virtual bool OnEnter(ProjectAurora.Data.Player player, ProjectAurora.Data.GameState state, ProjectAurora.Domain.GameEngine engine)
        {
            return true;
        }

        // Called after the player has successfully entered the room.
        public virtual void OnAfterEnter(ProjectAurora.Data.Player player, ProjectAurora.Data.GameState state, ProjectAurora.Domain.GameEngine engine)
        {
        }

        // Called when a player uses an item in this room; return true if the item use was handled.
        public virtual bool OnUseItem(ProjectAurora.Data.Item item, ProjectAurora.Data.Player player, ProjectAurora.Data.GameState state, ProjectAurora.Domain.GameEngine engine)
        {
            return false;
        }

        public virtual bool OnTakeItem(string itemName, ProjectAurora.Data.Player player, ProjectAurora.Data.GameState state, ProjectAurora.Domain.GameEngine engine)
        {
            return false;
        }

        public Room(string id, string name, string description)
        {
            ID = id ?? throw new System.ArgumentNullException(nameof(id));
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            Description = description ?? throw new System.ArgumentNullException(nameof(description));
            _exits = new Dictionary<string, Room>();
            _items = new List<Item>();
            IsLocked = false;
        }

        public void AddExit(string direction, Room room)
        {
            if (string.IsNullOrWhiteSpace(direction)) throw new System.ArgumentException("direction");
            _exits[direction] = room ?? throw new System.ArgumentNullException(nameof(room));
        }

        public bool TryGetExit(string direction, out Room? room)
        {
            return _exits.TryGetValue(direction, out room);
        }

        public void AddItem(Item item)
        {
            _items.Add(item ?? throw new System.ArgumentNullException(nameof(item)));
        }

        public bool RemoveItem(Item item)
        {
            return _items.Remove(item);
        }

        public void Lock() => IsLocked = true;
        public void Unlock() => IsLocked = false;
    }
}
