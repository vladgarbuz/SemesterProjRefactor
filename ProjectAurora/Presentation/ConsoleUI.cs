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
            _engine.RunSolarQuiz = RunSolarQuiz;
            _engine.RunHydroQTE = RunHydroQTE;
        }

        public void Run()
        {
            Console.Title = "Project Aurora";
            Console.Clear();
            
            PrintWelcome();

            // Initial Look
            _engine.Look();
            PrintOutput();

            while (_engine.IsRunning)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(input)) continue;

                // Clear screen on move? Spec says: "Screen Clearing: The console clears upon moving to a new room"
                // But we only know if we moved if the engine tells us.
                // The engine prints room change messages; we check if the CurrentRoom changed.
                // I'll check if the room changed?
                // Or I can just clear if the command was a movement command?
                // The engine handles logic.
                // Let's just clear if the output contains a room description?
                // Or better: The engine could have an event `OnRoomChanged`.
                // For now, I'll just print. If I want to clear, I need to know if room changed.
                // I'll check `_engine.Player.CurrentRoom` before and after?
                var oldRoom = _engine.Player.CurrentRoom;
                
                _parser.ParseAndExecute(input, _engine);
                
                if (_engine.Player.CurrentRoom != oldRoom)
                {
                    Console.Clear();
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

        private int RunSolarQuiz(ProjectAurora.Domain.IQuiz quiz)
        {
            Console.WriteLine("Question: " + quiz.Question);
            for (int i = 0; i < quiz.Options.Length; i++)
            {
                Console.WriteLine($"({i + 1}) {quiz.Options[i]}");
            }
            Console.Write("Answer: ");
            var key = Console.ReadLine();
            if (int.TryParse(key, out int result))
            {
                return result;
            }
            return 0;
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
            Console.Clear();
        }
    }
}
