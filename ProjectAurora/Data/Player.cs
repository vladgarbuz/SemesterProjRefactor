using System.Collections.Generic;

namespace ProjectAurora.Data
{
    public class Player
    {
        private readonly List<Item> _inventory;

        public Room CurrentRoom { get; private set; }
        public Room? PreviousRoom { get; private set; }
        public IReadOnlyList<Item> Inventory => _inventory.AsReadOnly();

        public Player(Room startingRoom)
        {
            CurrentRoom = startingRoom ?? throw new System.ArgumentNullException(nameof(startingRoom));
            _inventory = new List<Item>();
        }

        public bool HasItem(string itemName)
        {
            return _inventory.Exists(i => i.Name.ToLower() == itemName.ToLower());
        }
        
        public void AddItem(Item item)
        {
            _inventory.Add(item ?? throw new System.ArgumentNullException(nameof(item)));
        }

        public void RemoveItem(string itemName)
        {
            var item = _inventory.Find(i => i.Name.ToLower() == itemName.ToLower());
            if (item != null)
            {
                _inventory.Remove(item);
            }
        }

        public void MoveTo(Room newRoom)
        {
            if (newRoom == null) throw new System.ArgumentNullException(nameof(newRoom));
            PreviousRoom = CurrentRoom;
            CurrentRoom = newRoom;
        }
    }
}
