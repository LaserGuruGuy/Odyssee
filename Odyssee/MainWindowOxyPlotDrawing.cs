using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using Audyssey.MultEQAvr;
using MathNet.Numerics.IntegralTransforms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Odyssee
{
    public class LockableLogarithmicAxis : LogarithmicAxis
    {
        #region Properties
        public bool IsLocked { get; set; }
        public double[] MajorTickPositions { get; set; }
        public double[] MinorTickPositions { get; set; }
        #endregion

        #region Constructor
        public LockableLogarithmicAxis()
        {
            IsLocked = true;
        }
        #endregion

        #region Methods
        public override void GetTickValues(out IList<double> majorLabelValues, out IList<double> majorTickValues, out IList<double> minorTickValues)
        {
            if (!IsLocked)
            {
                base.GetTickValues(out majorLabelValues, out majorTickValues, out minorTickValues);
                return;
            }

            if (MajorTickPositions != null && MajorTickPositions.Length > 0)
            {
                majorTickValues = MajorTickPositions.ToList();
            }
            else
            {
                majorTickValues = this.DecadeTicks();
            }

            if (MinorTickPositions != null && MinorTickPositions.Length > 0)
            {
                minorTickValues = MinorTickPositions.ToList();
            }
            else
            {
                minorTickValues = this.SubdividedDecadeTicks();
            }

            majorLabelValues = majorTickValues;
        }

        /// <summary>
        /// Calculates ticks of the decades in the axis range with a specified step size.
        /// </summary>
        /// <param name="step">The step size.</param>
        /// <returns>A new IList containing the decade ticks.</returns>
        private IList<double> DecadeTicks(double step = 1)
        {
            return this.PowList(this.LogDecadeTicks(step));
        }

        /// <summary>
        /// Calculates logarithmic ticks of the decades in the axis range with a specified step size.
        /// </summary>
        /// <param name="step">The step size.</param>
        /// <returns>A new IList containing the logarithmic decade ticks.</returns>
        private IList<double> LogDecadeTicks(double step = 1)
        {
            var ret = new List<double>();
            if (step > 0)
            {
                var last = double.NaN;
                for (var exponent = Math.Ceiling(this.LogActualMinimum); exponent <= Math.Ceiling(this.LogActualMaximum); exponent += step)
                {
                    if (exponent <= last)
                    {
                        break;
                    }

                    last = exponent;
                    if (exponent >= this.LogActualMinimum)
                    {
                        ret.Add(exponent);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Raises all elements of a List to the power of <c>this.Base</c>.
        /// </summary>
        /// <param name="logInput">The input values.</param>
        /// <param name="clip">If true, discards all values that are not in the axis range.</param>
        /// <returns>A new IList containing the resulting values.</returns>
        private IList<double> PowList(IList<double> logInput, bool clip = false)
        {
            return
                logInput.Where(item => !clip || !(item < this.LogActualMinimum))
                    .TakeWhile(item => !clip || !(item > this.LogActualMaximum))
                    .Select(item => Math.Pow(this.Base, item))
                    .ToList();
        }

        /// <summary>
        /// Calculates ticks of all decades in the axis range and their subdivisions.
        /// </summary>
        /// <param name="clip">If true (default), the lowest and highest decade are clipped to the axis range.</param>
        /// <returns>A new IList containing the decade ticks.</returns>
        private IList<double> SubdividedDecadeTicks(bool clip = true)
        {
            var ret = new List<double>();
            for (var exponent = (int)Math.Floor(this.LogActualMinimum); ; exponent++)
            {
                if (exponent > this.LogActualMaximum)
                {
                    break;
                }

                var currentDecade = Math.Pow(this.Base, exponent);
                for (var mantissa = 1; mantissa < this.Base; mantissa++)
                {
                    var currentValue = currentDecade * mantissa;
                    if (clip && currentValue < this.ActualMinimum)
                    {
                        continue;
                    }

                    if (clip && currentValue > this.ActualMaximum)
                    {
                        break;
                    }

                    ret.Add(currentDecade * mantissa);
                }
            }
            return ret;
        }
        #endregion
    }

    class AxisLimit
    {
        public double XMin { get; set; }
        public double XMax { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }
        public double MajorStep { get; set; }
        public double MinorStep { get; set; }
    }

    public partial class MainWindow : Window
    {
        public PlotModel PlotModel { get; private set; } = new();

        private string ValueAxisLabelFormatter(double input)
        {
            return (string.Empty);
        }

        const double SecondsToMilliseconds = 1000;
        const double SampleRate48KHz = 48000;
        const double SampleSize = 16384;

        private string selectedCurveFilter = string.Empty;
        bool LogarithmicAxis = false;

        private Dictionary<string, Brush> ResponseDataTraceColor = new Dictionary<string, Brush> { { "0", Brushes.Black }, { "1", Brushes.Blue }, { "2", Brushes.Violet }, { "3", Brushes.Green }, { "4", Brushes.Orange }, { "5", Brushes.Red }, { "6", Brushes.Cyan }, { "7", Brushes.DeepPink } };
        private Dictionary<string, Brush> FlatCurveFilterTraceColor = new Dictionary<string, Brush> { { AudysseyMultEQAvr.SampleRateList[0], Brushes.BlueViolet }, { AudysseyMultEQAvr.SampleRateList[1], Brushes.BlueViolet }, { AudysseyMultEQAvr.SampleRateList[2], Brushes.BlueViolet }, { AudysseyMultEQAvr.DispDataList[0], Brushes.BlueViolet }, { AudysseyMultEQAvr.DispDataList[1], Brushes.BlueViolet } };
        private Dictionary<string, Brush> AudyCurveFilterTraceColor = new Dictionary<string, Brush> { { AudysseyMultEQAvr.SampleRateList[0], Brushes.Maroon }, { AudysseyMultEQAvr.SampleRateList[1], Brushes.Maroon }, { AudysseyMultEQAvr.SampleRateList[2], Brushes.Maroon }, { AudysseyMultEQAvr.DispDataList[0], Brushes.Maroon }, { AudysseyMultEQAvr.DispDataList[1], Brushes.Maroon } };

        private string selectedAxisLimits = "RadioButton_RangeChirp";
        private Dictionary<string, AxisLimit> AxisLimits = new Dictionary<string, AxisLimit>()
        {
            {"RadioButton_RangeFull", new AxisLimit { XMin = 10, XMax = SampleRate48KHz/2.0d, YMin = -35, YMax = 20, MajorStep = 5, MinorStep = 1 } },
            {"RadioButton_RangeSubwoofer", new AxisLimit { XMin = 10, XMax = 1000, YMin = -35, YMax = 20, MajorStep = 5, MinorStep = 1 } },
            {"RadioButton_RangeChirp", new AxisLimit { XMin = 0, XMax = SecondsToMilliseconds*SampleSize/SampleRate48KHz, YMin = -0.1, YMax = 0.1, MajorStep = 0.01, MinorStep = 0.001 } }
        };

        private void DrawChart()
        {
            PlotModel.Series.Clear();
            PlotModel.Axes.Clear();
            if (audysseyMultEQAvr != null)
            {
                PlotModel.Title = string.Empty;
                PlotAxis();
                /* plot selected channel */
                if (audysseyMultEQAvr.SelectedChannel != null)
                {
                    PlotLine(audysseyMultEQAvr.SelectedChannel, false, audysseyMultEQAvr.SmoothingFactor);
                }
                /* plot sticky channels */
                if (audysseyMultEQAvr.DetectedChannels != null)
                {
                    foreach (var SelectedChannel in audysseyMultEQAvr.DetectedChannels)
                    {
                        if ((bool)SelectedChannel.Stick)
                        {
                            PlotLine(SelectedChannel, true, audysseyMultEQAvr.SmoothingFactor);
                        }
                    }
                }
            }
            else
            {
                PlotModel.Title = string.Empty;
            }
            PlotModel.InvalidatePlot(true);
        }

        private void PlotAxis()
        {
            AxisLimit Limits = AxisLimits[selectedAxisLimits];

            if (selectedAxisLimits.Contains("Chirp"))
            {
                if (LogarithmicAxis)
                {
                    PlotModel.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Bottom, Title = "ms", Minimum = Limits.XMin, Maximum = Limits.XMax, MajorGridlineStyle = LineStyle.Dot, MinorGridlineStyle = LineStyle.Dot });
                }
                else
                {
                    PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "ms", Minimum = Limits.XMin, Maximum = Limits.XMax, MajorGridlineStyle = LineStyle.Dot });
                }
                PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "", Minimum = Limits.YMin, Maximum = Limits.YMax, MajorStep = Limits.MajorStep, MinorStep = Limits.MinorStep, MajorGridlineStyle = LineStyle.Solid });
            }
            else
            {
                if (LogarithmicAxis)
                {
                    PlotModel.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Bottom, Title = "Hz", Minimum = Limits.XMin, Maximum = Limits.XMax, MajorGridlineStyle = LineStyle.Dot, MinorGridlineStyle = LineStyle.Dot });
                }
                else
                {
                    PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Hz", Minimum = Limits.XMin, Maximum = Limits.XMax, MajorGridlineStyle = LineStyle.Dot });
                }
                PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "dB", Minimum = Limits.YMin, Maximum = Limits.YMax, MajorStep = Limits.MajorStep, MinorStep = Limits.MinorStep, MajorGridlineStyle = LineStyle.Solid });
            }
        }

        private void PlotLine(DetectedChannel selectedChannel, bool DotNotSolid = false, int SmoothingFactor = 0)
        {
            if (selectedChannel != null)
            {
                ///* Plot selected position: response data and filter coefficients */
                OxyColor CurveColor = new();

                /* Selected channel response data key and value are null if there is no channel selected in the GUI */
                if ((selectedChannel.SelectedResponseData.Key != null) && (selectedChannel.SelectedResponseData.Value != null))
                {
                    CurveColor = OxyColor.Parse(ResponseDataTraceColor[selectedChannel.SelectedResponseData.Key].ToString());
                    PlotCurve(SampleRate48KHz, selectedChannel.SelectedResponseData.Value, SmoothingFactor, CurveColor, DotNotSolid ? LineStyle.Dot : LineStyle.Solid, 1);
                }

                /* Iterate over all the sticky positions */
                foreach (var stickyResponseData in selectedChannel.StickyResponseData)
                {
                    /* Sticky channel response data key and value are null if there is no channel selected in the GUI */
                    if ((stickyResponseData.Key != null) && (stickyResponseData.Value != null))
                    {
                        /* Avoid duplication of response data if the sticky channel was also a selected channel */
                        if (!stickyResponseData.Equals(selectedChannel.SelectedResponseData))
                        {
                            CurveColor = OxyColor.Parse(ResponseDataTraceColor[stickyResponseData.Key].ToString());
                            PlotCurve(SampleRate48KHz, stickyResponseData.Value, SmoothingFactor, CurveColor, DotNotSolid ? LineStyle.Dot : LineStyle.Solid, 1);
                        }
                    }
                }

                /* Selected audy curve filter key and value are null if there is no channel selected in the GUI */
                if ((selectedChannel.SelectedAudyCurveFilter.Key != null) && (selectedChannel.SelectedAudyCurveFilter.Value != null))
                {
                    PlotCurveFilter(selectedChannel.SelectedAudyCurveFilter,
                    OxyColor.Parse(AudyCurveFilterTraceColor[selectedChannel.SelectedAudyCurveFilter.Key].ToString()),
                    SmoothingFactor,
                    selectedChannel.FilterFrequencies,
                    selectedChannel.DisplayFrequencies);
                }

                /* Selected flat curve filter key and value are null if there is no channel selected in the GUI */
                if ((selectedChannel.SelectedFlatCurveFilter.Key != null) && (selectedChannel.SelectedFlatCurveFilter.Value != null))
                {
                    PlotCurveFilter(selectedChannel.SelectedFlatCurveFilter,
                    OxyColor.Parse(FlatCurveFilterTraceColor[selectedChannel.SelectedFlatCurveFilter.Key].ToString()),
                    SmoothingFactor,
                    selectedChannel.FilterFrequencies,
                    selectedChannel.DisplayFrequencies);
                }

                /* Apply filter to response based on GUI radiobutton */
                if (selectedCurveFilter.Contains(AudysseyMultEQAvr.AudyEqSetList[0]))
                {
                    /* Audy curve filter */
                }
                else if (selectedCurveFilter.Contains(AudysseyMultEQAvr.AudyEqSetList[1]))
                {
                    /* Flat curve filter */
                }
            }
        }

        private void PlotCurveFilter(KeyValuePair<string, double[]> CurveFilter, OxyColor CurveColor, int SmoothingFactor, double[] FilterFrequencies, double[] DisplayFrequencies)
        {
            if (CurveFilter.Key.Equals(AudysseyMultEQAvr.SampleRateList[0]))
            {
                /* 1024 filter coefficients (704 for subwoofer) */
                PlotCurve(CurveFilter.Value.Length == 1024 ? AudysseyMultEQAvr.SampleFrequencyList[0] : AudysseyMultEQAvr.SampleFrequencyList[0] / 48d, CurveFilter.Value, SmoothingFactor, CurveColor, LineStyle.Solid, 2);
            }
            else if (CurveFilter.Key.Equals(AudysseyMultEQAvr.SampleRateList[1]))
            {
                /* 1024 filter coefficients (704 for subwoofer)*/
                PlotCurve(CurveFilter.Value.Length == 1024 ? AudysseyMultEQAvr.SampleFrequencyList[1] : AudysseyMultEQAvr.SampleFrequencyList[1] / 48d, CurveFilter.Value, SmoothingFactor, CurveColor, LineStyle.Solid, 2);
            }
            else if (CurveFilter.Key.Equals(AudysseyMultEQAvr.SampleRateList[2]))
            {
                /* 1024 filter coefficients (704 for subwoofer) */
                PlotCurve(CurveFilter.Value.Length == 1024 ? AudysseyMultEQAvr.SampleFrequencyList[2] : AudysseyMultEQAvr.SampleFrequencyList[2] / 48d, CurveFilter.Value, SmoothingFactor, CurveColor, LineStyle.Solid, 2);
            }
            else if (CurveFilter.Key.Equals(AudysseyMultEQAvr.DispDataList[0]))
            {
                /* 61 equalizer bands */
                if (CurveFilter.Value.Length == FilterFrequencies.Length)
                {
                    Collection<DataPoint> dataPoint = new Collection<DataPoint>();
                    for (int j = 0; j < CurveFilter.Value.Length; j++)
                    {
                        dataPoint.Add(new DataPoint(FilterFrequencies[j], CurveFilter.Value[j]));
                    }
                    var sinStemSeries = new StemSeries
                    {
                        Color = CurveColor,
                        MarkerStroke = CurveColor,
                        MarkerType = MarkerType.Circle
                    };
                    sinStemSeries.Points.AddRange(dataPoint);
                    PlotModel.Series.Add(sinStemSeries);
                }
            }
            else if (CurveFilter.Key.Equals(AudysseyMultEQAvr.DispDataList[1]))
            {
                /* 9 equalizer bands */
                if (CurveFilter.Value.Length == DisplayFrequencies.Length)
                {
                    Collection<DataPoint> dataPoint = new Collection<DataPoint>();
                    for (int j = 0; j < CurveFilter.Value.Length; j++)
                    {
                        dataPoint.Add(new DataPoint(DisplayFrequencies[j], CurveFilter.Value[j]));
                    }
                    var sinStemSeries = new StemSeries
                    {
                        Color = CurveColor,
                        StrokeThickness = 2,
                        MarkerType = MarkerType.Circle,
                        MarkerStroke = CurveColor,
                        MarkerStrokeThickness = 2
                    };
                    sinStemSeries.Points.AddRange(dataPoint);
                    PlotModel.Series.Add(sinStemSeries);
                }
            }
        }

        private void PlotCurve(double sampleRate, double[] responseData, int smoothingFactor, OxyColor oxyColor, LineStyle lineStyle, double strokeThickness)
        {
            Collection<DataPoint> dataPoint = new Collection<DataPoint>();
            if (selectedAxisLimits.Contains("Chirp"))
            {
                for (int j = 0; j < responseData.Length; j++)
                {
                    dataPoint.Add(new DataPoint(SecondsToMilliseconds * j / sampleRate, responseData[j]));
                }
            }
            else
            {
                AxisLimit axisLimits = AxisLimits[selectedAxisLimits];

                double[] Frequency = new double[responseData.Length];
                Complex[] complexData = new Complex[responseData.Length];

                for (int j = 0; j < responseData.Length; j++)
                {
                    Frequency[j] = (double)j / responseData.Length * sampleRate;
                    complexData[j] = (Complex)(responseData[j]);
                }
                Frequency[0] = 0.5 / responseData.Length * sampleRate;

                MathNet.Numerics.IntegralTransforms.Fourier.Forward(complexData, FourierOptions.NoScaling);

                if (smoothingFactor != 0)
                {
                    double[] smoothed = new double[responseData.Length];
                    for (int j = 0; j < responseData.Length; j++)
                    {
                        smoothed[j] = complexData[j].Magnitude;
                    }

                    LinSpacedFracOctaveSmooth(49 - smoothingFactor, ref smoothed, 1, 1d / 48);

                    for (int x = 0; x < responseData.Length / 2; x++)
                    {
                        dataPoint.Add(new DataPoint(Frequency[x], 20 * Math.Log10(smoothed[x])));
                    }
                }
                else
                {
                    for (int x = 0; x < responseData.Length / 2; x++)
                    {
                        dataPoint.Add(new DataPoint(Frequency[x], 20 * Math.Log10(complexData[x].Magnitude)));
                    }
                }
            }

            LineSeries lineserie = new LineSeries
            {
                ItemsSource = dataPoint,
                DataFieldX = "X",
                DataFieldY = "Y",
                StrokeThickness = strokeThickness,
                MarkerSize = 0,
                LineStyle = lineStyle,
                Color = oxyColor,
                MarkerType = MarkerType.None,
            };

            PlotModel.Series.Add(lineserie);
        }
        private void LinSpacedFracOctaveSmooth(double frac, ref double[] smoothed, float startFreq, double freqStep)
        {
            int passes = 8;
            // Scale octave frac to allow for number of passes
            double scaledFrac = 7.5 * frac; //Empirical tweak to better match Gaussian smoothing
            double octMult = Math.Pow(2, 0.5 / scaledFrac);
            double bwFactor = (octMult - 1 / octMult);
            double b = 0.5 + bwFactor * startFreq / freqStep;
            int N = smoothed.Length;
            double xp;
            double yp;
            // Smooth from HF to LF to avoid artificial elevation of HF data
            for (int pass = 0; pass < passes; pass++)
            {
                xp = smoothed[N - 1];
                yp = xp;
                // reverse pass
                for (int i = N - 2; i >= 0; i--)
                {
                    double a = 1 / (b + i * bwFactor);
                    yp += ((xp + smoothed[i]) / 2 - yp) * a;
                    xp = smoothed[i];
                    smoothed[i] = (float)yp;
                }
                // forward pass
                for (int i = 1; i < N; i++)
                {
                    double a = 1 / (b + i * bwFactor);
                    yp += ((xp + smoothed[i]) / 2 - yp) * a;
                    xp = smoothed[i];
                    smoothed[i] = (float)yp;
                }
            }
        }

        public void InitOxyPlotLvlm()
        {
            if (PlotModel != null)
            {
                if (PlotModel.Title != "Subwoofer Level")
                {
                    PlotModel.Title = "Subwoofer Level";

                    PlotModel.Axes.Clear();
                    PlotModel.Axes.Add(new LogarithmicAxis { Unit = "dB", Position = AxisPosition.Left, Base = 10, Minimum = 65, Maximum = 85, MajorStep = 5, MinorStep = 1, TickStyle = TickStyle.Outside, MajorGridlineStyle = LineStyle.Dot, MinorGridlineStyle = LineStyle.Dot });
                    PlotModel.Axes.Add(new LinearAxis { LabelFormatter = ValueAxisLabelFormatter, Position = AxisPosition.Bottom, Minimum = 0, TickStyle = TickStyle.None, MajorGridlineStyle = LineStyle.Dot, MinorGridlineStyle = LineStyle.Dot });

                    PlotModel.Series.Clear();
                    LineSeries Lineserie1 = new() { StrokeThickness = 2, MarkerSize = 0, LineStyle = LineStyle.Solid, Color = OxyColors.Blue, MarkerType = MarkerType.None };
                    PlotModel.Series.Add(Lineserie1);
                    LineSeries Lineserie2 = new() { StrokeThickness = 2, MarkerSize = 0, LineStyle = LineStyle.Dash, Color = OxyColors.Green, MarkerType = MarkerType.None };
                    PlotModel.Series.Add(Lineserie2);
                    LineSeries Lineserie3 = new() { StrokeThickness = 2, MarkerSize = 0, LineStyle = LineStyle.Dash, Color = OxyColors.Green, MarkerType = MarkerType.None };
                    PlotModel.Series.Add(Lineserie3);
                    LineSeries Lineserie4 = new() { StrokeThickness = 2, MarkerSize = 0, LineStyle = LineStyle.Dash, Color = OxyColors.Magenta, MarkerType = MarkerType.None };
                    PlotModel.Series.Add(Lineserie4);
                }
            }
        }

        public void AddOxyPlotLvlm()
        {
            if (PlotModel != null)
            {
                if (PlotModel.Series != null)
                {
                    for (int i = 0; i < PlotModel.Series.Count; i++)
                    {
                        var s = (LineSeries)PlotModel.Series[i];
                        switch (i)
                        {
                            case 0:
                                s.Points.Add(new DataPoint(s.Points.Count, (double)audysseyMultEQAvr.SPLValuedB));
                                break;
                            case 1:
                                s.Points.Add(new DataPoint(s.Points.Count, 72));
                                break;
                            case 2:
                                s.Points.Add(new DataPoint(s.Points.Count, 78));
                                break;
                            case 3:
                                s.Points.Add(new DataPoint(s.Points.Count, (double)audysseyMultEQAvr.SPLAvgdB));
                                break;
                        }
                        PlotModel.InvalidatePlot(true);
                    }
                }
            }
        }
    }
}