using System;
using System.Collections.Generic;

namespace TicTacToeObjects
{
    public enum MatrixReflexion
    {
        VERTICAL = 0b001,
        HORISONTAL = 0b010,
        SYMETRIC = 0b100,
        VH = VERTICAL | HORISONTAL,
        VS = VERTICAL | SYMETRIC,
        HS = HORISONTAL | SYMETRIC,
        None = 0b000,
        Null
    }


    public class Matrix<T> : ICloneable where T : struct
    {
        protected T[,] _data;
        int _height;
        int _width;

        public Matrix(T[,] elements)
        {
            Data = elements;
        }
        public Matrix(Matrix<T> other)
        {
            Data = other.DataPointer;
        }
        public Matrix(T element, int height, int width)
        {
            _height = height;
            _width = width;
            _data = new T[_height, _width];
            Map(t => element);
        }
        public int Height { get => _height; }
        public int Width { get => _width; }

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
        public T[,] DataPointer { get => _data; }
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
        public T this[Vector possition] 
        { 
            get => this[possition.Y, possition.X];
            set => this[possition.Y, possition.X] = value;
        }

        public void Map(Func<T, T> f)
        {
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    this[i, j] = f(this[i, j]);
        }
        public void All(Action<T> action)
        {
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    action(this[i, j]);
        }
        public int Find(T value, Func<T, T, bool> function = null)
        {
            if (function == null)
                function = (x, y) => EqualityComparer<T>.Default.Equals(x, y);

            for (int i = 0; i < _height * _width; i++)
                if (function(this[i], value))
                    return i;
            return -1;
        }
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
        public T Min(Func<T, T, bool> function = null)
        {
            if (function == null)
                function = (x, y) => Comparer<T>.Default.Compare(x, y) > 0;
            else
                function = (x, y) => (function(y, x));
            return Max(function);
        }
        public int Count(Predicate<T> predicate)
        {
            int rep = 0;
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    if (predicate(_data[i, j]))
                        rep++;
            return rep;
        }

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
        public static bool operator !=(Matrix<T> a, Matrix<T> b) => !(a == b);


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

        public MatrixReflexion GetReflexionFrom(Matrix<T> other) => other.GetReflexionTo(this);

        public bool Similar(Matrix<T> other) => GetReflexionTo(other) != MatrixReflexion.Null;

        public void Rotate(MatrixReflexion reflexion)
        {
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
        public override bool Equals(object obj) => GetHashCode() == obj.GetHashCode();

        public object Clone() => new Matrix<T>(_data);
    }

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

                if ((_reflexion & MatrixReflexion.VERTICAL) == MatrixReflexion.VERTICAL)
                    i = Height - 1 - i;
                if ((_reflexion & MatrixReflexion.HORISONTAL) == MatrixReflexion.HORISONTAL)
                    j = Height - 1 - j;
                if ((_reflexion & MatrixReflexion.SYMETRIC) == MatrixReflexion.SYMETRIC)
                {
                    int tmp = i;
                    i = j;
                    j = tmp;
                }

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
