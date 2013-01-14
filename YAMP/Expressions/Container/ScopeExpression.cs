﻿/*
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
using System.Collections.Generic;
using System.Text;

namespace YAMP
{
    /// <summary>
    /// This class represents the scope of a function.
    /// </summary>
    class ScopeExpression : Expression
    {
        #region ctor

        public ScopeExpression()
		{
		}

        public ScopeExpression(int line, int column, int length, QueryContext scope)
            : base(scope.Parent, line, column)
		{
            Scope = scope;
            IsSingleStatement = true;
            Length = length;
		}

        #endregion

        #region Properties

        public QueryContext Scope { get; private set; }

        #endregion

        #region Methods

        public override Value Interpret(Dictionary<string, Value> symbols)
        {
            Scope.Interpret(symbols);
            return Scope.Output;
        }

        public override Expression Scan(ParseEngine engine)
        {
            var start = engine.Pointer;
            var chars = engine.Characters;

            if (chars[start] == '{')
            {
                var index = start;
                var line = engine.CurrentLine;
                var column = engine.CurrentColumn;
                engine.Advance();
                index++;
                var scope = new QueryContext(engine.Query);
                var eng = scope.Parser;
                eng.Reset(scope.Input.Substring(index))
                    .SetOffset(line, column + 1)
                    .Parse();

                if (!eng.IsTerminated)
                    engine.AddError(new YAMPScopeNotClosedError(line, column));

                foreach (var error in eng.Errors)
                    engine.AddError(error);

                engine.Advance(eng.Pointer);
                return new ScopeExpression(line, column, engine.Pointer - start, scope);
            }

            return null;
        }

        #endregion

        #region String Representations

        public override string ToCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");

            foreach (var statement in Scope.Parser.Statements)
                sb.Append("\t").Append(statement.Container.ToCode()).AppendLine(";");

            sb.Append("}");
            return sb.ToString();
        }

        #endregion
    }
}
