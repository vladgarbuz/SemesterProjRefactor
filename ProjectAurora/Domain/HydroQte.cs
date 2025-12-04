namespace ProjectAurora.Domain
{
    public class HydroQte : IQte
    {
        public char[] Keys { get; }
        public int Rounds { get; }

        public HydroQte(int rounds = 5)
        {
            Rounds = rounds;
            Keys = new[] { 'A', 'S', 'D', 'J', 'K', 'L' };
        }
    }
}
