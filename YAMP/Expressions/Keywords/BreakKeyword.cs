﻿/*
	Copyright (c) 2012-2014, Florian Rappl.
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

namespace YAMP
{
    /// <summary>
    /// This is the class responsible for the break keyword. Basic syntax:
    /// break;
    /// </summary>
    class BreakKeyword : Keyword
    {
        #region ctor

        public BreakKeyword()
            : base("break")
        {
        }

        public BreakKeyword(int line, int column, QueryContext query)
            : this()
        {
            Query = query;
            StartLine = line;
            StartColumn = column;
            Length = Token.Length;
        }

        #endregion

        #region Methods

        public override Expression Scan(ParseEngine engine)
        {
            var kw = new BreakKeyword(engine.CurrentLine, engine.CurrentColumn, engine.Query);
            engine.Advance(Token.Length);

            if (!IsBreakable(engine))
                engine.AddError(new YAMPKeywordNotPossible(engine, Token), kw);

            return kw;
        }

        public override Value Interpret(Dictionary<string, Value> symbols)
        {
            var ctx = GetBreakableContext(Query);
            ctx.CurrentStatement.GetKeyword<BreakableKeyword>().Break();
            return null;
        }

        #endregion

        #region String Representations

        public override string ToCode()
        {
            return Token;
        }

        #endregion

        #region Helpers

        bool IsBreakable(ParseEngine engine)
        {
            if (engine.HasMarker(Marker.Breakable))
                return true;

            if (engine.Parent != null)
                return IsBreakable(engine.Parent);

            return false;
        }

        QueryContext GetBreakableContext(QueryContext context)
        {
            if (context.CurrentStatement.IsKeyword<BreakableKeyword>())
                return context;

            context.Stop();
            return GetBreakableContext(context.Parent);
        }

        #endregion
    }
}
