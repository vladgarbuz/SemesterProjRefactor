namespace ProjectAurora.Domain
{
    public class SolarQuiz : IMultiQuestionQuiz
    {
        public string[] Questions { get; } = new[]
        {
            "What happens if solar panels overheat?"
        };

        public string[][] Options { get; } = new[]
        {
            new[] { "More energy", "Less efficiency", "Catch fire" }
        };

        public int[] CorrectAnswers { get; } = new[] { 2 };  // 1-indexed: 'Less efficiency'

        public int PassThreshold { get; } = 1;  // Need 1 out of 1 correct
    }
}
