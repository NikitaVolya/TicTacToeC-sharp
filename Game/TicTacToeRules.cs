using System;
using System.Collections.Generic;

namespace TicTacToeObjects
{

    public enum TicTacToeSymbls
    {
        FirstPlayer = 'X',
        SecondPlayer = 'O',
        Space = '.',
        Draw = '-',
        None = '#'
    }

    /// <summary>
    /// Contains the rules and logic for evaluating a Tic-Tac-Toe game.
    /// </summary>
    public class TicTacToeRules
    {
        static float PlayerWinScore = -100.0f;
        static float AiWinScore = 50.0f;
        static public int LOAD_STEPS = 4;

        static public Dictionary<string, int> AI_LEVELS = new Dictionary<string, int>() {
            {"dumb", 2 }, { "normal", 4}, {"hard", 5 }, {"impossible", 9 } 
        };

        public static TicTacToeSymbls FirstStep = TicTacToeSymbls.FirstPlayer;

        /// <summary>
        /// Returns the next player's symbol in the game.
        /// </summary>
        /// <param name="player">The current player's symbol.</param>
        /// <returns>The next player's symbol.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided symbol is invalid.</exception>
        public static TicTacToeSymbls GetNext(TicTacToeSymbls player)
        {
            if (player == TicTacToeSymbls.FirstPlayer)
                return TicTacToeSymbls.SecondPlayer;
            if (player == TicTacToeSymbls.SecondPlayer)
                return TicTacToeSymbls.FirstPlayer;
            throw new ArgumentException("Invalid player symbol.");
        }

        /// <summary>
        /// Checks for a winning row or column in a Tic-Tac-Toe grid.
        /// </summary>
        /// <param name="matrix">The 3x3 Tic-Tac-Toe board.</param>
        /// <returns>The winning player's symbol if a line is found, otherwise TicTacToeSymbls.None.</returns>
        /// <exception cref="ArgumentException">Thrown if the matrix is not 3x3.</exception>
        private static TicTacToeSymbls CheckLine(Matrix<TicTacToeSymbls> matrix)
        {
            if (matrix.Height != 3 || matrix.Width != 3)
                throw new ArgumentException("Matrix must be 3x3.");

            for (int i = 0; i < 3; i++)
            {
                // Check horizontal lines
                if (matrix[i, 0] == matrix[i, 1] &&
                    matrix[i, 1] == matrix[i, 2] &&
                    matrix[i, 0] != TicTacToeSymbls.Space)
                    return matrix[i, 0];

                // Check vertical lines
                if (matrix[0, i] == matrix[1, i] &&
                    matrix[1, i] == matrix[2, i] &&
                    matrix[0, i] != TicTacToeSymbls.Space)
                    return matrix[0, i];
            }
            return TicTacToeSymbls.None;
        }

        /// <summary>
        /// Checks for a winning diagonal in a Tic-Tac-Toe grid.
        /// </summary>
        /// <param name="matrix">The 3x3 Tic-Tac-Toe board.</param>
        /// <returns>The winning player's symbol if a diagonal is found, otherwise TicTacToeSymbls.None.</returns>
        /// <exception cref="ArgumentException">Thrown if the matrix is not 3x3.</exception>
        private static TicTacToeSymbls CheckDiagonal(Matrix<TicTacToeSymbls> matrix)
        {
            if (matrix.Height != 3 || matrix.Width != 3)
                throw new ArgumentException("Matrix must be 3x3.");

            // Check main diagonal (\)
            if (matrix[0, 0] == matrix[1, 1] &&
                matrix[1, 1] == matrix[2, 2] &&
                matrix[0, 0] != TicTacToeSymbls.Space)
                return matrix[0, 0];

            // Check anti-diagonal (/)
            if (matrix[0, 2] == matrix[1, 1] &&
                matrix[1, 1] == matrix[2, 0] &&
                matrix[0, 2] != TicTacToeSymbls.Space)
                return matrix[0, 2];

            return TicTacToeSymbls.None;
        }

        /// <summary>
        /// Determines the winner of a Tic-Tac-Toe game by checking rows, columns, and diagonals.
        /// </summary>
        /// <param name="matrix">The 3x3 Tic-Tac-Toe board.</param>
        /// <returns>The symbol of the winning player if there is a winner, otherwise TicTacToeSymbls.None.</returns>
        public static TicTacToeSymbls GetWinner(Matrix<TicTacToeSymbls> matrix)
        {
            // Check for a winning line (row or column)
            TicTacToeSymbls line = CheckLine(matrix);
            if (line != TicTacToeSymbls.None)
                return line;

            // Check for a winning diagonal
            TicTacToeSymbls diagonal = CheckDiagonal(matrix);
            if (diagonal != TicTacToeSymbls.None)
                return diagonal;

            // No winner found
            return TicTacToeSymbls.None;
        }

        /// <summary>
        /// Checks if the Tic-Tac-Toe board is completely filled (i.e., no empty spaces remain).
        /// </summary>
        /// <param name="matrix">The 3x3 Tic-Tac-Toe board.</param>
        /// <returns>True if the board is full, otherwise false.</returns>
        public static bool IsFull(Matrix<TicTacToeSymbls> matrix) => matrix.Find(TicTacToeSymbls.Space) == -1;

        /// <summary>
        /// Checks the current state of the Tic-Tac-Toe game based on the given board.
        /// </summary>
        /// <param name="matrix">The 3x3 Tic-Tac-Toe board.</param>
        /// <returns>
        /// Returns the winner of the game as <see cref="TicTacToeSymbls"/>. 
        /// If the board is full and there is no winner, it returns <see cref="TicTacToeSymbls.Draw"/>.
        /// </returns>
        public static TicTacToeSymbls CheckGame(Matrix<TicTacToeSymbls> matrix)
        {
            TicTacToeSymbls winner = GetWinner(matrix);
            if (IsFull(matrix) && winner == TicTacToeSymbls.None)
                return TicTacToeSymbls.Draw;
            return winner;
        }

        /// <summary>
        /// Checks if the Tic-Tac-Toe game is over.
        /// </summary>
        /// <param name="matrix">The 3x3 Tic-Tac-Toe board.</param>
        /// <returns>
        /// Returns true if the game is over.
        /// </returns>
        public static bool IsOver(Matrix<TicTacToeSymbls> matrix) => CheckGame(matrix) != TicTacToeSymbls.None;

        /// <summary>
        /// Calculates the score based on the current state of the Tic-Tac-Toe game.
        /// </summary>
        /// <param name="matrix">The 3x3 Tic-Tac-Toe board.</param>
        /// <returns>
        /// Returns a float value representing the score:
        /// - A value if the first player wins.
        /// - A value if the AI (second player) wins.
        /// - Zero if the game ends in a draw.
        /// </returns>
        /// <exception cref="NotImplementedException">Thrown if an unexpected game state occurs.</exception>


        public static float GetScore(Matrix<TicTacToeSymbls> matrix)
        {
            var end_game = CheckGame(matrix);
            if (end_game == TicTacToeSymbls.FirstPlayer)
                return PlayerWinScore;
            else if (end_game == TicTacToeSymbls.SecondPlayer)
                return AiWinScore;
            else if (end_game == TicTacToeSymbls.Draw)
                return 0;
            throw new NotImplementedException();
        }
    }
}
