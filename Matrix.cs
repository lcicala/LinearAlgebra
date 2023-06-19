namespace LinearAlgebra
{
    public class Matrix
    {
        private double[,] m_Values;

        private bool[,] _stored;
        private Matrix[,] _associatedMatrices;
        private Matrix _inverseMatrix;
        private double? _determinant;

        private int _rows;
        private int _cols;

        public int Rows { get => _rows; }
        public int Columns { get => _cols; }

        public Matrix(int rows, int cols)
        {
            m_Values = new double[rows,cols];
            _stored = new bool[rows,cols];
            _associatedMatrices = new Matrix[rows,cols];
            _determinant = null;
            _inverseMatrix = null!;
            _rows = rows;
            _cols = cols;
        }

        public double this[Index i, Index j]
        {
            get
            {
                int row = i.IsFromEnd ? Rows - 1 - i.Value : i.Value;
                int col = j.IsFromEnd ? Columns - 1 - j.Value : j.Value;
                return m_Values[row, col];
            }
            set
            {
                int row = i.IsFromEnd ? Rows - 1 - i.Value : i.Value;
                int col = j.IsFromEnd ? Columns - 1 - j.Value : j.Value;
                _stored[row, col] = false;
                _inverseMatrix = null!;
                _determinant = null;
                m_Values[row, col] = value;
            }
        }

        public Vector this[Index i, Range r]
        {
            get
            {
                (var offset, var length) = r.GetOffsetAndLength(Columns);
                var result = new Vector(length);
                int row = i.IsFromEnd ? Rows - 1 - i.Value : i.Value;
                for (int j = 0; j < length; j++)
                {
                    result[j] = m_Values[row, offset + j];
                }
                return result;
            }
            set
            {
                (var offset, var length) = r.GetOffsetAndLength(Columns);
                int row = i.IsFromEnd ? Rows - 1 - i.Value : i.Value;
                for (int j = 0; j < length; j++)
                {
                    _stored[row, offset + j] = false;
                    m_Values[row, offset + j] = value[j];
                }
                _inverseMatrix = null!;
                _determinant = null;
            }
        }

        public Vector this[Range r, Index j]
        {
            get
            {
                (var offset, var length) = r.GetOffsetAndLength(Rows);
                var result = new Vector(length);
                int col = j.IsFromEnd ? Columns - 1 - j.Value : j.Value;
                for (int i = 0; i < length; i++)
                {
                    result[i] = m_Values[offset + i, col];
                }
                return result;
            }
            set
            {
                (var offset, var length) = r.GetOffsetAndLength(Rows);
                int col = j.IsFromEnd ? Columns - 1 - j.Value : j.Value;
                for (int i = 0; i < length; i++)
                {
                    _stored[offset + i, col] = false;
                    m_Values[offset + i, col] = value[i];
                }
                _inverseMatrix = null!;
                _determinant = null;
            }
        }

        public Matrix this[Range ri, Range rj]
        {
            get
            {
                (var row_offset, var row_length) = ri.GetOffsetAndLength(Rows);
                (var col_offset, var col_length) = rj.GetOffsetAndLength(Columns);
                var result = new Matrix(row_length, col_length);
                for (int i = 0; i < row_length; i++)
                {
                    for (int j = 0; j < col_length; j++)
                    {
                        result[i, j] = m_Values[row_offset + i, col_offset + j];
                    }
                }
                return result;
            }
            set
            {
                (var row_offset, var row_length) = ri.GetOffsetAndLength(Rows);
                (var col_offset, var col_length) = rj.GetOffsetAndLength(Columns);
                for (int i = 0; i < row_length; i++)
                {
                    for (int j = 0; j < col_length; j++)
                    {
                        m_Values[row_offset + i, col_offset + j] = value[i, j];
                    }
                }
            }
        }

        public Matrix GetAssociatedMatrix(int row, int col)
        {
            if (_stored[row, col])
                return _associatedMatrices[row, col];
            var result = new Matrix(Rows - 1, Columns - 1);
            for (int i = 0, k = 0; i < Rows; i++)
            {
                if (i == row)
                    continue;
                for (int j = 0, h = 0; j < Columns; j++)
                {
                    if (j == col)
                        continue;
                    result[k, h] = m_Values[i, j];
                    h++;
                }
                k++;
            }
            _stored[row, col] = true;
            _associatedMatrices[row, col] = result;
            return result;
        }

        public double Determinant()
        {
            if (_determinant != null)
                return _determinant.Value;
            if (Rows != Columns) return 0;
            if (Rows == 0) return 0;
            if (Rows == 1) return this[0, 0];
            if (Rows == 2) return this[0, 0] * this[1, 1] - this[1, 0] * this[0, 1];
            double det = 0;
            if(Rows == 3)
            {
                for (int k = 0; k < Columns; k++)
                {
                    double tmp = 1;
                    double tmp2 = 1;
                    for (int i = 0; i < Rows; i++)
                    {
                        //for (int j = 0; j < Columns; j++)
                        //{
                        tmp *= this[i, (k + i) % Columns];
                        tmp2 *= this[i, ^((i + k) % Columns)];
                        //}
                    }
                    det += tmp;
                    det -= tmp2;
                }
                return det;
            }
            for (int i = 0; i < Columns; i++)
            {
                det += Math.Pow(-1, i) * this[0, i] * this.GetAssociatedMatrix(0, i).Determinant();
            }
            _determinant = det;
            return det;
        }

        public double DeterminantC()
        {
            //if (_determinant != null)
            //    return _determinant.Value;
            if (Rows != Columns) return 0;
            if (Rows == 0) return 0;
            if (Rows == 1) return this[0, 0];
            if (Rows == 2) return this[0, 0] * this[1, 1] - this[1, 0] * this[0, 1];
            double det = 1;
            var tmp = RowsReduction(this);
            for (int i = 0; i < Rows; i++)
            {
                if (tmp[i, i] == 0)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        if (tmp[i, j] == 1)
                        {
                            tmp = tmp.SwapRows(i, j);
                            tmp[i, i] *= -1;
                            break;
                        }
                    }
                }
            }
            for (int i = 0; i < Columns; i++)
            {
                det *= tmp[i, i];
            }
            return det;
        }

        public static Matrix RowsReduction(Matrix matrix)
        {
            Matrix result = matrix[.., ..];
            int j;
            List<int> used = new List<int>();
            for (int i = 0; i < matrix.Rows-1; i++)
            {
                for (j = 0; j < matrix.Rows; j++)
                {
                    if (result[j, i] != 0 && !used.Contains(j))
                    {
                        used.Add(j);
                        break;
                    }
                }
                for (int k = 0; k < matrix.Rows; k++)
                {
                    if (j == k)
                        continue;
                    result[k, ..] = result[k, ..] - ((result[k, i] / result[j, i]) * result[j, ..]);
                    result[k, i] = 0;
                }
                //result[j, ..] = result[j, ..] * (1 / result[j, i]);
                //if (i != j)
                //{
                //    result = result.SwapRows(i, j);
                //    used.Add(i);
                //}
                //else
                //    used.Add(j);
            }
            return result;
        }

        public Matrix Inverse()
        {
            if (_inverseMatrix != null)
                return _inverseMatrix;
            double det = Determinant();
            if (det == 0)
                return null!;
            var result = new Matrix(Rows, Columns);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    result[i, j] = Math.Pow(-1, i + j) / det * this.GetAssociatedMatrix(i, j).Determinant();
                }
            }
            _inverseMatrix = result.Transpose();
            return _inverseMatrix;
        }

        public static Matrix Identity(int dim)
        {
            var result = new Matrix(dim, dim);
            for(int i = 0; i < dim; i++)
            {
                result[i, i] = 1;
            }
            return result;
        }

        public Matrix InverseGauss()
        {
            var tmp = new Matrix(Rows, Columns*2);
            tmp[.., Columns..] = Identity(Columns);
            tmp[.., ..Columns] = this;
            tmp = Gauss(tmp);
            //for (int i = 0; i < Rows; i++)
            //{
            //    if (tmp[i, i] != 1)
            //    {
            //        for (int j = 0; j < Columns; j++)
            //        {
            //            if (tmp[i, j] == 1)
            //            {
            //                tmp = tmp.SwapRows(i, j);
            //                break;
            //            }
            //        }
            //    }
            //}
            return tmp[.., Columns..];
        }

        private Matrix SwapRows(int r1, int r2)
        {
            var result = this[.., ..];
            result[r1,..] = this[r2,..];
            result[r2,..] = this[r1,..];
            return result;
        }

        public static Matrix Gauss(Matrix m)
        {
            Matrix result = m[.., ..];
            int j;
            List<int> used = new List<int>();
            for (int i = 0; i < m.Rows; i++)
            {
                for (j = 0; j < m.Rows; j++)
                {
                    if (result[j, i] != 0 && !used.Contains(j))
                    {
                        //used.Add(j);
                        break;
                    }
                }
                for (int k = 0; k < m.Rows; k++)
                {
                    if (j == k)
                        continue;
                    result[k, ..] = result[k, ..] - ((result[k, i] / result[j, i]) * result[j, ..]);
                    result[k, i] = 0;
                }
                result[j, ..] = result[j, ..] * (1 / result[j, i]);
                if (i != j)
                {
                    result = result.SwapRows(i, j);
                    used.Add(i);
                }
                else
                    used.Add(j);
            }
            return result;
        }

        public Matrix Transpose()
        {
            Matrix result = new Matrix(Columns, Rows);
            for(int i = 0; i < Columns; i++)
            {
                result[i, ..] = this[.., i];
            }
            return result;
        }

        public override string ToString()
        {
            string result = "";
            for(int i = 0; i < Rows; i++)
            {
                for(int j = 0; j < Columns; j++)
                {
                    result += this[i, j].ToString() + " ";
                }
                result += "\n";
            }
            return result;
        }

        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if (m1.Rows != m2.Rows || m1.Columns != m2.Columns)
                throw new ArgumentException();
            Matrix result = new Matrix(m1.Rows, m1.Columns);
            for (int i = 0; i < m1.Rows; i++)
            {
                for(int j = 0; j < m1.Columns; j++)
                {
                    result[i, j] = m1[i, j] + m2[i, j];
                }
            }
            return result;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.Columns != m2.Rows)
                throw new ArgumentException();
            Matrix result = new Matrix(m1.Rows, m2.Columns);
            for (int i = 0; i < m1.Rows; i++)
            {
                for (int j = 0; j < m1.Columns; j++)
                {
                    result[i, j] = m1[i, ..] * m2[.., j];
                }
            }
            return result;
        }

        public static Matrix operator !(Matrix m)
        {
            return m.InverseGauss();
        }
    }
}