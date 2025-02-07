using System;
using System.Collections.Generic;

namespace TicTacToeObjects
{
    /// <summary>
    /// A generic class representing a 2D matrix of elements of type T.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements stored in the matrix. Must be a value type (struct).
    /// </typeparam>
    public class Matrix<T> : ICloneable, IDisposable where T : struct
    {
        protected T[,] _data;
        int _height;
        int _width;

        /// <summary>
        /// Initializes a matrix with the specified elements.
        /// </summary>
        /// <param name="elements">A 2D array of elements to initialize the matrix.</param>
        public Matrix(T[,] elements)
        {
            Data = elements;
        }

        /// <summary>
        /// Initializes a new matrix by copying data from another matrix.
        /// </summary>
        /// <param name="other">The matrix to copy.</param>
        public Matrix(Matrix<T> other)
        {
            Data = other.DataPointer;
        }

        /// <summary>
        /// Initializes a matrix of given dimensions and fills it with the specified value.
        /// </summary>
        /// <param name="element">The value to fill the matrix with.</param>
        /// <param name="height">Number of rows.</param>
        /// <param name="width">Number of columns.</param>
        public Matrix(T element, int height, int width)
        {
            _height = height;
            _width = width;
            _data = new T[_height, _width];
            Map(t => element);
        }

        /// <summary>
        /// Gets the number of rows in the matrix.
        /// </summary>
        public int Height { get => _height; }

        /// <summary>
        /// Gets the number of columns in the matrix.
        /// </summary>
        public int Width { get => _width; }

        /// <summary>
        /// Gets or sets the matrix data as a 2D array.
        /// </summary>
        public T[,] Data {
            get
            {
                T[,] rep = new T[_height, _width];
                for (int i = 0; i < _height; i++)
                    for (int j = 0; j < _width; j++)
                        rep[i, j] = this[i, j];
                return rep;
            }
            set
            {
                _height = value.GetLength(0);
                _width = value.GetLength(1);
                _data = new T[_height, _width];
                for (int i = 0; i < _height; i++)
                    for (int j = 0; j < _width; j++)
                        _data[i, j] = value[i, j];
            }
        }

        /// <summary>
        /// Gets a reference to the internal data storage.
        /// </summary>
        public T[,] DataPointer { get => _data; }

        /// <summary>
        /// Accesses matrix elements by row and column indices.
        /// </summary>
        /// <param name="i">Row index.</param>
        /// <param name="j">Column index.</param>
        /// <returns>The element at the specified position.</returns>
        virtual public T this[int i, int j]
        {
            get
            {
                if (_height <= i || i < 0 || _width <= j || j < 0)
                    throw new IndexOutOfRangeException();
                return _data[i, j];
            }
            set
            {
                if (_height <= i || i < 0 || _width <= j || j < 0)
                    throw new IndexOutOfRangeException();
                _data[i, j] = value;
            }
        }

        /// <summary>
        /// Accesses matrix elements using a single index (row-major order).
        /// </summary>
        /// <param name="index">Index of the element.</param>
        public T this[int index]
        {
            get
            {
                int i = index / _width;
                int j = index % _width;
                return this[i, j];
            }
            set
            {
                int i = index / _width;
                int j = index % _width;

                this[i, j] = value;
            }
        }

        /// <summary>
        /// Accesses matrix elements using a vector position.
        /// </summary>
        /// <param name="position">The position vector.</param>
        public T this[Vector possition] 
        { 
            get => this[possition.Y, possition.X];
            set => this[possition.Y, possition.X] = value;
        }

        /// <summary>
        /// Applies a transformation function to all elements of the matrix.
        /// </summary>
        public void Map(Func<T, T> f)
        {
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    this[i, j] = f(this[i, j]);
        }

        /// <summary>
        /// Applies a transformation function to all elements of the matrix.
        /// </summary>
        public void All(Action<T> action)
        {
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    action(this[i, j]);
        }

        /// <summary>
        /// Finds the index of the first occurrence of a value in the matrix.
        /// </summary>
        public int Find(T value, Func<T, T, bool> function = null)
        {
            if (function == null)
                function = (x, y) => EqualityComparer<T>.Default.Equals(x, y);

            for (int i = 0; i < _height * _width; i++)
                if (function(this[i], value))
                    return i;
            return -1;
        }

        /// <summary>
        /// Gets the maximum value in the matrix.
        /// </summary>
        public T Max(Func<T, T, bool> function = null)
        {
            if (function == null)
                function = (x, y) => Comparer<T>.Default.Compare(x, y) < 0;

            T rep = this[0, 0];
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    if (function(rep, this[i, j]))
                        rep = this[i, j];
            return rep;
        }

        /// <summary>
        /// Gets the minimum value in the matrix.
        /// </summary>
        public T Min(Func<T, T, bool> function = null)
        {
            if (function == null)
                function = (x, y) => Comparer<T>.Default.Compare(x, y) > 0;
            else
                function = (x, y) => (function(y, x));
            return Max(function);
        }

        /// <summary>
        /// Counts the number of elements in the matrix that satisfy the given predicate.
        /// </summary>
        /// <param name="predicate">A function that defines the condition for counting elements.</param>
        /// <returns>The number of elements that match the condition.</returns>
        public int Count(Predicate<T> predicate)
        {
            int rep = 0;
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    if (predicate(_data[i, j]))
                        rep++;
            return rep;
        }

        /// <summary>
        /// Replaces the first occurrence of a specified value in the matrix with a new value.
        /// If multiple occurrences exist, it replaces the N-th occurrence based on the given number.
        /// </summary>
        /// <param name="value">The value to search for in the matrix.</param>
        /// <param name="new_value">The new value to replace the found value with.</param>
        /// <param name="number">The occurrence index to replace (0 for the first, 1 for the second, etc.).</param>
        public void ReplaceFirst(T value, T new_value, int number = 0)
        {
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                {
                    if (EqualityComparer<T>.Default.Equals(_data[i, j], value))
                    {

                        if (number == 0)
                        {
                            _data[i, j] = new_value;
                            return;
                        }
                        else
                            number--;
                    }
                }
        }

        /// <summary>
        /// Replaces all occurrences of a specified value in the matrix with a new value.
        /// </summary>
        /// <param name="value">The value to search for in the matrix.</param>
        /// <param name="new_value">The new value to replace the found value with.</param>
        public void Replace(T value, T new_value)
        {
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    if (EqualityComparer<T>.Default.Equals(_data[i, j], value))
                            _data[i, j] = new_value;
        }

        /// <summary>
        /// Determines whether two matrices are equal by comparing their dimensions and elements.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>True if the matrices have the same dimensions and all elements are equal; otherwise, false.</returns>
        public static bool operator ==(Matrix<T> a, Matrix<T> b)
        {
            if (a.Width != b.Width || a.Height != b.Height)
                return false;
            for (int i = 0; i < a.Height; i++)
                for (int j = 0; j < a.Width; j++)
                    if (!EqualityComparer<T>.Default.Equals(a[i, j], b[i, j]))
                        return false;
            return true;
        }

        /// <summary>
        /// Determines whether two matrices are not equal.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>True if the matrices are not equal; otherwise, false.</returns>
        public static bool operator !=(Matrix<T> a, Matrix<T> b) => !(a == b);

        /// <summary>
        /// Determines the reflection type required to transform this matrix into another matrix.
        /// </summary>
        /// <param name="other">The target matrix to compare against.</param>
        /// <returns>The reflection type (if any) that transforms this matrix into the other matrix. 
        /// Returns <see cref="MatrixReflexion.Null"/> if no valid reflection exists.</returns>
        public MatrixReflexion GetReflexionTo(Matrix<T> other)
        {
            foreach (MatrixReflexion reflexion in Enum.GetValues(typeof(MatrixReflexion)))
            {
                if (reflexion == MatrixReflexion.Null)
                    continue;
                if (other == new Reflexion<T>(reflexion, this))
                    return reflexion;
            }
            return MatrixReflexion.Null;
        }

        /// <summary>
        /// Determines the reflection type required to transform the other matrix into this matrix.
        /// This is simply the inverse operation of <see cref="GetReflexionTo"/>.
        /// </summary>
        /// <param name="other">The matrix to compare against.</param>
        /// <returns>The reflection type (if any) that transforms the other matrix into this matrix. 
        /// Returns <see cref="MatrixReflexion.Null"/> if no valid reflection exists.</returns>
        public MatrixReflexion GetReflexionFrom(Matrix<T> other) => other.GetReflexionTo(this);

        /// <summary>
        /// Checks if the current matrix is similar to another matrix.
        /// Two matrices are considered similar if one can be transformed into the other via a valid reflection.
        /// </summary>
        /// <param name="other">The matrix to compare against.</param>
        /// <returns><c>true</c> if the matrices are similar (i.e., they can be made identical through a valid reflection); 
        /// otherwise, <c>false</c>.</returns>
        public bool Similar(Matrix<T> other) => GetReflexionTo(other) != MatrixReflexion.Null;

        /// <summary>
        /// Rotates the matrix according to the specified reflection type (rotation or flip).
        /// This method only works if the matrix is square (height equals width).
        /// </summary>
        /// <param name="reflexion">The reflection type that defines the rotation or flip (e.g., 90 degrees clockwise, vertical flip, etc.).</param>
        public void Rotate(MatrixReflexion reflexion)
        {
            if (_height != _width)
                return;

            var rotated = new Reflexion<T>(reflexion, this);
            Data = rotated.Data;
        }

        public override string ToString()
        {
            T max_item = Max((x, y) => (Convert.ToString(x).Length < Convert.ToString(y).Length));
            int item_size = Convert.ToString(max_item).Length;
            string rep = "";

            for (int i = 0; i < _height; i++)
            {
                rep += "| ";
                for (int j = 0; j < _width; j++)
                    rep += Convert.ToString(this[i, j]).PadRight(item_size + 1);
                rep += '|';
                if (i < _height - 1)
                    rep += '\n';
            }

            return rep;
        }
        public override int GetHashCode() => ToString().GetHashCode();
        public override bool Equals(object obj) 
        { 
            if (obj is Matrix<T> other)
                return this == other;
            return false;
        }

        public object Clone() => new Matrix<T>(_data);

        public virtual void Dispose()
        {
            _data = null;
            GC.SuppressFinalize(this);
        }
    }

    public enum MatrixReflexion
    {
        VERTICAL = 0b001,
        HORISONTAL = 0b010,
        SYMETRIC = 0b100,
        None = 0b000,
        Null = 0b1000
    }

    /// <summary>
    /// Represents a reflection or transformation applied to a matrix, such as rotation or flip.
    /// </summary>
    /// <typeparam name="T">The type of the matrix elements (must be a value type).</typeparam>
    public class Reflexion<T> : Matrix<T> where T : struct
    {
        MatrixReflexion _reflexion;

        public Reflexion(MatrixReflexion reflexion, Matrix<T> matrix) : base(matrix)
        {
            _reflexion = reflexion;
        }

        public override T this[int i, int j]
        {
            get
            {
                if (Height <= i || i < 0 || Width <= j || j < 0)
                    throw new IndexOutOfRangeException();

                if ((_reflexion & MatrixReflexion.SYMETRIC) == MatrixReflexion.SYMETRIC)
                {
                    int tmp = i;
                    i = j;
                    j = tmp;
                }
                if ((_reflexion & MatrixReflexion.VERTICAL) == MatrixReflexion.VERTICAL)
                    i = Height - 1 - i;
                if ((_reflexion & MatrixReflexion.HORISONTAL) == MatrixReflexion.HORISONTAL)
                    j = Height - 1 - j;
        
                return _data[i, j];
            }
            set
            {
                if (Height <= i || i < 0 || Width <= j || j < 0)
                    throw new IndexOutOfRangeException();
                _data[i, j] = value;
            }
        }
    }

    public class Vector
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString() => $"Vector ({X}:{Y})";
    }
}
