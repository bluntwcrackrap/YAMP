using System;
using System.Collections;

namespace YAMP
{
	public abstract class Operator : IRegisterToken
	{
		string _op;
		int _level;
		
		public Operator (string op) : this(op, 0)
		{
		}
		
		public Operator (string op, int level)
		{
			_op = op;
			_level = level;
		}
		
		public int Level
		{
			get { return _level; }
		}
		
		public virtual string Set(string input)
		{
			return input.Substring(_op.Length);
		}
		
		public abstract Value Perform(Value left, Value right);

        public virtual Value Handle(AbstractExpression left, AbstractExpression right, Hashtable symbols)
        {
            var l = left.Interpret(symbols);
            var r = right.Interpret(symbols);
            return Perform(l, r);
        }

		#region IRegisterToken implementation
		
		public void RegisterToken ()
		{
			Tokens.Instance.AddOperator(_op, GetType());
		}
		
		#endregion
		
		public override string ToString ()
		{
			return string.Format ("{0} [ Operator: Level = {1} ]", _op, _level);
		}
	}
}

