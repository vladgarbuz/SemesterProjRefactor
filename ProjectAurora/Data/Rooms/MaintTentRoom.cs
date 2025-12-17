using ProjectAurora.Domain;
using ProjectAurora.Data;

namespace ProjectAurora.Data.Rooms
{
    public class MaintTentRoom : Room
    {
        public MaintTentRoom(string id, string name, string description) : base(id, name, description)
        {
        }

        public override bool OnEnter(Player player, GameState state, GameEngine engine)
        {
            if (!state.QuizCompleted && engine.RunQuiz != null)
            {
                var quiz = new ProjectAurora.Domain.SolarQuiz();
                int correctCount = engine.RunQuiz(quiz);
                if (correctCount >= quiz.PassThreshold)
                {
                    state.CompleteQuiz();
                    engine.Print("Correct!");
                    return true;
                }
                else
                {
                    engine.Print("Incorrect. You cannot enter.");
                    return false;
                }
            }

            return true;
        }
    }
}
