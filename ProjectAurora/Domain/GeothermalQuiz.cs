namespace ProjectAurora.Domain
{
    public class GeothermalQuiz : IMultiQuestionQuiz
    {
        public string[] Questions { get; } = new[]
        {
            "What is the main advantage of geothermal energy over solar and wind?",
            "What happens to the water after it's used in a geothermal plant?",
            "At what depth do geothermal plants typically access hot water?"
        };

        public string[][] Options { get; } = new[]
        {
            new[] { "Cheaper to build", "Provides constant 24/7 power", "Uses less land", "Easier to maintain" },
            new[] { "Released into rivers", "Reinjected back underground", "Evaporated completely", "Stored in tanks" },
            new[] { "100-500 meters", "1-3 kilometers", "5-10 kilometers", "20+ kilometers" }
        };

        public int[] CorrectAnswers { get; } = new[] { 2, 2, 2 };  // 1-indexed: B, B, B

        public int PassThreshold { get; } = 2;  // Need 2 out of 3 correct
    }
}
