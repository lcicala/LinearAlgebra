using System;
using System.Collections;
using System.Collections.Generic;

namespace LinearAlgebra
{
    public class Vector : IEnumerable<double>
    {
        protected double[] m_Values;
        protected int _dim;

        public int Dimension { get => _dim; }

        public double this[Index i]
        {
            get
            {
                return m_Values[i];
            }
            set
            {
                m_Values[i] = value;
            }
        }

        public Vector this[Range r]
        {
            get
            {
                (var offset, var length) = r.GetOffsetAndLength(Dimension);
                var result = new Vector(length);
                for (int i = 0; i < length; i++)
                {
                    result[i] = m_Values[offset + i];
                }
                return result;
            }
            set
            {
                (var offset, var length) = r.GetOffsetAndLength(Dimension);
                for (int i = 0; i < length; i++)
                {
                    m_Values[offset + i] = value[i];
                }
            }
        }


        public Vector(int dimension)
        {
            m_Values = new double[dimension];
            _dim = dimension;
        }

        public Vector(params double[] values)
        {
            this.m_Values = values;
            _dim = values.Length;
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < Dimension; i++)
            {
                result += this[i].ToString() + " ";
            }
            return result;
        }

        public IEnumerator<double> GetEnumerator()
        {
            for(int i = 0; i < Dimension; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static Vector operator ^(Vector v1, Vector v2)
        {
            if (v1.Dimension != v2.Dimension)
                throw new ArgumentException();
            Vector result = new Vector(v1.Dimension);
            for(int i = 0; i < v1.Dimension; i++)
            {
                result[i] = v1[i] * v2[i];
            }
            return result;
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            if (v1.Dimension != v2.Dimension)
                throw new ArgumentException();
            Vector result = new Vector(v1.Dimension);
            for (int i = 0; i < v1.Dimension; i++)
            {
                result[i] = v1[i] + v2[i];
            }
            return result;
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            if (v1.Dimension != v2.Dimension)
                throw new ArgumentException();
            Vector result = new Vector(v1.Dimension);
            for (int i = 0; i < v1.Dimension; i++)
            {
                result[i] = v1[i] - v2[i];
            }
            return result;
        }

        public static Vector operator *(Vector v, double d)
        {
            Vector result = new Vector(v.Dimension);
            for (int i = 0; i < v.Dimension; i++)
            {
                result[i] = d*v[i];
            }
            return result;
        }

        public static Vector operator *(double d, Vector v)
        {
            Vector result = new Vector(v.Dimension);
            for (int i = 0; i < v.Dimension; i++)
            {
                result[i] = d * v[i];
            }
            return result;
        }

        public static double operator *(Vector v1, Vector v2)
        {
            if (v1.Dimension != v2.Dimension)
                throw new ArgumentException();
            double result = 0;
            for (int i = 0; i < v1.Dimension; i++)
            {
                result += v1[i] * v2[i];
            }
            return result;
        }

        public static implicit operator Vector((double, double) v)
        {
            var ret = new Vector(2);
            ret[0] = v.Item1;
            ret[1] = v.Item2;
            return ret;
        }
    }
}
