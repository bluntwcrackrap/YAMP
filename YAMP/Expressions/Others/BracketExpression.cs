using System;
using System.Collections;
using System.Collections.Generic;
namespace YAMP
{
	class BracketExpression : AbstractExpression
	{
		ParseTree _tree;
		
		public BracketExpression () : base(@"\(.*\)")
		{
		}
		
		public BracketExpression(ParseTree tree) : this()
		{
			_tree = tree;
		}
		
		public ParseTree Tree
		{
			get { return _tree; }
		}
		
		public override Value Interpret (Hashtable symbols)
		{
            if (_tree.Operators.Length == 1)
                return _tree.Operators[0].Handle(_tree.Expressions[0], _tree.Expressions[1], symbols);
            else
                return _tree.Expressions[0].Interpret(symbols);
		}
		
		public override string Set (string input)
		{
			if(input[0] == '(')
			{			
				var brackets = 1;
				
				for(var i = 1; i < input.Length; i++)
				{
					if(input[i] == ')')
						brackets--;
					else if(input[i] == '(')
						brackets++;
					
					if(brackets == 0)
					{
						_input = input.Substring(1, i - 1);
						_tree = new ParseTree(_input);
						i++;
						return input.Length > i ? input.Substring(i) : string.Empty;
					}
				}
				
				throw new BracketException("(", input);
			}
			
			_input = input;
			_tree = new ParseTree(_input);
			return string.Empty;
		}
		
		public override string ToString ()
		{
			return _tree.ToString();
		}
}
}

