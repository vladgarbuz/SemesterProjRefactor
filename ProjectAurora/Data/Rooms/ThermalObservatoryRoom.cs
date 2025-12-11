using ProjectAurora.Data;
using ProjectAurora.Domain;

namespace ProjectAurora.Data.Rooms
{
    public class ThermalObservatoryRoom : Room
    {
        public ThermalObservatoryRoom(string id, string name, string description) : base(id, name, description) { }

        public override bool OnUseItem(Item item, Player player, GameState state, GameEngine engine)
        {
            if (item.Name.ToLower() == "thermal data")
            {
                if (state.ThermalDataSubmitted)
                {
                    engine.Print("You have already submitted the thermal data to Dr. Elena Voss.");
                    return true;
                }

                engine.Print("You submit the thermal data to Dr. Elena Voss for analysis. You can now use your permit to take the quiz.");
                player.RemoveItem("thermal data");
                state.MarkThermalDataSubmitted();
                return true;
            }

            if (item.Name.ToLower() == "permit")
            {
                if (state.GeothermalCertified)
                {
                    engine.Print("You have already completed the geothermal certification.");
                    return true;
                }

                if (!state.ThermalDataSubmitted)
                {
                    engine.Print("Dr. Elena Voss needs the thermal data first. Bring it to her when you have it.");
                    return true;
                }

                if (engine.RunQuiz != null)
                {
                    var quiz = new GeothermalQuiz();
                    int correctCount = engine.RunQuiz(quiz);
                    
                    if (correctCount >= quiz.PassThreshold)
                    {
                        engine.Print($"Congratulations! You answered {correctCount} out of {quiz.Questions.Length} correctly and earned your Geothermal Certificate!");
                        player.AddItem(new Item("geothermal_certificate", "A certificate proving your knowledge of geothermal energy."));
                        player.RemoveItem("permit");
                        state.MarkGeothermalCertified();
                    }
                    else
                    {
                        engine.Print($"You only answered {correctCount} out of {quiz.Questions.Length} correctly. You need at least {quiz.PassThreshold}. Try again!");
                    }
                }
                else
                {
                    engine.Print("Quiz system is not available.");
                }
                return true;
            }
            return false;
        }
    }
}
