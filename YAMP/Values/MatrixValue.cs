using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace YAMP
{
	public class MatrixValue : Value
	{
		ScalarValue[,] _values;
		
		int capacityX;
		int dimX;
		
		public int DimensionX
		{
			get { return dimX; }
		}
		
		int capacityY;
		int dimY;
		
		public int DimensionY
		{
			get { return dimY; }
		}
		
		public int Length
		{
			get { return dimY * dimX; }
		}
		
		public MatrixValue ()
		{
			dimX = 0;
			dimY = 0;
			capacityX = 32;
			capacityY = 32;
			_values = new ScalarValue[capacityX, capacityY];
			
			for(int i = 0; i < capacityX; i++)
				for(int j = 0; j < capacityY; j++)
					_values[i, j] = new ScalarValue();
		}
		
		public MatrixValue(int rows, int cols) : this()
		{
			this[rows, cols] = new ScalarValue();
		}
		
		public static MatrixValue Create(Value value)
		{
			if(value is MatrixValue)
				return value as MatrixValue;
			else if(value is ScalarValue)
			{
				var m = new MatrixValue();
				m[1, 1] = value as ScalarValue;
				return m;
			}
			
			throw new ArgumentException("matrix");
		}

        public static MatrixValue One(int dimension)
        {
            var m = new MatrixValue(dimension, dimension);

            for (var i = 1; i <= dimension; i++)
                m[i, i] = new ScalarValue(1.0);

            return m;
        }

        public MatrixValue Clone()
        {
            var m = new MatrixValue();
			m._values = _values.Clone() as ScalarValue[,];
			m.dimX = dimX;
			m.dimY = dimY;
            return m;
        }
		
		public MatrixValue AddColumn(Value value)
		{
			if(value is MatrixValue)
			{
				var t = value as MatrixValue;				
				int j, i = 1;
				
				for(var k = 1; k <= t.DimensionY; k++)
				{
					j = DimensionX + 1;
					
					for(var l = 1; l <= t.DimensionX; l++)
						this[i, j++] = t[k, l];
					
					i++;
				}
				
				return this;
			}
			else if(value is ScalarValue)
			{
				var t = value as ScalarValue;
				this[1, DimensionX + 1] = t;
				return this;
			}
			
			throw new OperationNotSupportedException(",", value);
		}
		
		public MatrixValue AddRow(Value value)
		{
			if(value is MatrixValue)
			{
				var t = value as MatrixValue;				
				int j, i = DimensionY + 1;
				
				for(var k = 1; k <= t.DimensionY; k++)
				{
					j = 1;
					
					for(var l = 1; l <= t.DimensionX; l++)
						this[i, j++] = t[k, l];
					
					i++;
				}
				
				return this;
			}
			else if(value is ScalarValue)
			{
				var t = value as ScalarValue;
				this[DimensionY + 1, 1] = t;
				return this;
			}
			
			throw new OperationNotSupportedException(";", value);
		}
	
		public override Value Add (Value right)
		{
			if(right is MatrixValue)
			{
				var r = right as MatrixValue;
				
				if(r.DimensionX != DimensionX)
					throw new DimensionException(DimensionX, r.DimensionX);
				
				if(r.DimensionY != DimensionY)
					throw new DimensionException(DimensionY, r.DimensionY);
				
                var m = new MatrixValue(DimensionY, DimensionX);
				
				for(var j = 1; j <= DimensionY; j++)
					for(var i = 1; i <= DimensionX; i++)
						m[j, i] = this[j, i].Add(r[j, i]) as ScalarValue;				
				
				return m;
			}
            else if (right is ScalarValue)
            {
                var m = new MatrixValue(DimensionY, DimensionX);

				for(var j = 1; j <= DimensionY; j++)
					for(var i = 1; i <= DimensionX; i++)
						m[j, i] = this[j, i].Add(right) as ScalarValue;

                return m;
            }
			
			throw new OperationNotSupportedException("+", right);
		}
		
		public override Value Power (Value exponent)
		{
            if (DimensionX != DimensionY)
                throw new DimensionException(DimensionX, DimensionY);

            if (exponent is ScalarValue)
            {
                if (DimensionX == 1)
                    return this[1, 1].Power(exponent);

                var exp = exponent as ScalarValue;
                
                if(exp.ImaginaryValue != 0.0 || Math.Floor(exp.Value) != exp.Value)
                    throw new OperationNotSupportedException("^", exponent);

                var eye = MatrixValue.One(DimensionX);
                var multiplier = this;
                var count = (int)Math.Abs(exp.Value);

                if (exp.Value < 0)
                    multiplier = this.Inverse();

                for (var i = 0; i < count; i++)
                    eye = eye.Multiply(multiplier) as MatrixValue;

                return eye;
            }

            throw new OperationNotSupportedException("^", exponent);
		}
		
		public override Value Subtract (Value right)
		{
			if(right is MatrixValue)
			{
				var r = right as MatrixValue;
				
				if(r.DimensionX != DimensionX)
					throw new DimensionException(DimensionX, r.DimensionX);
				
				if(r.DimensionY != DimensionY)
					throw new DimensionException(DimensionY, r.DimensionY);
				
                var m = new MatrixValue(DimensionY, DimensionX);
				
				for(var j = 1; j <= DimensionY; j++)
					for(var i = 1; i <= DimensionX; i++)
						m[j, i] = this[j, i].Subtract(r[j, i]) as ScalarValue;				
				
				return m;
			}
            else if (right is ScalarValue)
            {
                var m = new MatrixValue(DimensionY, DimensionX);

				for(var j = 1; j <= DimensionY; j++)
					for(var i = 1; i <= DimensionX; i++)
						m[j, i] = this[j, i].Subtract(right) as ScalarValue;

                return m;
            }
			
			throw new OperationNotSupportedException("-", right);
		}

		public override Value Multiply (Value right)
		{	
			if(right is MatrixValue)
			{
				var A = this;
				var B = right as MatrixValue;
				
				if(A.DimensionX != B.DimensionY)
					throw new DimensionException(A.DimensionX, B.DimensionY);
				
				var M = new MatrixValue(A.DimensionY, B.DimensionX);
				
				for(var j = 1; j <= B.DimensionX; j++)
				{					
					for(var i = 1; i <= A.DimensionY; i++)
					{						
						for(var k = 1; k <= A.DimensionX; k++)
							M[i, j] = M[i, j].Add(A[i, k].Multiply(B[k, j])) as ScalarValue;
						
						if(A.DimensionY == B.DimensionX && A.DimensionY == 1)
							return M[1, 1];
					}
				}
				
				return M;
			}
			else if(right is ScalarValue)
			{
                var A = new MatrixValue(DimensionY, DimensionX);

				for(var i = 1; i <= DimensionX; i++)
					for(var j = 1; j <= DimensionY; j++)
						A[j, i] = this[j, i].Multiply(right) as ScalarValue;

                return A;
			}
			
			throw new OperationNotSupportedException("*", right);
		}

		public override Value Divide (Value denominator)
		{
			if(denominator is ScalarValue)
			{
                var m = new MatrixValue(DimensionY, DimensionX);
				
				for(var j = 1; j <= DimensionY; j++)
					for(var i = 1; i <= DimensionX; i++)
						m[j, i] = this[j, i].Divide(denominator) as ScalarValue;
				
				return m;
			}
			else if (denominator is MatrixValue)
			{
				var Q = denominator as MatrixValue;
				
				if(DimensionX != Q.DimensionX)
					throw new DimensionException(DimensionX, Q.DimensionX);
				
				return this.Multiply(Q.Inverse());
			}
			
			throw new OperationNotSupportedException("/", denominator);
		}
		
		public MatrixValue Inverse()
		{
			//TODO
			return this.Transpose();
		}
		
		public override string ToString ()
		{			
			var sb = new StringBuilder();
			
			for(var j = 1; j <= DimensionY; j++)
			{
				for(var i = 1; i <= DimensionX; i++)
				{
					sb.Append(this[j, i].ToString());
					
					if(i < DimensionX)
						sb.Append("\t");
				}
				
				if(j < DimensionY)
					sb.AppendLine();
			}
			
			return sb.ToString();
		}
		
		void Resize()
		{
			int newX = capacityX;
			int newY = capacityY;
			
			if(DimensionX > newX)
				newX = DimensionX * 2;
			
			if(DimensionY > newY)
				newY = DimensionY * 2;
			
			var tmp = new ScalarValue[newX, newY];
			
			for(int i = 0; i < newX; i++)
			{
				for(int j = 0; j < newY; j++)
				{
					if(i < capacityX && j < capacityY)
						tmp[i, j] = _values[i, j];
					else
						tmp[i, j] = new ScalarValue();
				}
			}
			
			_values = tmp;
			capacityX = newX;
			capacityY = newY;
		}
							
		public ScalarValue this[int j, int i]
		{
			get
			{
				if(i > DimensionX || i < 1 || j > DimensionY || j < 1)
					throw new ArgumentOutOfRangeException("Access in Matrix out of bounds.");
									
				return _values[i - 1, j - 1];
			}
			set
			{
				if(i < 1 || j < 1)
					throw new ArgumentOutOfRangeException("Access in Matrix out of bounds.");
				
				if(i > DimensionX)
					dimX = i;
				
				if(j > DimensionY)
					dimY = j;
				
				if(DimensionX > capacityX || DimensionY > capacityY)
					Resize();
									
				_values[i - 1, j - 1] = value;
			}
		}
		
		public ScalarValue this[int i]
		{
			get
			{
				if(i > Length || i < 1)
					throw new ArgumentOutOfRangeException("Access in Matrix out of bounds.");
				
				var row = (i - 1) % DimensionY + 1;
				var col = (i - 1) / DimensionY + 1;
				return this[row, col];
			}
			set
			{
				if(i < 1)
					throw new ArgumentOutOfRangeException("Access in Matrix out of bounds.");

                var row = (i - 1) % DimensionY + 1;
                var col = (i - 1) / DimensionY + 1;
				this[row, col] = value;
			}
		}
		
		public MatrixValue GetRowVector(int j)
		{
			var m = new MatrixValue(1, DimensionX);
			
			for(var i = 1; i <= DimensionX; i++)
				m[1, i] = this[j, i].Clone();
			
			return m;
		}
		
		public MatrixValue GetColumnVector(int i)
		{
			var m = new MatrixValue(DimensionY, 1);
			
			for(var j = 1; j <= DimensionY; j++)
				m[j, 1] = this[j, i].Clone();
			
			return m;
		}
		
		public MatrixValue Transpose()
		{
			var m = new MatrixValue(dimX, dimY);
			
			for(var i = 1; i <= DimensionY; i++)
				for(var j = 1; j <= DimensionX; j++)
					m[j, i] = this[i, j];
			
			return m;
		}
		
		public ScalarValue Abs()
		{
			var sum = 0.0;
			
			foreach(var p in _values)
			{
				sum += (p.Value * p.Value + p.ImaginaryValue * p.ImaginaryValue);
			}
			
			return new ScalarValue(Math.Sqrt(sum));
		}
		
		public ScalarValue Det()
		{
			if(DimensionX == DimensionY)
			{
				if(DimensionX == 1)
					return this[1, 1];
                else if (DimensionX == 2)
                    return this[1, 1] * this[2, 2] - this[1, 2] * this[2, 1];
			}
			
			return new ScalarValue(0.0);
		}

        public Value Index(Value right)
        {
            if (right is ScalarValue)
                return this[(int)(right as ScalarValue).Value];
            else if (right is MatrixValue)
            {
                var m = right as MatrixValue;

                if (m.DimensionX == 1)
                {
                    var r = new MatrixValue(m.DimensionY, 1);

                    for (var j = 1; j <= m.DimensionY; j++)
                        r[j] = this[(int)m[j].Value];

                    return r;
                }
                else if (m.DimensionX == 2)
                {
                    var r = new MatrixValue(m.DimensionY, 1);

                    for (var j = 1; j <= m.DimensionY; j++)
                        r[j] = this[(int)m[j, 1].Value, (int)m[j, 2].Value];

                    return r;
                }

                throw new DimensionException(2, m.DimensionX);
            }

            throw new OperationNotSupportedException("_", right);
        }
    }
}

