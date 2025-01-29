using System;
using TicTacToeObjects;

namespace Game
{
    internal class GameScreen
    {
        Game _game;

        public GameScreen(Game gane)
        {
            _game = gane;
        }

        static public T UserChoice<T>(string title, T[] menu)
        {
            
            if (menu.Length == 0)
                throw new Exception();
            int choice = 0;
            bool input_cycle = true;

            while (input_cycle)
            {
                Console.Clear();
                Console.WriteLine(title);
                for (int i = 0; i < menu.Length; i++)
                {
                    if (i == choice)
                        Console.WriteLine("[{0}]", menu[i]);
                    else
                        Console.WriteLine(" {0} ",menu[i]);
                }

                ConsoleKeyInfo user_input = Console.ReadKey();

                switch (user_input.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (choice != 0) choice--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (choice < menu.Length - 1 ) choice++;
                        break;
                    case ConsoleKey.Enter:
                        input_cycle = false;
                        break;
                }
            }
            Program.logger.Information("Choced option {0}", menu[choice]);
            return menu[choice];
        }

        private void DrawField(Vector cursor = null)
        {
            Matrix<TicTacToeSymbls> field = _game.GameField;
            Console.Clear();
            Console.WriteLine("===========");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (cursor == null || cursor.Y != i || cursor.X != j)
                        Console.Write(" {0} ", (char)field[i, j]);
                    else
                        Console.Write("({0})", (char)field[i, j]);
                    if (j != 2)
                        Console.Write('|');
                }
                Console.Write('\n');
            }
            Console.WriteLine("===========");
        }

        public Vector UserInput()
        {
            var game_field = _game.GameField;
            Vector rep = new Vector { X = 0, Y = 0};
            while (true)
            {
                DrawField(rep);
                Console.WriteLine("Arrows - move, Enter - choice");
                ConsoleKeyInfo user_inpit = Console.ReadKey();
                switch (user_inpit.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (rep.Y != 0)
                            rep.Y -= 1;
                        break;
                    case ConsoleKey.DownArrow:
                        if (rep.Y != 2)
                            rep.Y += 1;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (rep.X != 0)
                            rep.X -= 1;
                        break;
                    case ConsoleKey.RightArrow:
                        if (rep.X != 2)
                            rep.X += 1;
                        break;
                    case ConsoleKey.Enter:
                        if (game_field[rep] == TicTacToeSymbls.Space)
                            return rep;
                        break;
                }
            }
        }

        public void Draw() => DrawField();
    }
}
