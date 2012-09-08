using System;

namespace YAMP
{
	class TransposeOperator : UnaryOperator
	{
		public TransposeOperator () : base("'")
		{
		}
		
		public override Value Perform (Value left)
		{
			if(left is ScalarValue)
				return (left as ScalarValue).Conjugate();
			else if(left is MatrixValue)
				return (left as MatrixValue).Adjungate();
			
			throw new OperationNotSupportedException("'", left);
		}
	}
}

