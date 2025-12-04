namespace ProjectAurora.Domain
{
    public class SolarQuiz : IQuiz
    {
        public string Question { get; }
        public string[] Options { get; }
        public int CorrectAnswer { get; }

        public SolarQuiz()
        {
            Question = "What happens if solar panels overheat?";
            Options = new[] { "More energy", "Less efficiency", "Catch fire" };
            CorrectAnswer = 2; // 'Less efficiency'
        }
    }
}
