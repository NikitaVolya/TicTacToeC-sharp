using System;
using System.Collections.Generic;
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

        public Matrix<TicTacToeSymbls> Value { get => _root.Value.Clone() as Matrix<TicTacToeSymbls>; }

        public TicTacToeTree()
        {
            _root = new Node(new Matrix<TicTacToeSymbls>(TicTacToeSymbls.Space, 3, 3));
            Generate(_root, TicTacToeRules.FirstStep);
        }

        static private void Generate(Node node, TicTacToeSymbls symbl)
        {
            if (node.IsLeaf)
                return;

            int spaces = node.Value.Count(e => e == TicTacToeSymbls.Space);
            TicTacToeSymbls next_sybml = TicTacToeRules.GetNext(symbl);

            for (int i = 0; i < spaces; i++)
            {
                var tmp_field = node.Value.Clone() as Matrix<TicTacToeSymbls>;
                tmp_field.ReplaceFirst(TicTacToeSymbls.Space, symbl, i);

                Node new_node = new Node(tmp_field);
                if (node.AddChilde(new_node))
                    Generate(new_node, next_sybml);
            }
        }

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
        }

        public void MoveToBest()
        {
            Node next_node = _root.Childes[0];
            for (int i = 0; i <  _root.Childes.Count; i++) 
                if (next_node.Score < _root.Childes[i].Score)
                    next_node = _root.Childes[i];

            _root.RemoveChilde(next_node);
            _root.Dispose();

            _root = next_node;
        }
    }
}
