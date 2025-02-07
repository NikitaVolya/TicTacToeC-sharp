using System;
using TicTacToeObjects;
using Serilog;
using System.Linq;

namespace Game
{
    internal class Program
    {
        static public Serilog.Core.Logger logger = new LoggerConfiguration().WriteTo.Debug().CreateLogger();

        static Game game;

        static void StartGame()
        {
            logger.Information("Start");
            bool game_mode = false;

            switch (GameScreen.UserChoice("Select game mode:", new string[] { "PvP", "PvAI" }))
            {
                case "PvP":
                    game_mode = false;
                    break;

                case "PvAI":
                    string user_input = GameScreen.UserChoice("Choose difficulty level:", TicTacToeRules.AI_LEVELS.Keys.ToArray());
                    TicTacToeRules.LOAD_STEPS = TicTacToeRules.AI_LEVELS[user_input];
                    game_mode = true;
                    break;
            }

            TicTacToeRules.FirstStep = GameScreen.UserChoice("Who goes first?:",
                new TicTacToeSymbls[] { TicTacToeSymbls.FirstPlayer, TicTacToeSymbls.SecondPlayer });

            game = new Game(game_mode);

            game.Start();
            Console.ReadKey();
        }
        
        static void Main(string[] args)
        {
            do
            {
                StartGame();
            } while (GameScreen.UserChoice("Start new game?:",
            new string[] { "yes", "no"}) == "yes");
        }
    }
}
