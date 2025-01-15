using System;

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

    public class TicTacToeRules
    {
        static float PlayerWinScore = -100.0f;
        static float AiWinScore = 100.0f;
        static float DrawScore = 0.0f;


        public static TicTacToeSymbls FirstStep = TicTacToeSymbls.FirstPlayer;

        public static TicTacToeSymbls GetNext(TicTacToeSymbls player)
        {
            if (player == TicTacToeSymbls.FirstPlayer)
                return TicTacToeSymbls.SecondPlayer;
            if (player == TicTacToeSymbls.SecondPlayer)
                return TicTacToeSymbls.FirstPlayer;
            throw new ArgumentException();
        }

        private static TicTacToeSymbls CheckLine(Matrix<TicTacToeSymbls> matrix)
        { 
            for (int i = 0; i < 3; i++)
            {
                if (matrix[i, 0] == matrix[i, 1] && 
                    matrix[i, 1] == matrix[i, 2] && 
                    matrix[i, 0] != TicTacToeSymbls.Space)
                    return matrix[i, 0];
                if (matrix[0, i] == matrix[1, i] &&
                    matrix[1, i] == matrix[2, i] &&
                    matrix[0, i] != TicTacToeSymbls.Space)
                    return matrix[0, i];
            }
            return TicTacToeSymbls.None;
        }
        private static TicTacToeSymbls CheckDiagonal(Matrix<TicTacToeSymbls> matrix)
        {
            if (matrix[0, 0] == matrix[1, 1] &&
                matrix[1, 1] == matrix[2, 2] &&
                matrix[0, 0] != TicTacToeSymbls.Space)
                return matrix[0, 0];
            if (matrix[0, 2] == matrix[1, 1] &&
                matrix[1, 1] == matrix[2, 0] &&
                matrix[0, 2] != TicTacToeSymbls.Space)
                return matrix[0, 2];
            return TicTacToeSymbls.None;
        }

        public static TicTacToeSymbls GetWinner(Matrix<TicTacToeSymbls> matrix)
        {
            TicTacToeSymbls line = CheckLine(matrix);
            if (line != TicTacToeSymbls.None)
                return line;
            TicTacToeSymbls diagonal = CheckDiagonal(matrix);
            if (diagonal != TicTacToeSymbls.None)
                return diagonal;
            return TicTacToeSymbls.None;
        }
        public static bool IsFull(Matrix<TicTacToeSymbls> matrix) => matrix.Find(TicTacToeSymbls.Space) == -1;
        public static bool IsDraw(Matrix<TicTacToeSymbls> matrix) => IsFull(matrix) && GetWinner(matrix) == TicTacToeSymbls.None;

        public static TicTacToeSymbls CheckGame(Matrix<TicTacToeSymbls> matrix)
        {
            if (IsDraw(matrix))
                return TicTacToeSymbls.Draw;
            return GetWinner(matrix);
        }

        public static bool IsOver(Matrix<TicTacToeSymbls> matrix) => CheckGame(matrix) != TicTacToeSymbls.None;

        public static float GetScore(Matrix<TicTacToeSymbls> matrix)
        {
            var end_game = CheckGame(matrix);
            if (end_game == TicTacToeSymbls.FirstPlayer)
                return PlayerWinScore;
            else if (end_game == TicTacToeSymbls.SecondPlayer)
                return AiWinScore;
            else if (end_game == TicTacToeSymbls.Draw)
                return DrawScore;

            throw new NotImplementedException();
        }
    }
}
