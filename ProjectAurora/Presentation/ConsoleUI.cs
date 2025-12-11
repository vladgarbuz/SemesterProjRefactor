using System;
using System.Threading;
using ProjectAurora.Domain;

namespace ProjectAurora.Presentation
{
    public class ConsoleUI
    {
        private readonly GameEngine _engine;
        private readonly CommandParser _parser;

        public ConsoleUI()
        {
            _engine = new GameEngine();
            _parser = new CommandParser();

            // Wire up delegates
            _engine.RunQuiz = RunQuiz;
            _engine.RunHydroQTE = RunHydroQTE;
        }

        public void Run()
        {
            Console.Title = "Project Aurora";
            ClearConsole();
            
            PrintWelcome();

            // Initial Look
            _engine.Look();
            PrintOutput();

            while (_engine.IsRunning)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(input)) continue;

                var oldRoom = _engine.Player.CurrentRoom;
                
                _parser.ParseAndExecute(input, _engine);
                
                if (_engine.Player.CurrentRoom != oldRoom)
                {
                    ClearConsole();
                }
                
                PrintOutput();
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private void PrintOutput()
        {
            if (!string.IsNullOrEmpty(_engine.OutputMessage))
            {
                var msg = _engine.OutputMessage;
                
                // Check for color tags
                if (msg.Contains("__RED__"))
                {
                    string[] parts = msg.Split(new[] { "__RED__", "__RESET__" }, StringSplitOptions.None);
                    // Even indices are normal, odd indices are RED (assuming balanced tags)
                    
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (i % 2 == 1) // Odd index = Red
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(parts[i]);
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.Write(parts[i]);
                        }
                    }
                    Console.WriteLine();
                }
                // Simple heuristic coloring for errors
                else if (msg.StartsWith("I don't know") || msg.StartsWith("You can't") || msg.StartsWith("Access denied") || msg.StartsWith("Move where"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(msg);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(msg);
                }
                
                _engine.ClearOutput(); // Clear after printing
            }
        }

        private int RunQuiz(ProjectAurora.Domain.IMultiQuestionQuiz quiz)
        {
            Console.WriteLine();
            if (quiz.Questions.Length > 1)
            {
                Console.WriteLine($"=== QUIZ: Answer {quiz.PassThreshold} out of {quiz.Questions.Length} correctly to pass ===");
            }
            else
            {
                Console.WriteLine("=== QUIZ ===");
            }
            Console.WriteLine();

            int correctCount = 0;

            for (int q = 0; q < quiz.Questions.Length; q++)
            {
                if (quiz.Questions.Length > 1)
                {
                    Console.WriteLine($"Question {q + 1}: {quiz.Questions[q]}");
                }
                else
                {
                    Console.WriteLine($"Question: {quiz.Questions[q]}");
                }
                for (int i = 0; i < quiz.Options[q].Length; i++)
                {
                    Console.WriteLine($"  ({i + 1}) {quiz.Options[q][i]}");
                }
                Console.Write("Your answer: ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out int answer) && answer == quiz.CorrectAnswers[q])
                {
                    if (quiz.Questions.Length > 1)
                    {
                        Console.WriteLine("Correct!");
                    }
                    correctCount++;
                }
                else
                {
                    if (quiz.Questions.Length > 1)
                    {
                        Console.WriteLine("Incorrect.");
                    }
                }
                Console.WriteLine();
            }

            if (quiz.Questions.Length > 1)
            {
                Console.WriteLine($"You answered {correctCount} out of {quiz.Questions.Length} correctly.");
                Console.WriteLine();
            }

            return correctCount;
        }

        private bool RunHydroQTE(ProjectAurora.Domain.IQte qte)
        {
            int timeoutMs = 3000; // allow 3 seconds per key press
            Console.WriteLine($"PREPARE FOR QTE! Press the keys shown within {timeoutMs / 1000} seconds!");
            Thread.Sleep(1000);
            Random rnd = new Random();
            char[] keys = qte.Keys;
            int rounds = qte.Rounds;

            for (int i = 0; i < rounds; i++)
            {
                char target = keys[rnd.Next(keys.Length)];
                Console.WriteLine($"PRESS: {target}");
                
                // Wait for input with timeout
                // Console.KeyAvailable loop
                DateTime start = DateTime.Now;
                bool pressed = false;
                
                while ((DateTime.Now - start).TotalMilliseconds < timeoutMs)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true).KeyChar;
                        if (char.ToUpper(key) == target)
                        {
                            pressed = true;
                            Console.WriteLine(" OK!");
                            break;
                        }
                        else
                        {
                            Console.WriteLine(" WRONG!");
                            return false;
                        }
                    }
                    Thread.Sleep(10);
                }

                if (!pressed)
                {
                    Console.WriteLine(" TOO SLOW!");
                    return false;
                }
            }

            return true;
        }

        private void ClearConsole()
        {
            try
            {
                if (!Console.IsOutputRedirected)
                {
                    Console.Clear();
                }
            }
            catch (System.IO.IOException)
            {
                // Ignore
            }
        }

        private void PrintWelcome()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
  _____           _           _      _                               
 |  __ \         (_)         | |    / \                              
 | |__) | __ ___  _  ___  ___| |_  / _ \   _   _ _ __ ___  _ __ __ _ 
 |  ___/ '__/ _ \| |/ _ \/ __| __|/ ___ \ | | | | '__/ _ \| '__/ _` |
 | |   | | | (_) | |  __/ (__| |_/ /   \ \| |_| | | | (_) | | | (_| |
 |_|   |_|  \___/| |\___|\___|\__/_/     \_\__,_|_|  \___/|_|  \__,_|
                _/ |                                                 
               |__/                                                  
");
            Console.ResetColor();
            Console.WriteLine("Welcome, Engineer. The world needs your help.");
            Console.WriteLine();
            
            Console.WriteLine("OBJECTIVE:");
            Console.WriteLine("  Restore power to the three regions: Solar Desert, Hydro Hub, and Windy Highlands.");
            Console.WriteLine("  Return to the Aurora Control Hub once all systems are online.");
            Console.WriteLine();
            
            Console.WriteLine("COMMANDS:");
                Console.WriteLine("  - Navigation:  north, south, east, west, up, down");
            Console.WriteLine("  - Interaction: look, take [item], use [item], talk, inventory (or inv, i)");
            Console.WriteLine("  - System:      help, exit");
            Console.WriteLine();
            
            Console.WriteLine("Press any key to start...");
            if (!Console.IsInputRedirected)
            {
                Console.ReadKey();
            }
            ClearConsole();
        }
    }
}
