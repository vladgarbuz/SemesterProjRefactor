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
            if (!state.QuizCompleted && engine.RunSolarQuiz != null)
            {
                var quiz = new ProjectAurora.Domain.SolarQuiz();
                int result = engine.RunSolarQuiz(quiz);
                if (result == 2)
                {
                    state.CompleteQuiz();
                    engine.Print("Correct! You enter the tent.");
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
