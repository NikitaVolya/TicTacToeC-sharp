using Serilog.Debugging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using TicTacToeObjects;

namespace Game
{
    internal class TicTacToeTree
    {
        private class Node : IDisposable
        {
            private bool _disposedValue;

            public Matrix<TicTacToeSymbls> Value { get; set; }
            public List<Node> Childes { get; set; }
            public Node Parent;
            private bool _is_leaf;
            private float _score;
            private int _weight;

            public bool IsLeaf { get => _is_leaf; }

            public float Score { get => _score; }

            private void CalculateScore()
            {
                if (_is_leaf)
                    _score = TicTacToeRules.GetScore(Value);
                else
                {
                    _score = 0.0f;
                    foreach (var node in Childes)
                        _score += node.Score * node._weight / 10.0f;
                }
                if (Parent != null)
                    Parent.CalculateScore();
            }

            public Node(Matrix<TicTacToeSymbls> value)
            {
                Value = value;

                _is_leaf = TicTacToeRules.IsOver(value);

                Childes = new List<Node>();
                Parent = null;
                _weight = 1;
                CalculateScore();
            }

            public bool AddChilde(Node node)
            {
                bool rep = false;
                var find = Childes.Find(e => e.Value.Similar(node.Value));
                if (find != null)
                {
                    find._weight += 1;
                }
                else
                {
                    rep = true;
                    Childes.Add(node);
                    node.Parent = this;
                }

                CalculateScore();
                return rep;
            }

            public Node FindChilde(Matrix<TicTacToeSymbls> matrix) => Childes.Find(e => e.Value == matrix);
            public void RemoveChilde(Node node) 
            { 
                Childes.Remove(node);
                node.Parent = null;
            }

            public void Dispose()
            {
                foreach (var node in Childes)
                    node.Dispose();
                Childes.Clear();
                Value.Dispose();

                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposedValue)
                {
                    if (disposing)
                    {
                        Value = null;
                        Parent = null;
                    }

                    _disposedValue = true;
                }
            }
        }

        private Node _root;
        private TicTacToeSymbls _step;

        public Matrix<TicTacToeSymbls> Value { get => _root.Value.Clone() as Matrix<TicTacToeSymbls>; }

        public TicTacToeTree()
        {
            _root = new Node(new Matrix<TicTacToeSymbls>(TicTacToeSymbls.Space, 3, 3));
            _step = TicTacToeRules.FirstStep;
            Update();
        }

        static private void Generate(Node node, TicTacToeSymbls symbl, int step)
        {
            if (node.IsLeaf || step == 0)
                return;

            int spaces = node.Value.Count(e => e == TicTacToeSymbls.Space);
            TicTacToeSymbls next_sybml = TicTacToeRules.GetNext(symbl);

            if (node.Childes.Count == 0)
                for (int i = 0; i < spaces; i++)
                {
                    var tmp_field = node.Value.Clone() as Matrix<TicTacToeSymbls>;
                    tmp_field.ReplaceFirst(TicTacToeSymbls.Space, symbl, i);

                    Node new_node = new Node(tmp_field);
                    node.AddChilde(new_node);
                }

            for (int i = 0; i < node.Childes.Count; i++)
                Generate(node.Childes[i], next_sybml, step - 1);
        }

        public void Update() => Generate(_root, _step, TicTacToeRules.LOAD_STEPS);

        public void MoveTo(Matrix<TicTacToeSymbls> matrix)
        {
            Node next_node = null;
            
            for (int i = 0; i < _root.Childes.Count; i++)
                if (matrix.Similar(_root.Childes[i].Value))
                { 
                    next_node = _root.Childes[i];
                    break;
                }

            if (next_node == null)
                throw new Exception();

            _root.RemoveChilde(next_node);
            _root.Dispose();

            _root = next_node;
            _step = TicTacToeRules.GetNext(_step);
        }

        public void MoveToBest()
        {
            float best_score = _root.Childes.Max(x => x.Score);
            var all_posible_nodes = from node in _root.Childes 
                                    where node.Score == best_score
                                    select node;

            Random random = new Random();
            int choice = random.Next(all_posible_nodes.Count());
            Node next_node = all_posible_nodes.ElementAt(choice);

            _root.RemoveChilde(next_node);
            _root.Dispose();

            _root = next_node;
            _step = TicTacToeRules.GetNext(_step);
        }
    }
}
