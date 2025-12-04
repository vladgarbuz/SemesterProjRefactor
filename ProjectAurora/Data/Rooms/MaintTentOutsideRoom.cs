using ProjectAurora.Domain;
using ProjectAurora.Data;

namespace ProjectAurora.Data.Rooms
{
    public class MaintTentOutsideRoom : Room
    {
        public MaintTentOutsideRoom(string id, string name, string description) : base(id, name, description)
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
                    if (Exits.TryGetValue("north", out var maintTent))
                    {
                        player.MoveTo(maintTent);
                    }
                    // Award the Desert Key only once and only on success
                    if (!player.HasItem("Desert Key"))
                    {
                        player.AddItem(new ProjectAurora.Data.Items.KeyItem("Desert Key", "Key to the Junkyard."));
                    }
                    engine.Print("You received the Desert Key.");
                    return true;
                }
                else
                {
                    engine.Print("Incorrect. You step back.");
                    return false;
                }
            }

            return true;
        }
    }
}
