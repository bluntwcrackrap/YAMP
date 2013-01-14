/*
	Copyright (c) 2012-2013, Florian Rappl.
	All rights reserved.

	Redistribution and use in source and binary forms, with or without
	modification, are permitted provided that the following conditions are met:
		* Redistributions of source code must retain the above copyright
		  notice, this list of conditions and the following disclaimer.
		* Redistributions in binary form must reproduce the above copyright
		  notice, this list of conditions and the following disclaimer in the
		  documentation and/or other materials provided with the distribution.
		* Neither the name of the YAMP team nor the names of its contributors
		  may be used to endorse or promote products derived from this
		  software without specific prior written permission.

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
	ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
	WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
	DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
	DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
	(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
	LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
	ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
	(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
	SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;

namespace YAMP
{
    /// <summary>
    /// The abstract base class for any dot operator (.*, ./, ...), which is essentially a special
    /// binary operator.
    /// </summary>
	public abstract class DotOperator : BinaryOperator
	{
		BinaryOperator _top;

		public DotOperator(BinaryOperator top) : base("." + top.Op, top.Level)
		{
			_top = top;
		}

		public abstract ScalarValue Operation(ScalarValue left, ScalarValue right);

		public override Value Perform(Value left, Value right)
		{
			if (!(left is NumericValue))
				throw new YAMPOperationInvalidException(Op, left);
			else if (!(right is NumericValue))
				throw new YAMPOperationInvalidException(Op, right);

			if (left is MatrixValue && right is MatrixValue)
			{
				var l = (MatrixValue)left;
				var r = (MatrixValue)right;
				return Dot(l, r);
			}
			else if (left is MatrixValue && right is ScalarValue)
			{
				var l = (MatrixValue)left;
				var r = (ScalarValue)right;
				return Dot(l, r);
			}
			else if (left is ScalarValue && right is MatrixValue)
			{
				var l = (ScalarValue)left;
				var r = (MatrixValue)right;
				return Dot(l, r);
			}

			return _top.Perform(left, right);
		}

		public MatrixValue Dot(MatrixValue left, MatrixValue right)
		{
            if (left.DimensionX != right.DimensionX || left.DimensionY != right.DimensionY)
                throw new YAMPDifferentDimensionsException(left, right);

			var m = new MatrixValue(left.DimensionY, left.DimensionX);

			for (var i = 1; i <= left.DimensionX; i++)
				for (var j = 1; j <= left.DimensionY; j++)
					m[j, i] = Operation(left[j, i], right[j, i]);

			return m;
		}

		public MatrixValue Dot(MatrixValue left, ScalarValue right)
		{
			var m = new MatrixValue(left.DimensionY, left.DimensionX);

			for (var i = 1; i <= left.DimensionX; i++)
				for (var j = 1; j <= left.DimensionY; j++)
					m[j, i] = Operation(left[j, i], right);

			return m;
		}

		public MatrixValue Dot(ScalarValue left, MatrixValue right)
		{
			var m = new MatrixValue(right.DimensionY, right.DimensionX);

			for (var i = 1; i <= right.DimensionX; i++)
				for (var j = 1; j <= right.DimensionY; j++)
					m[j, i] = Operation(left, right[j, i]);

			return m;
		}
	}
}

