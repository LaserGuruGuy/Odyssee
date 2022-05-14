// <copyright file="OnlineFirFilter.cs" company="Math.NET">
// Math.NET Filtering, part of the Math.NET Project
// http://filtering.mathdotnet.com
// http://github.com/mathnet/mathnet-filtering
//
// Copyright (c) 2009-2014 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;

namespace MathNet.Filtering.FIR
{
    /// <summary>
    /// Finite Impulse Response (FIR) Filters are based on
    /// Fourier series and implemented using a discrete
    /// convolution equation. FIR Filters are always
    /// online, stable and causal.
    /// </summary>
    /// <remarks>
    /// System Description: H(z) = a0 + a1*z^-1 + a2*z^-2 + ...
    /// </remarks>
    public class OnlineFirFilter
    {
        private readonly double[] _Coefficients;
        private readonly double[] _Buffer;
        private readonly double _Gain;
        private readonly int _Size;
        private int _Offset;

        public double Gain { get { return 1.0/_Gain; } }

        /// <summary>
        /// Finite Impulse Response (FIR) Filter.
        /// The sum of the filter coefficients should be 1.
        /// To ensure a filter gain of 1 the gain is calculated.
        /// The reciproce is aplied when filtering to achieve unity gain.
        /// </summary>
        public OnlineFirFilter(IList<double> coefficients)
        {
            _Size = coefficients.Count;
            _Gain = 0;
            _Buffer = new double[_Size];
            _Coefficients = new double[_Size << 1];
            for (int i = 0; i < _Size; i++)
            {
                _Gain += coefficients[i];
                _Coefficients[i] = _Coefficients[_Size + i] = coefficients[i];
            }
            _Gain = 1.0d / _Gain;
        }

        /// <summary>
        /// Process a single sample.
        /// </summary>
        public double ProcessSample(double sample)
        {
            _Offset = (_Offset != 0) ? _Offset - 1 : _Size - 1;
            _Buffer[_Offset] = sample;

            double acc = 0;
            for (int i = 0, j = _Size - _Offset; i < _Size; i++, j++)
            {
                acc += _Buffer[i] * _Coefficients[j];
            }

            return _Gain * acc;
        }

        /// <summary>
        /// Reset internal state (not coefficients!).
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < _Buffer.Length; i++)
            {
                _Buffer[i] = 0d;
            }
            _Offset = 0;
        }
    }
}