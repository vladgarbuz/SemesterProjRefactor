namespace ProjectAurora.Domain
{
    public interface IQuiz
    {
        string Question { get; }
        string[] Options { get; }
        int CorrectAnswer { get; }
    }
}
