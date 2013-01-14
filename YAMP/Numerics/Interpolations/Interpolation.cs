﻿using System;
using YAMP;

namespace YAMP.Numerics
{
    /// <summary>
    /// Abstract base class for various interpolation algorithms.
    /// </summary>
    public abstract class Interpolation
    {
        #region Members

        int np;
        MatrixValue _samples;

        #endregion

        #region ctor

        public Interpolation(MatrixValue samples)
        {
            _samples = samples;
            np = samples.DimensionY;

            if (samples.DimensionX != 2)
                new YAMPMatrixDimensionException(samples.DimensionY, 2, samples.DimensionY, samples.DimensionX);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the vector with samples.
        /// </summary>
        public MatrixValue Samples { get { return _samples; } }

        /// <summary>
        /// Gets the number of interpolation points.
        /// </summary>
        public int Np { get { return np; } }

        #endregion

        #region Methods

        /// <summary>
        /// Uses the abstract Compute() methods to compute ALL values.
        /// </summary>
        /// <param name="x">The matrix with given x values.</param>
        /// <returns>The interpolated y values.</returns>
        public virtual MatrixValue ComputeValues(MatrixValue x)
        {
            var y = new MatrixValue(x.DimensionY, x.DimensionX);

            for (var i = 1; i <= x.DimensionX; i++)
                for (var j = 1; j <= x.DimensionY; j++)
                    y[j, i].Value = ComputeValue(x[j, i].Value);

            return y;
        }

        /// <summary>
        /// Computes an interpolated y-value for the given x-value.
        /// </summary>
        /// <param name="x">The x-value to search for a y-value.</param>
        /// <returns>The corresponding y-value.</returns>
        public abstract double ComputeValue(double x);

        protected void SolveTridiag(double[] sub, double[] diag, double[] sup, ref double[] b, int n)
        {
            int i;

            for (i = 2; i <= n; i++)
            {
                sub[i] = sub[i] / diag[i - 1];
                diag[i] = diag[i] - sub[i] * sup[i - 1];
                b[i] = b[i] - sub[i] * b[i - 1];
            }

            b[n] = b[n] / diag[n];

            for (i = n - 1; i >= 1; i--)
                b[i] = (b[i] - sup[i] * b[i + 1]) / diag[i];
        }

        #endregion
    }
}
