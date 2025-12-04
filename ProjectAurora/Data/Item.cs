namespace ProjectAurora.Data
{
    public class Item
    {
        public string Name { get; }
        public string Description { get; }
        public bool IsPickupable { get; }

        public Item(string name, string description, bool isPickupable = true)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            Description = description ?? throw new System.ArgumentNullException(nameof(description));
            IsPickupable = isPickupable;
        }
    }
}
