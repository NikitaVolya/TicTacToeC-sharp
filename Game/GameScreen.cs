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
