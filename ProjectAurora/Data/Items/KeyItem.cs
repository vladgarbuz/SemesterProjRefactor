namespace ProjectAurora.Data.Items
{
    public class KeyItem : ProjectAurora.Data.Item
    {
        public string Unlocks { get; }
        public KeyItem(string name, string description, string unlocks = "") : base(name, description, true)
        {
            Unlocks = unlocks;
        }
    }
}
