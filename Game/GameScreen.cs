using System;
using TicTacToeObjects;

namespace Game
{
    /// <summary>
    /// Handles the user interface for the Tic-Tac-Toe game.
    /// Provides methods for rendering the game field and managing user input.
    /// </summary>
    internal class GameScreen
    {

        /// <summary>
        /// Displays a menu with a given title and allows the user to select an option using arrow keys.
        /// </summary>
        /// <typeparam name="T">The type of menu options.</typeparam>
        /// <param name="title">The title of the menu displayed to the user.</param>
        /// <param name="menu">An array of options of type T.</param>
        /// <returns>The option selected by the user.</returns>
        /// <exception cref="ArgumentException">Thrown if the menu array is empty.</exception>
        static public T UserChoice<T>(string title, T[] menu)
        {
            
            if (menu.Length == 0)
                throw new ArgumentException("Menu cannot be empty.");

            int choice = 0;
            bool input_cycle = true;
            while (input_cycle)
            {
                Console.Clear();
                Console.WriteLine(title);
                for (int i = 0; i < menu.Length; i++)
                    Console.WriteLine(i == choice ? "[{0}]" : " {0} ", menu[i]);

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


        /// <summary>
        /// Draws a 3x3 Tic-Tac-Toe field in the console.
        /// Optionally highlights a cell at the cursor position.
        /// </summary>
        /// <param name="field">A 3x3 matrix representing the game board.</param>
        /// <param name="cursor">An optional cursor position to highlight a specific cell.</param>
        static public void DrawField(Matrix<TicTacToeSymbls> field, Vector cursor = null)
        {
            Console.Clear();
            Console.WriteLine("===========");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write(cursor == null || cursor.Y != i || cursor.X != j ? " {0} " : "({0})", (char)field[i, j]);
                    if (j != 2)
                        Console.Write('|');
                }
                Console.Write('\n');
            }
            Console.WriteLine("===========");
        }

        /// <summary>
        /// Allows the user to select a cell on the Tic-Tac-Toe board using arrow keys.
        /// The selection is confirmed with the Enter key.
        /// </summary>
        /// <param name="game_field">A 3x3 matrix representing the game board.</param>
        /// <returns>The coordinates of the selected cell as a Vector.</returns>
        static public Vector UserInput(Matrix<TicTacToeSymbls> game_field)
        {
            Vector rep = new Vector { X = 0, Y = 0};
            while (true)
            {
                DrawField(game_field, rep);
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
    }
}
