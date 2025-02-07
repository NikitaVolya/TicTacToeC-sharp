using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToeObjects;

namespace Game
{
    /// <summary>
    /// Represents a decision tree for Tic-Tac-Toe that helps the AI determine the best move.
    /// The tree is made up of nodes, each representing a possible game state.
    /// </summary>
    internal class TicTacToeTree
    {
        /// <summary>
        /// Represents a node in the Tic-Tac-Toe decision tree.
        /// Each node stores a game board state, a list of child nodes (possible next moves),
        /// and a score used for AI decision-making.
        /// </summary>
        private class Node : IDisposable
        {
            private bool _disposedValue;

            private bool _is_leaf;
            private float _score;
            private int _weight;

            /// <summary>
            /// Reference to the parent node in the decision tree.
            /// </summary>
            public Node Parent;

            /// <summary>
            /// The game board state represented by this node.
            /// </summary>
            public Matrix<TicTacToeSymbls> Value { get; set; }

            /// <summary>
            /// The list of possible next moves (child nodes).
            /// </summary>
            public List<Node> Childes { get; set; }

            /// <summary>
            /// Indicates whether this node is a leaf node (i.e., game over state).
            /// </summary>
            public bool IsLeaf { get => _is_leaf; }

            /// <summary>
            /// The score assigned to this node based on the game state.
            /// </summary>
            public float Score { get => _score; }

            /// <summary>
            /// Calculates the score for the current node based on its children.
            /// </summary>
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

            /// <summary>
            /// Creates a new node representing a game state.
            /// </summary>
            /// <param name="value">The game board state.</param>
            public Node(Matrix<TicTacToeSymbls> value)
            {
                Value = value;

                _is_leaf = TicTacToeRules.IsOver(value);

                Childes = new List<Node>();
                Parent = null;
                _weight = 1;
                CalculateScore();
            }

            /// <summary>
            /// Adds a child node to this node if it does not already exist.
            /// If the child already exists, its weight is increased.
            /// </summary>
            /// <param name="node">The child node to add.</param>
            /// <returns>True if a new node was added, false if the weight was increased instead.</returns>
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

            /// <summary>
            /// Finds a child node that matches a given board state.
            /// </summary>
            /// <param name="matrix">The game board state to find.</param>
            /// <returns>The matching child node or null if not found.</returns>
            public Node FindChilde(Matrix<TicTacToeSymbls> matrix) => Childes.Find(e => e.Value == matrix);

            /// <summary>
            /// Removes a child node from this node.
            /// </summary>
            /// <param name="node">The child node to remove.</param>
            public void RemoveChilde(Node node) 
            { 
                Childes.Remove(node);
                node.Parent = null;
            }

            /// <summary>
            /// Disposes of the node and its children, freeing up resources.
            /// </summary>
            public void Dispose()
            {
                foreach (var node in Childes)
                    node.Dispose();
                Childes.Clear();
                Value.Dispose();

                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Performs cleanup operations when disposing of the node.
            /// </summary>
            /// <param name="disposing">True if called from Dispose(), false if from a finalizer.</param>
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

        /// <summary>
        /// Gets a clone of the current game state (root node) from the decision tree.
        /// This ensures the original game state is not directly modified outside of the tree.
        /// </summary>
        public Matrix<TicTacToeSymbls> Value { get => _root.Value.Clone() as Matrix<TicTacToeSymbls>; }

        /// <summary>
        /// Initializes a new instance of the TicTacToeTree class.
        /// Creates the root node representing the initial empty game state and sets up the first player's turn.
        /// </summary>
        public TicTacToeTree()
        {
            _root = new Node(new Matrix<TicTacToeSymbls>(TicTacToeSymbls.Space, 3, 3));
            _step = TicTacToeRules.FirstStep;
            Update();
        }

        /// <summary>
        /// Recursively generates the child nodes for the Tic-Tac-Toe decision tree based on the current game state.
        /// This method explores all possible moves up to a specified depth and updates the tree with new nodes.
        /// </summary>
        /// <param name="node">The current node in the decision tree, representing a specific game state.</param>
        /// <param name="symbl">The symbol (X or O) of the player whose turn it is to move.</param>
        /// <param name="step">The maximum number of steps to explore (depth of the tree). A value of 0 means no further steps will be explored.</param>
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

        /// <summary>
        /// Updates the Tic-Tac-Toe decision tree by generating child nodes based on the current game state.
        /// This method explores all possible moves up to a specified depth defined by TicTacToeRules.LOAD_STEPS.
        /// </summary>
        public void Update() => Generate(_root, _step, TicTacToeRules.LOAD_STEPS);

        /// <summary>
        /// Moves to the next game state by updating the root node of the decision tree.
        /// The specified node becomes the new root, and the previous root is removed and disposed of.
        /// </summary>
        /// <param name="next_node">The next node to move to, representing the next game state.</param>
        /// <exception cref="Exception">Thrown if the provided node is null.</exception>
        private void Move(Node next_node)
        {
            if (next_node == null)
                throw new Exception();

            _root.RemoveChilde(next_node);
            _root.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            _root = next_node;
            _step = TicTacToeRules.GetNext(_step);
        }

        /// <summary>
        /// Moves to the game state represented by the given matrix by finding the corresponding child node in the tree.
        /// The method looks for a child node whose game state matches the provided matrix and moves the root to that node.
        /// </summary>
        /// <param name="matrix">The game state matrix to move to.</param>
        /// <exception cref="Exception">Thrown if no matching child node is found.</exception>
        public void MoveTo(Matrix<TicTacToeSymbls> matrix)
        {
            Node next_node = null;
            
            for (int i = 0; i < _root.Childes.Count; i++)
                if (matrix.Similar(_root.Childes[i].Value))
                { 
                    next_node = _root.Childes[i];
                    break;
                }

            Move(next_node);
        }

        /// <summary>
        /// Moves to the child node with the best score, representing the most favorable game state for the AI.
        /// If there are multiple nodes with the same best score, a random choice is made between them.
        /// </summary>
        public void MoveToBest()
        {
            float best_score = _root.Childes.Max(x => x.Score);
            var all_posible_nodes = from node in _root.Childes 
                                    where node.Score == best_score
                                    select node;

            Random random = new Random();
            int choice = random.Next(all_posible_nodes.Count());
            Node next_node = all_posible_nodes.ElementAt(choice);

            Move(next_node);
        }
    }
}
