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

namespace YAMP
{
	/// <summary>
	/// Provides a kind attribute to be read by the help method. This attribute specifies the kind of function / constant that is declared.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
	public class KindAttribute : Attribute
	{
        /// <summary>
        /// Creates a new attribute for storing the kind of a function.
        /// </summary>
		/// <param name="kind">The kind to store.</param>
		public KindAttribute(string kind)
        {
			Kind = kind;
        }

		/// <summary>
		/// Creates a new attribute for storing the kind of a function.
		/// </summary>
		/// <param name="kind">The kind to store.</param>
		public KindAttribute(PopularKinds kind) : this(kind.ToString())
		{
		}

        /// <summary>
        /// Gets the kind.
        /// </summary>
        public string Kind { get; private set; }
	}

	public enum PopularKinds
	{
		Function,
		Plot,
		System,
		Constant
	}
}
