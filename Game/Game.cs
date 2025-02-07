using Serilog;
using System;
using TicTacToeObjects;

namespace Game
{
    /// <summary>
    /// Manages the Tic-Tac-Toe game logic, including player turns, AI moves, and game flow.
    /// Handles the game field, determines the winner, and manages the game loop.
    /// </summary>
    internal class Game
    {
        Matrix<TicTacToeSymbls> _game_field;

        TicTacToeTree _tree;

        Action _first_player;
        Action _second_player;
        TicTacToeSymbls _current_player = TicTacToeRules.FirstStep;

        /// <summary>
        /// Gets a clone of the current game field to prevent direct modifications.
        /// </summary>
        public Matrix<TicTacToeSymbls> GameField { get => _game_field.Clone() as Matrix<TicTacToeSymbls>; }

        /// <summary>
        /// Initializes a new game of Tic-Tac-Toe.
        /// Sets up the game field and assigns player or AI control based on the mode.
        /// </summary>
        /// <param name="ai_mode">If true, the second player is controlled by AI; otherwise, it's another player.</param>
        public Game(bool ai_mode) {
            _game_field = new Matrix<TicTacToeSymbls>(TicTacToeSymbls.Space, 3, 3);
            _first_player = () => PlayerStep(TicTacToeSymbls.FirstPlayer);

            if (ai_mode)
            {
                _second_player = () => AIStep();
                _tree = new TicTacToeTree();
            }
            else 
            {
                _second_player = () => PlayerStep(TicTacToeSymbls.SecondPlayer);
                _tree = null;
            }
        }

        /// <summary>
        /// Executes the AI's turn in the Tic-Tac-Toe game.
        /// Moves the AI's decision tree to the current game state, selects the best move,
        /// and updates the game field accordingly.
        /// </summary>
        public void AIStep()
        {
            Program.logger.Information("AI Step start");

            MatrixReflexion reflexion = MatrixReflexion.None;
            if (_game_field != _tree.Value)
            {
                Program.logger.Information("Move tree to next node position");
                _tree.MoveTo(_game_field);

                reflexion = _game_field.GetReflexionTo(_tree.Value);
                Program.logger.Information("Game field reflexion: {0}", reflexion);
            }


            Program.logger.Information("Move tree to best node");
            _tree.MoveToBest();
            _tree.Update();

            _game_field = _tree.Value;
            _game_field.Rotate(reflexion);
        }

        /// <summary>
        /// Handles a player's turn by allowing them to select a cell and placing their symbol.
        /// </summary>
        /// <param name="symbl">The symbol (X or O) representing the player.</param>
        public void PlayerStep(TicTacToeSymbls symbl)
        {
            Vector position = GameScreen.UserInput(_game_field);
            _game_field[position] = symbl;
        }

        /// <summary>
        /// Starts and manages the Tic-Tac-Toe game loop until the game is over.
        /// Alternates turns between players or AI, checks for a winner, and displays the result.
        /// </summary>
        public void Start()
        {
            while (!TicTacToeRules.IsOver(_game_field))
            {
                if (_current_player == TicTacToeSymbls.FirstPlayer)
                    _first_player();
                else
                    _second_player();

                _current_player = TicTacToeRules.GetNext(_current_player);
            }
            GameScreen.DrawField(_game_field);

            var end = TicTacToeRules.CheckGame(_game_field);
            if (end == TicTacToeSymbls.FirstPlayer)
                Console.WriteLine("First player win!");
            else if (end == TicTacToeSymbls.SecondPlayer)
                Console.WriteLine("Second player win!");
            else
                Console.WriteLine("Draw!!!");
        }
    }
}
