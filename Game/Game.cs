using Serilog;
using System;
using TicTacToeObjects;

namespace Game
{
    internal class Game
    {
        GameScreen _screen;
        Matrix<TicTacToeSymbls> _game_field;

        TicTacToeTree _tree;

        Action _first_player;
        Action _second_player;
        TicTacToeSymbls _current_player = TicTacToeRules.FirstStep;

        public Matrix<TicTacToeSymbls> GameField { get => _game_field.Clone() as Matrix<TicTacToeSymbls>; }

        public Game(bool ai_mode) {
            _screen = new GameScreen(this);
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

        public void PlayerStep(TicTacToeSymbls symbl)
        {
            Vector position = _screen.UserInput();
            _game_field[position] = symbl;
        }

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
            _screen.Draw();

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
