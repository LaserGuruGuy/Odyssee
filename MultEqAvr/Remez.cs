/**************************************************************************
 * Parks-McClellan algorithm for FIR filter design (C version)
 *-------------------------------------------------
 *  Copyright (c) 1995,1998  Jake Janovetz <janovetz@uiuc.edu>
 *  https://github.com/janovetz/remez-exchange
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Library General Public
 *  License as published by the Free Software Foundation; either
 *  version 2 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Library General Public License for more details.
 *
 *  You should have received a copy of the GNU Library General Public
 *  License along with this library; if not, write to the Free
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 *
 *  Sep 1999 - Paul Kienzle (pkienzle@cs.indiana.edu)
 *      Modified for use in octave as a replacement for the matlab function
 *      remez.mex.  In particular, magnitude responses are required for all
 *      band edges rather than one per band, griddensity is a parameter,
 *      and errors are returned rather than printed directly.
 *  Mar 2000 - Kai Habel (kahacjde@linux.zrz.tu-berlin.de)
 *      Change: ColumnVector x=arg(i).vector_value();
 *      to: ColumnVector x(arg(i).vector_value());
 *  There appear to be some problems with the routine Search. See comments
 *  therein [search for PAK:].  I haven't looked closely at the rest
 *  of the code---it may also have some problems.
 *************************************************************************/

using MathNet.Numerics;
using System;
using System.Collections.Generic;

namespace Remez.Filtering
{
    public class ParksMcClellanFilter
    {
        const int BANDPASS = 1;
        const int DIFFERENTIATOR = 2;
        const int HILBERT = 3;

        const int NEGATIVE = 0;
        const int POSITIVE = 1;

        const int GRIDDENSITY = 16;
        const int MAXITERATIONS = 40;

        /*******************
         * CreateDenseGrid
         *=================
         * Creates the dense grid of frequencies from the specified bands.
         * Also creates the Desired Frequency Response function (D[]) and
         * the Weight function (W[]) on that dense grid
         *
         *
         * INPUT:
         * ------
         * int      r        - 1/2 the number of filter coefficients
         * int      numtaps  - Number of taps in the resulting filter
         * int      numband  - Number of bands in user specification
         * double   bands[]  - User-specified band edges [2*numband]
         * double   des[]    - Desired response per band [2*numband]
         * double   weight[] - Weight per band [numband]
         * int      symmetry - Symmetry of filter - used for grid check
         * int      griddensity
         * int      gridsize   - Number of elements in the dense frequency grid
         *
         * OUTPUT:
         * -------
         * double Grid[]     - Frequencies (0 to 0.5) on the dense grid [gridsize]
         * double D[]        - Desired response on the dense grid [gridsize]
         * double W[]        - Weight function on the dense grid [gridsize]
         *******************/

        public void CreateDenseGrid(int r, int numtaps, int numband, double[] bands, double[] des, double[] weight, int gridsize, ref double[] Grid, ref double[] D, ref double[] W, int symmetry, int griddensity)
        {
            int i, j, k, band;
            double delf, lowf, highf, grid0;

            delf = 0.5 / (griddensity * r);

            /*
             * For differentiator, hilbert,
             *   symmetry is odd and Grid[0] = max(delf, bands[0])
             */
            grid0 = (symmetry == NEGATIVE) && (delf > bands[0]) ? delf : bands[0];

            j=0;
            for (band = 0; band < numband; band++)
            {
                lowf = (band==0 ? grid0 : bands[2 * band]);
                highf = bands[2 * band + 1];
                k = (int) ((highf - lowf)/delf + 0.5);   /* .5 for rounding */
                for (i = 0; i < k; i++)
                {
                    D[j] = des[2 * band] + i* (des[2 * band + 1] - des[2 * band]) / (k - 1);
                    W[j] = weight[band];
                    Grid[j] = lowf;
                    lowf += delf;
                    j++;
                }
                Grid[j - 1] = highf;
            }
            
            /*
             * Similar to above, if odd symmetry, last grid point can't be .5
             *  - but, if there are even taps, leave the last grid point at .5
             */
            if ((symmetry == NEGATIVE) &&
                (Grid[gridsize - 1] > (0.5 - delf)) &&
                (numtaps % 2 == 1))
            {
                Grid[gridsize - 1] = 0.5 - delf;
            }
        }


        /********************
         * InitialGuess
         *==============
         * Places Extremal Frequencies evenly throughout the dense grid.
         *
         *
         * INPUT: 
         * ------
         * int r        - 1/2 the number of filter coefficients
         * int gridsize - Number of elements in the dense frequency grid
         *
         * OUTPUT:
         * -------
         * int Ext[]    - Extremal indexes to dense frequency grid [r+1]
         ********************/
        void InitialGuess(int r, ref int[] Ext, int gridsize)
        {
            for (int i = 0; i <= r; i++)
            {
                Ext[i] = i * (gridsize - 1) / r;
            }
        }


        /***********************
         * CalcParms
         *===========
         *
         *
         * INPUT:
         * ------
         * int    r      - 1/2 the number of filter coefficients
         * int    Ext[]  - Extremal indexes to dense frequency grid [r+1]
         * double Grid[] - Frequencies (0 to 0.5) on the dense grid [gridsize]
         * double D[]    - Desired response on the dense grid [gridsize]
         * double W[]    - Weight function on the dense grid [gridsize]
         *
         * OUTPUT:
         * -------
         * double ad[]   - 'b' in Oppenheim & Schafer [r+1]
         * double x[]    - [r+1]
         * double y[]    - 'C' in Oppenheim & Schafer [r+1]
         ***********************/
        void CalcParms(int r, int[] Ext, double[] Grid, double[] D, double[] W, ref double[] ad, ref double[] x, ref double[] y)
        {
            int i, j, k, ld;
            double sign, xi, delta, denom, numer;

            /*
             * Find x[]
             */
            for (i = 0; i <= r; i++)
            {
                x[i] = Math.Cos(2.0d * Math.PI * Grid[Ext[i]]);
            }

            /*
             * Calculate ad[]  - Oppenheim & Schafer eq 7.132
             */
            ld = (r - 1) / 15 + 1;         /* Skips around to avoid round errors */
            for (i = 0; i <= r; i++)
            {
                denom = 1.0;
                xi = x[i];

                for (j = 0; j < ld; j++)
                {
                    for (k = j; k <= r; k += ld)
                    {
                        if (k != i)
                        {
                            denom *= 2.0 * (xi - x[k]);
                        }
                    }
                }
                if (Math.Abs(denom) < 0.00001)
                {
                    denom = 0.00001;
                }
                ad[i] = 1.0 / denom;
            }

            /*
             * Calculate delta  - Oppenheim & Schafer eq 7.131
             */
            numer = denom = 0;
            sign = 1;
            for (i = 0; i <= r; i++)
            {
                numer += ad[i] * D[Ext[i]];
                denom += sign * ad[i] / W[Ext[i]];
                sign = -sign;
            }
            delta = numer / denom;
            sign = 1;

            /*
             * Calculate y[]  - Oppenheim & Schafer eq 7.133b
             */
            for (i = 0; i <= r; i++)
            {
                y[i] = D[Ext[i]] - sign * delta / W[Ext[i]];
                sign = -sign;
            }
        }


        /*********************
         * ComputeA
         *==========
         * Using values calculated in CalcParms, ComputeA calculates the
         * actual filter response at a given frequency (freq).  Uses
         * eq 7.133a from Oppenheim & Schafer.
         *
         *
         * INPUT:
         * ------
         * double freq - Frequency (0 to 0.5) at which to calculate A
         * int    r    - 1/2 the number of filter coefficients
         * double ad[] - 'b' in Oppenheim & Schafer [r+1]
         * double x[]  - [r+1]
         * double y[]  - 'C' in Oppenheim & Schafer [r+1]
         *
         * OUTPUT:
         * -------
         * Returns double value of A[freq]
         *********************/
        double ComputeA(double freq, int r, double[] ad, double[] x, double[] y)
        {
            int i;
            double xc, c, denom, numer;

            denom = numer = 0;
            xc = Math.Cos(2.0 * Math.PI * freq);
            for (i = 0; i <= r; i++)
            {
                c = xc - x[i];
                if (Math.Abs(c) < 1.0e-7)
                {
                    numer = y[i];
                    denom = 1;
                    break;
                }
                c = ad[i] / c;
                denom += c;
                numer += c * y[i];
            }
            return numer / denom;
        }


        /************************
         * CalcError
         *===========
         * Calculates the Error function from the desired frequency response
         * on the dense grid (D[]), the weight function on the dense grid (W[]),
         * and the present response calculation (A[])
         *
         *
         * INPUT:
         * ------
         * int    r      - 1/2 the number of filter coefficients
         * double ad[]   - [r+1]
         * double x[]    - [r+1]
         * double y[]    - [r+1]
         * int gridsize  - Number of elements in the dense frequency grid
         * double Grid[] - Frequencies on the dense grid [gridsize]
         * double D[]    - Desired response on the dense grid [gridsize]
         * double W[]    - Weight function on the desnse grid [gridsize]
         *
         * OUTPUT:
         * -------
         * double E[]    - Error function on dense grid [gridsize]
         ************************/
        void CalcError(int r, double[] ad, double[] x, double[] y, int gridsize, double[] Grid, double[] D, double[] W, ref double[] E)
        {
            int i;
            double A;

            for (i = 0; i < gridsize; i++)
            {
                A = ComputeA(Grid[i], r, ad, x, y);
                E[i] = W[i] * (D[i] - A);
            }
        }


        /************************
         * Search
         *========
         * Searches for the maxima/minima of the error curve.  If more than
         * r+1 extrema are found, it uses the following heuristic (thanks
         * Chris Hanson):
         * 1) Adjacent non-alternating extrema deleted first.
         * 2) If there are more than one excess extrema, delete the
         *    one with the smallest error.  This will create a non-alternation
         *    condition that is fixed by 1).
         * 3) If there is exactly one excess extremum, delete the smaller
         *    of the first/last extremum
         *
         *
         * INPUT:
         * ------
         * int    r        - 1/2 the number of filter coefficients
         * int    Ext[]    - Indexes to Grid[] of extremal frequencies [r+1]
         * int    gridsize - Number of elements in the dense frequency grid
         * double E[]      - Array of error values.  [gridsize]
         * OUTPUT:
         * -------
         * int    Ext[]    - New indexes to extremal frequencies [r+1]
         ************************/
        int Search(int r, int[] Ext, int gridsize, double[] E)
        {
            int i, j, k, l, extra;     /* Counters */
            bool up, alt;
            int[] foundExt;             /* Array of found extremals */

            /*
             * Allocate enough space for found extremals.
             */
            foundExt = new int[2 * r];
            k = 0;

            /*
             * Check for extremum at 0.
             */
            if (((E[0] > 0.0) && (E[0] > E[1])) || ((E[0] < 0.0) && (E[0] < E[1])))
            {
                foundExt[k++] = 0;
            }

            /*
             * Check for extrema inside dense grid
             */
            for (i = 1; i < gridsize - 1; i++)
            {
                if (((E[i] >= E[i - 1]) && (E[i] > E[i + 1]) && (E[i] > 0.0)) ||
                    ((E[i] <= E[i - 1]) && (E[i] < E[i + 1]) && (E[i] < 0.0)))
                {
                    // PAK: we sometimes get too many extremal frequencies
                    if (k >= 2 * r)
                    {
                        return -3;
                    }
                    foundExt[k++] = i;
                }
            }

            /*
             * Check for extremum at 0.5
             */
            j = gridsize - 1;
            if (((E[j] > 0.0) && (E[j] > E[j - 1])) ||
                ((E[j] < 0.0) && (E[j] < E[j - 1])))
            {
                if (k >= 2 * r)
                {
                    return -3;
                }
                foundExt[k++] = j;
            }

            // PAK: we sometimes get not enough extremal frequencies
            if (k < r + 1)
            {
                return -2;
            }

            /*
             * Remove extra extremals
             */
            extra = k - (r + 1);
            //   assert(extra >= 0);

            while (extra > 0)
            {
                if (E[foundExt[0]] > 0.0)
                    up = true;                /* first one is a maxima */
                else
                    up = false;               /* first one is a minima */

                l = 0;
                alt = true;
                for (j = 1; j < k; j++)
                {
                    if (Math.Abs(E[foundExt[j]]) < Math.Abs(E[foundExt[l]]))
                        l = j;               /* new smallest error. */
                    if ((up) && (E[foundExt[j]] < 0.0))
                        up = false;          /* switch to a minima */
                    else if ((!up) && (E[foundExt[j]] > 0.0))
                        up = true;           /* switch to a maxima */
                    else
                    {
                        alt = false;
                        // PAK: break now and you will delete the smallest overall
                        // extremal.  If you want to delete the smallest of the
                        // pair of non-alternating extremals, then you must do:
                        //
                        // if (fabs(E[foundExt[j]]) < fabs(E[foundExt[j-1]])) l=j;
                        // else l=j-1;
                        break;             /* Ooops, found two non-alternating  */
                    }                      /* extrema.  Delete smallest of them */
                }  /* if the loop finishes, all extrema are alternating */

                /*
                 * If there's only one extremal and all are alternating,
                 * delete the smallest of the first/last extremals.
                 */
                if ((alt) && (extra == 1))
                {
                    if (Math.Abs(E[foundExt[k - 1]]) < Math.Abs(E[foundExt[0]]))
                        /* Delete last extremal */
                        l = k - 1;
                    // PAK: changed from l = foundExt[k-1]; 
                    else
                        /* Delete first extremal */
                        l = 0;
                    // PAK: changed from l = foundExt[0];     
                }

                for (j = l; j < k - 1; j++)        /* Loop that does the deletion */
                {
                    foundExt[j] = foundExt[j + 1];
                    //  assert(foundExt[j]<gridsize);
                }
                k--;
                extra--;
            }

            for (i = 0; i <= r; i++)
            {
                //      assert(foundExt[i]<gridsize);
                Ext[i] = foundExt[i];       /* Copy found extremals to Ext[] */
            }

            return 0;
        }


        /*********************
         * FreqSample
         *============
         * Simple frequency sampling algorithm to determine the impulse
         * response h[] from A's found in ComputeA
         *
         *
         * INPUT:
         * ------
         * int      N        - Number of filter coefficients
         * double   A[]      - Sample points of desired response [N/2]
         * int      symmetry - Symmetry of desired filter
         *
         * OUTPUT:
         * -------
         * double h[] - Impulse Response of final filter [N]
         *********************/
        void FreqSample(int N, double[] A, ref double[] h, int symm)
        {
            int n, k;
            double x, val, M;

            M = (N - 1.0) / 2.0;
            if (symm == POSITIVE)
            {
                if (N % 2 == 1)
                {
                    for (n = 0; n < N; n++)
                    {
                        val = A[0];
                        x = 2.0 * Math.PI * (n - M) / N;
                        for (k = 1; k <= M; k++)
                        {
                            val += 2.0 * A[k] * Math.Cos(x * k);
                        }
                        h[n] = val / N;
                    }
                }
                else
                {
                    for (n = 0; n < N; n++)
                    {
                        val = A[0];
                        x = 2.0 * Math.PI * (n - M) / N;
                        for (k = 1; k <= (N / 2 - 1); k++)
                        {
                            val += 2.0 * A[k] * Math.Cos(x * k);
                        }
                        h[n] = val / N;
                    }
                }
            }
            else
            {
                if (N % 2 == 1)
                {
                    for (n = 0; n < N; n++)
                    {
                        val = 0;
                        x = 2.0 * Math.PI * (n - M) / N;
                        for (k = 1; k <= M; k++)
                        {
                            val += 2.0 * A[k] * Math.Sin(x * k);
                        }
                        h[n] = val / N;
                    }
                }
                else
                {
                    for (n = 0; n < N; n++)
                    {
                        val = A[N / 2] * Math.Sin(Math.PI * (n - M));
                        x = 2.0 * Math.PI * (n - M) / N;
                        for (k = 1; k <= (N / 2 - 1); k++)
                        {
                            val += 2.0 * A[k] * Math.Sin(x * k);
                        }
                        h[n] = val / N;
                    }
                }
            }
        }


        /*******************
         * isDone
         *========
         * Checks to see if the error function is small enough to consider
         * the result to have converged.
         *
         * INPUT:
         * ------
         * int    r     - 1/2 the number of filter coeffiecients
         * int    Ext[] - Indexes to extremal frequencies [r+1]
         * double E[]   - Error function on the dense grid [gridsize]
         *
         * OUTPUT:
         * -------
         * Returns true if the result converged
         * Returns false if the result has not converged
         ********************/
        bool isDone(int r, int[] Ext, double[] E)
        {
            int i;
            double min, max, current;

            min = max = Math.Abs(E[Ext[0]]);
            for (i = 1; i <= r; i++)
            {
                current = Math.Abs(E[Ext[i]]);
                if (current < min)
                {
                    min = current;
                }
                if (current > max)
                {
                    max = current;
                }
            }
            return (((max - min) / max) < 0.0001);
        }


        /********************
         * remez
         *=======
         * Calculates the optimal (in the Chebyshev/minimax sense)
         * FIR filter impulse response given a set of band edges,
         * the desired reponse on those bands, and the weight given to
         * the error in those bands.
         *
         * INPUT:
         * ------
         * int     ref numtaps  - Number of filter coefficients
         * int     ref numband  - Number of bands in filter specification
         * double  bands[]      - User-specified band edges [2 * numband]
         * double  des[]        - User-specified band responses [2 * numband]
         * double  weight[]     - User-specified error weights [numband]
         * int     *type        - Type of filter
         * int     *griddensity - ??
         *
         * OUTPUT:
         * -------
         * double h[]      - Impulse response of final filter [numtaps]
         ********************/
        void remez(ref double[] h, int numtaps, int numband, double[] bands, double[] des, double[] weight, ref int type, ref int griddensity)
        {
            int i, iter, gridsize, r;
            double c;
            int symmetry;

            if (type == BANDPASS)
                symmetry = POSITIVE;
            else
                symmetry = NEGATIVE;

            r = numtaps / 2;                  /* number of extrema */
            if ((numtaps % 2 == 1) && (symmetry == POSITIVE))
            {
                r++;
            }

            h[0] = 32;
            /*
             * Predict dense grid size in advance for memory allocation
             *   .5 is so we round up, not truncate
             */
            gridsize = 0;
            for (i = 0; i < numband; i++)
            {
                gridsize += (int)(2 * r * (griddensity) * (bands[2 * i + 1] - bands[2 * i]) + .5);
            }
            if (symmetry == NEGATIVE)
            {
                gridsize--;
            }

            /*
             * Dynamically allocate memory for arrays with proper sizes
             */
            double[] Grid = new double[gridsize];
            double[] D = new double[gridsize];
            double[] W = new double[gridsize];
            double[] E = new double[gridsize];
            int[] Ext = new int[r + 1];
            double[] taps = new double[r + 1];
            double[] x = new double[r + 1];
            double[] y = new double[r + 1];
            double[] ad = new double[r + 1];

            /*
             * Create dense frequency grid
             */
            CreateDenseGrid(r, numtaps, numband, bands, des, weight, gridsize, ref Grid, ref D, ref W, symmetry, griddensity);
            InitialGuess(r, ref Ext, gridsize);

            /*
             * For Differentiator: (fix grid)
             */
            if (type == DIFFERENTIATOR)
            {
                for (i = 0; i < gridsize; i++)
                {
                    /* D[i] = D[i]*Grid[i]; */
                    if (D[i] > 0.0001)
                    {
                        W[i] = W[i] / Grid[i];
                    }
                }
            }

            /*
             * For odd or Negative symmetry filters, alter the
             * D[] and W[] according to Parks McClellan
             */
            if (symmetry == POSITIVE)
            {
                if (numtaps % 2 == 0)
                {
                    for (i = 0; i < gridsize; i++)
                    {
                        c = Math.Cos(Math.PI * Grid[i]);
                        D[i] /= c;
                        W[i] *= c;
                    }
                }
            }
            else
            {
                if (numtaps % 2 == 1)
                {
                    for (i = 0; i < gridsize; i++)
                    {
                        c = Math.Sin(2.0 * Math.PI * Grid[i]);
                        D[i] /= c;
                        W[i] *= c;
                    }
                }
                else
                {
                    for (i = 0; i < gridsize; i++)
                    {
                        c = Math.Sin(Math.PI * Grid[i]);
                        D[i] /= c;
                        W[i] *= c;
                    }
                }
            }

            /*
             * Perform the Remez Exchange algorithm
             */
            for (iter = 0; iter < MAXITERATIONS; iter++)
            {
                CalcParms(r, Ext, Grid, D, W, ref ad, ref x, ref y);
                CalcError(r, ad, x, y, gridsize, Grid, D, W, ref E);
                int err = Search(r, Ext, gridsize, E);
                if (err != 0) Console.WriteLine("error, %i, %i", err, gridsize);
                //      for(i=0; i <= r; i++) assert(Ext[i]<gridsize);
                if (isDone(r, Ext, E)) break;
            }

            CalcParms(r, Ext, Grid, D, W, ref ad, ref x, ref y);

            /*
             * Find the 'taps' of the filter for use with Frequency
             * Sampling.  If odd or Negative symmetry, fix the taps
             * according to Parks McClellan
             */
            for (i = 0; i <= numtaps / 2; i++)
            {
                if (symmetry == POSITIVE)
                {
                    if (numtaps % 2 == 1)
                    {
                        c = 1;
                    }
                    else
                    {
                        c = Math.Cos(Math.PI * (double)i / numtaps);
                    }
                }
                else
                {
                    if (numtaps % 2 == 1)
                    {
                        c = Math.Sin(2.0 * Math.PI * (double)i / numtaps);
                    }
                    else
                    {
                        c = Math.Sin(Math.PI * (double)i / numtaps);
                    }
                }
                taps[i] = ComputeA((double)i / numtaps, r, ad, x, y) * c;
            }

            /*
             * Frequency sampling design with calculated taps
             */
            FreqSample(numtaps, taps, ref h, symmetry);
        }

        /*
        %!test
        %! b = [
        %!    0.0415131831103279
        %!    0.0581639884202646
        %!   -0.0281579212691008
        %!   -0.0535575358002337
        %!   -0.0617245915143180
        %!    0.0507753178978075
        %!    0.2079018331396460
        %!    0.3327160895375440
        %!    0.3327160895375440
        %!    0.2079018331396460
        %!    0.0507753178978075
        %!   -0.0617245915143180
        %!   -0.0535575358002337
        %!   -0.0281579212691008
        %!    0.0581639884202646
        %!    0.0415131831103279];
        %! assert(remez(15,[0,0.3,0.4,1],[1,1,0,0]),b,1e-14);
         */
    }
}