using System.Collections.Generic;

namespace ProjectAurora.Data
{
    public class GameState
    {
        private readonly System.Collections.Generic.Dictionary<string, bool> _flags = new();

        public bool TalkedToLiora => GetFlag(nameof(TalkedToLiora));
        public bool QuizCompleted => GetFlag(nameof(QuizCompleted));
        public bool SolarFixed => GetFlag(nameof(SolarFixed));
        public bool LeverRepaired => GetFlag(nameof(LeverRepaired));
        public bool QteComplete => GetFlag(nameof(QteComplete));
        public bool BoxVisited => GetFlag(nameof(BoxVisited));
        public bool FedRaccoon => GetFlag(nameof(FedRaccoon));
        public bool Step1Complete => GetFlag(nameof(Step1Complete));
        public bool Step2Complete => GetFlag(nameof(Step2Complete));
        public bool WindyRestored => GetFlag(nameof(WindyRestored));

        public GameState()
        {
            // Defaults are false by default (dictionary starts empty)
        }

        public bool GetFlag(string name)
        {
            return _flags.TryGetValue(name, out var value) && value;
        }

        public void SetFlag(string name, bool value = true)
        {
            _flags[name] = value;
        }

        // Solar
        public void MarkTalkedToLiora() => SetFlag(nameof(TalkedToLiora));
        public void CompleteQuiz() => SetFlag(nameof(QuizCompleted));
        public void SetSolarFixed() => SetFlag(nameof(SolarFixed));

        // Hydro
        public void MarkLeverRepaired() => SetFlag(nameof(LeverRepaired));
        public void MarkQteComplete() => SetFlag(nameof(QteComplete));

        // Windy
        public void MarkBoxVisited() => SetFlag(nameof(BoxVisited));
        public void MarkFedRaccoon() => SetFlag(nameof(FedRaccoon));
        public void MarkStep1Complete() => SetFlag(nameof(Step1Complete));
        public void MarkStep2Complete() => SetFlag(nameof(Step2Complete));
        public void MarkWindyRestored() => SetFlag(nameof(WindyRestored));
    }
}
