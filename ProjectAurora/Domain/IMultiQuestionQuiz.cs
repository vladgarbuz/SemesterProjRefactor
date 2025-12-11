namespace ProjectAurora.Domain
{
    public interface IMultiQuestionQuiz
    {
        string[] Questions { get; }
        string[][] Options { get; }
        int[] CorrectAnswers { get; }  // 1-indexed (matches user input 1, 2, 3, 4)
        int PassThreshold { get; }  // Number of correct answers needed to pass
    }
}
