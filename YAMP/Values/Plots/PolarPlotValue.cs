﻿/*
    Copyright (c) 2012, Florian Rappl.
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
using YAMP.Converter;

namespace YAMP
{
	public sealed class PolarPlotValue : PlotValue
	{
		#region ctor

		public PolarPlotValue()
		{
			FractionSymbol = "π";
			FractionUnit = Math.PI;
			Gridlines = true;
		}

		#endregion

		#region Properties

		[StringToStringConverter]
		public string FractionSymbol
		{
			get;
			set;
		}

		[ScalarToDoubleConverter]
		public double FractionUnit
		{
			get;
			set;
		}

		#endregion

        #region Methods

        public override void AddPoints(MatrixValue m)
        {
            if (m.DimensionY == 0 || m.DimensionX == 0)
                return;

            if (m.IsVector)
            {
                var x = Generate(0.0, 1.0, m.Length);
                Normalize(x, m.Length);
                var y = Convert(m, 0, m.Length);
                AddValues(x, y);
            }
            else if (m.DimensionX <= m.DimensionY)
            {
                var x = ConvertY(m, 0, m.DimensionY, 0);

                for (var k = 2; k <= m.DimensionX; k++)
                {
                    var y = ConvertY(m, 0, m.DimensionY, k - 1);
                    AddValues(x, y);
                }
            }
            else
            {
                var x = ConvertX(m, 0, m.DimensionX, 0);

                for (var k = 2; k <= m.DimensionY; k++)
                {
                    var y = ConvertX(m, 0, m.DimensionX, k - 1);
                    AddValues(x, y);
                }
            }
        }

        public void AddPoints(MatrixValue x, MatrixValue y)
        {
            if (x.IsVector)
            {
                var vx = Convert(x, 0, x.Length);

                if (y.DimensionY > y.DimensionX || y.DimensionY >= x.Length)
                {
                    var dim = Math.Min(x.Length, y.DimensionY);

                    for (var i = 0; i < y.DimensionX; i++)
                    {
                        var vy = ConvertY(y, 0, dim, i);
                        AddValues(vx, vy);
                    }
                }
                else
                {
                    var dim = Math.Min(x.Length, y.DimensionX);

                    for (var i = 0; i < y.DimensionY; i++)
                    {
                        var vy = ConvertX(y, 0, dim, i);
                        AddValues(vx, vy);
                    }
                }
            }
            else
            {
                AddPoints(x);
                AddPoints(y);
            }
        }

        public void AddPoints(MatrixValue x, MatrixValue y, params MatrixValue[] zs)
        {
            if (x.IsVector)
            {
                AddPoints(x, y);

                foreach (var z in zs)
                    AddPoints(x, z);
            }
            else
            {
                AddPoints(x);
                AddPoints(y);

                foreach (var z in zs)
                    AddPoints(z);
            }
        }

		void AddValues(double[] _x, double[] _y)
		{
			var p = new Points<PointPair>();
			MinX = 0.0;
			MaxX = 2.0 * Math.PI;
			var ymin = double.MaxValue;
			var ymax = double.MinValue;

			for (var i = 0; i < _y.Length; i++)
			{
				var x = _x[i];
				var y = _y[i];

				p.Add(new PointPair
				{
					Angle = x,
					Magnitude = y
				});

				if (y < ymin)
					ymin = y;

				if (ymax < y)
					ymax = y;
			}

			if (Count == 0 || ymin < MinY)
				MinY = ymin;

			if (Count == 0 || ymax > MaxY)
				MaxY = ymax;

			AddSeries(p);
		}

        void Normalize(double[] x, double max)
        {
            for (var i = 0; i < x.Length; i++)
                x[i] = x[i] / max;
        }

		#endregion

		#region Nested types

		public struct PointPair
		{
			public double Angle;
			public double Magnitude;
		}

		#endregion

        #region Serialization

        public override byte[] Serialize()
        {
            using (var s = Serializer.Create())
            {
                Serialize(s);
                s.Serialize(FractionSymbol);
                s.Serialize(FractionUnit);
                s.Serialize(Count);

                for (var i = 0; i < Count; i++)
                {
                    var points = this[i];
                    points.Serialize(s);
                    s.Serialize(points.Count);

                    for (int j = 0; j < points.Count; j++)
                    {
                        s.Serialize(points[j].Angle);
                        s.Serialize(points[j].Magnitude);
                    }
                }

                return s.Value;
            }
        }

        public override Value Deserialize(byte[] content)
        {
            using (var ds = Deserializer.Create(content))
            {
                Deserialize(ds);
                FractionSymbol = ds.GetString();
                FractionUnit = ds.GetDouble();
                var length = ds.GetInt();

                for (var i = 0; i < length; i++)
                {
                    var points = new Points<PointPair>();
                    points.Deserialize(ds);
                    var count = ds.GetInt();

                    for (int j = 0; j < count; j++)
                    {
                        var x = ds.GetDouble();
                        var y = ds.GetDouble();

                        points.Add(new PointPair
                        {
                            Angle = x,
                            Magnitude = y
                        });
                    }

                    AddSeries(points);
                }
            }

            return this;
        }

        #endregion

        #region Index

        public Points<PointPair> this[int index]
        {
            get
            {
                return base.GetSeries(index) as Points<PointPair>;
            }
        }

        public PointPair this[int index, int point]
        {
            get
            {
                return this[index][point];
            }
        }

        #endregion
	}
}
