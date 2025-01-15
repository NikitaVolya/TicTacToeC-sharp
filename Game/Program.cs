using System;
using TicTacToeObjects;

namespace Game
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game;

            Console.WriteLine("How is first?:\n1. X\n2. O");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    TicTacToeRules.FirstStep = TicTacToeSymbls.FirstPlayer;
                    break;
                case "2":
                    TicTacToeRules.FirstStep = TicTacToeSymbls.SecondPlayer;
                    break;
            }
            Console.WriteLine("Choice game mode:\n1. PvP\n2. PvAI");

            choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    game = new Game(false);
                    break;
                    
                case "2":
                    game = new Game(true);
                    break;
                default:
                    return;
            }

            game.Start();

            Console.ReadKey();
        }
    }
}
