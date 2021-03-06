using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using Audyssey.MultEQ.List;
using Audyssey.MultEQAvr;
using MathNet.Filtering.FIR;
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

        OnlineFirFilter FirFilter = null;

        private string selectedCurveFilter = string.Empty;
        bool LogarithmicAxis = false;

        private Dictionary<string, Brush> ResponseDataTraceColor = new Dictionary<string, Brush> { { "0", Brushes.Black }, { "1", Brushes.Blue }, { "2", Brushes.Violet }, { "3", Brushes.Green }, { "4", Brushes.Orange }, { "5", Brushes.Red }, { "6", Brushes.Cyan }, { "7", Brushes.DeepPink } };

        private string selectedAxisLimits = "RadioButton_ImpulseResponse";
        private Dictionary<string, AxisLimit> AxisLimits = new Dictionary<string, AxisLimit>()
        {
            {"RadioButton_RangeSamp", new AxisLimit { XMin = 2, XMax = 24000, YMin = -35, YMax = 20, MajorStep = 5, MinorStep = 1 } },
            {"RadioButton_RangeFull", new AxisLimit { XMin = 20, XMax = 20000, YMin = -35, YMax = 20, MajorStep = 5, MinorStep = 1 } },
            {"RadioButton_RangeSubw", new AxisLimit { XMin = 10, XMax = 200, YMin = -35, YMax = 20, MajorStep = 5, MinorStep = 1 } },
            {"RadioButton_ImpulseResponse", new AxisLimit { XMin = 0, XMax = SecondsToMilliseconds*SampleSize/SampleRate48KHz, YMin = -0.3, YMax = +0.3, MajorStep = 0.1, MinorStep = 0.01 } }
        };

        private void DrawChart()
        {
            PlotModel.EdgeRenderingMode = EdgeRenderingMode.PreferSpeed;
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

            if (selectedAxisLimits.Contains("ImpulseResponse"))
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
                    PlotCurve(SampleRate48KHz, selectedChannel.SelectedResponseData.Value, (double)selectedChannel.ChLevel, SmoothingFactor, CurveColor, DotNotSolid ? LineStyle.Dot : LineStyle.Solid, 1);
                }

                if (selectedChannel.StickyResponseData != null)
                {
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
                                PlotCurve(SampleRate48KHz, stickyResponseData.Value, (double)selectedChannel.ChLevel, SmoothingFactor, CurveColor, DotNotSolid ? LineStyle.Dot : LineStyle.Solid, 1);
                            }
                        }
                    }
                }

                /* Selected audy curve filter key and value are null if there is no channel selected in the GUI */
                if ((selectedChannel.SelectedAudyCurveFilter.Key != null) && (selectedChannel.SelectedAudyCurveFilter.Value != null))
                {
                    PlotCurveFilter(MultEQList.AudyEqSetList[0],
                        selectedChannel.SelectedAudyCurveFilter,
                        OxyColor.Parse(Brushes.Teal.ToString()),
                        SmoothingFactor,
                        ChannelList.FilterFrequencies.ToArray(),
                        ChannelList.DisplayFrequencies.ToArray());
                }

                /* Selected flat curve filter key and value are null if there is no channel selected in the GUI */
                if ((selectedChannel.SelectedFlatCurveFilter.Key != null) && (selectedChannel.SelectedFlatCurveFilter.Value != null))
                {
                    PlotCurveFilter(MultEQList.AudyEqSetList[1], 
                        selectedChannel.SelectedFlatCurveFilter,
                        OxyColor.Parse(Brushes.BlueViolet.ToString()),
                        SmoothingFactor,
                        ChannelList.FilterFrequencies.ToArray(),
                        ChannelList.DisplayFrequencies.ToArray());
                }

                if (CheckBox_CurveFilter.IsChecked == true)
                {
                    /* Select filter curve based on GUI radiobutton */
                    if (selectedCurveFilter.Contains(AudysseyMultEQAvr.AudyEqSetList[0]))
                    {
                        /* Load 48kHz audy FIR filter coefficients */
                        try
                        {
                            FirFilter = new(selectedChannel.AudyCurveFilter[AudysseyMultEQAvr.SampleRateList[2]]);
                            selectedChannel.FirFilterGain = FirFilter.Gain;
                        }
                        catch
                        {

                        }
                    }
                    else if (selectedCurveFilter.Contains(AudysseyMultEQAvr.AudyEqSetList[1]))
                    {
                        /* Load 48kHz flat FIR filter coefficients */
                        try
                        {
                            FirFilter = new(selectedChannel.FlatCurveFilter[AudysseyMultEQAvr.SampleRateList[2]]);
                            selectedChannel.FirFilterGain = FirFilter.Gain;
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        FirFilter = null;
                        selectedChannel.FirFilterGain = null;
                    }
                }
                else
                {
                    FirFilter = null;
                    selectedChannel.FirFilterGain = null;
                }

                PlotAverageCurve(selectedChannel.AverageResponseData, (double)selectedChannel.ChLevel, SmoothingFactor);
            }
        }

        private void PlotCurveFilter(string AudyEqSet, KeyValuePair<string, double[]> CurveFilter, OxyColor CurveColor, int SmoothingFactor, double[] FilterFrequencies, double[] DisplayFrequencies)
        {
            if (CurveFilter.Key.Equals(AudysseyMultEQAvr.SampleRateList[0]))
            {
                /* 32 kHz 1024 filter coefficients (2666 Hz 704 filter coefficients for subwoofer) */
                PlotCurve(CurveFilter.Value.Length == 1024 ? AudysseyMultEQAvr.SampleFrequencyList[0] : AudysseyMultEQAvr.SampleFrequencyList[0] / 12d, CurveFilter.Value, 0, SmoothingFactor, CurveColor, LineStyle.Solid, 2);
            }
            else if (CurveFilter.Key.Equals(AudysseyMultEQAvr.SampleRateList[1]))
            {
                /* 44.1 kHz 1024 filter coefficients (3675 Hz  704 filter coefficients for subwoofer)*/
                PlotCurve(CurveFilter.Value.Length == 1024 ? AudysseyMultEQAvr.SampleFrequencyList[1] : AudysseyMultEQAvr.SampleFrequencyList[1] / 12d, CurveFilter.Value, 0, SmoothingFactor, CurveColor, LineStyle.Solid, 2);
            }
            else if (CurveFilter.Key.Equals(AudysseyMultEQAvr.SampleRateList[2]))
            {
                /* 48 kHz 1024 filter coefficients (4000 Hz 704 filter coefficients for subwoofer) */
                PlotCurve(CurveFilter.Value.Length == 1024 ? AudysseyMultEQAvr.SampleFrequencyList[2] : AudysseyMultEQAvr.SampleFrequencyList[2] / 12d, CurveFilter.Value, 0, SmoothingFactor, CurveColor, LineStyle.Solid, 2);
            }
            else if (CurveFilter.Key.Equals(AudysseyMultEQAvr.DispDataList[0]))
            {
                // only show frequencyplot
                if (selectedAxisLimits.Contains("ImpulseResponse") == false)
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
                            ItemsSource = dataPoint,
                            Title = AudyEqSet + CurveFilter.Key,
                            Color = CurveColor,
                            MarkerStroke = CurveColor,
                            MarkerType = MarkerType.Circle
                        };

                        PlotModel.Series.Add(sinStemSeries);
                    }
                }
            }
            else if (CurveFilter.Key.Equals(AudysseyMultEQAvr.DispDataList[1]))
            {
                // only show frequencyplot
                if (selectedAxisLimits.Contains("ImpulseResponse") == false)
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
                            ItemsSource = dataPoint,
                            Title = AudyEqSet + CurveFilter.Key,
                            Color = CurveColor,
                            StrokeThickness = 2,
                            MarkerType = MarkerType.Circle,
                            MarkerStroke = CurveColor,
                            MarkerStrokeThickness = 2
                        };

                        PlotModel.Series.Add(sinStemSeries);
                    }
                }
            }
        }

        private void PlotAverageCurve(List<KeyValuePair<string, double[]>> AverageResponseData, double ChLevel, int smoothingFactor)
        {
            if (selectedAxisLimits.Contains("ImpulseResponse") == false)
            {
                if (AverageResponseData != null)
                {
                    if (AverageResponseData.Count > 0)
                    {
                        int Length = AverageResponseData[0].Value.Length;
                        double[] Frequency = MathNet.Numerics.IntegralTransforms.Fourier.FrequencyScale(Length, 48000);
                        double[] MagnitudeSquared = new double[Length / 2];

                        foreach (var ResponseData in AverageResponseData)
                        {
                            MathNet.Numerics.Complex32[] complexData = new MathNet.Numerics.Complex32[Length];

                            if (CheckBox_CurveFilter.IsChecked == true && FirFilter != null)
                            {
                                /* reset FIR filter state */
                                FirFilter.Reset();
                                /* FIR filter respnse */
                                for (int i = 0; i < Length; i++)
                                {
                                    complexData[i] = (MathNet.Numerics.Complex32)FirFilter.ProcessSample(ResponseData.Value[i]);
                                }
                            }
                            else
                            {
                                /* unfiltered */
                                for (int i = 0; i < Length; i++)
                                {
                                    complexData[i] = (MathNet.Numerics.Complex32)ResponseData.Value[i];
                                }
                            }

                            MathNet.Numerics.IntegralTransforms.Fourier.Forward(complexData, MathNet.Numerics.IntegralTransforms.FourierOptions.NoScaling);

                            for (int i = 0; i < Length / 2; i++)
                            {
                                /* averaging in power domain */
                                MagnitudeSquared[i] += complexData[i].MagnitudeSquared;
                            }
                        }

                        /* average over all positions */
                        for (int i = 0; i < Length / 2; i++)
                        {
                            MagnitudeSquared[i] /= AverageResponseData.Count;
                        }

                        if (smoothingFactor > 0)
                        {
                            LinSpacedFracOctaveSmooth(49 - smoothingFactor, ref MagnitudeSquared, 1, 1d / 48);
                        }

                        Collection<DataPoint> dataPoint = new Collection<DataPoint>();
                        for (int i = 0; i < Length / 2; i++)
                        {
                            /* 20*log10(sqrt(MagnitudeSquared)) == 10*log10(MagnitudeSquared) */
                            dataPoint.Add(new DataPoint(Frequency[i], ChLevel + 10 * Math.Log10(MagnitudeSquared[i])));
                        }

                        LineSeries lineserie = new LineSeries
                        {
                            ItemsSource = dataPoint,
                            DataFieldX = "X",
                            DataFieldY = "Y",
                            StrokeThickness = 2,
                            MarkerSize = 0,
                            LineStyle = LineStyle.Solid,
                            Color = OxyColor.Parse(Brushes.DarkRed.ToString()),
                            MarkerType = MarkerType.None,
                        };

                        PlotModel.Series.Add(lineserie);
                    }
                }
            }
        }

        private void PlotCurve(double sampleRate, double[] responseData, double ChLevel, int smoothingFactor, OxyColor oxyColor, LineStyle lineStyle, double strokeThickness)
        {
            Collection<DataPoint> dataPoint = new Collection<DataPoint>();

            if (responseData != null)
            {
                if (selectedAxisLimits.Contains("ImpulseResponse"))
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
                    MathNet.Numerics.Complex32[] complexData = new MathNet.Numerics.Complex32[responseData.Length];

                    for (int j = 0; j < Frequency.Length; j++)
                    {
                        Frequency[j] = (double)j / responseData.Length * sampleRate;
                        complexData[j] = (MathNet.Numerics.Complex32)(responseData[j]);
                    }
                    Frequency[0] = 0.5 / responseData.Length * sampleRate;

                    MathNet.Numerics.IntegralTransforms.Fourier.Forward(complexData, MathNet.Numerics.IntegralTransforms.FourierOptions.NoScaling);

                    if (smoothingFactor != 0)
                    {
                        double[] smoothed = new double[responseData.Length];
                        for (int j = 0; j < responseData.Length; j++)
                        {
                            /* MagnitudeSquared if we take 10*log10 later */
                            smoothed[j] = complexData[j].MagnitudeSquared;
                        }

                        LinSpacedFracOctaveSmooth(49 - smoothingFactor, ref smoothed, 1, 1d / 48);

                        for (int x = 0; x < responseData.Length / 2; x++)
                        {
                            /* 10*log10 for previously MagnitudeSquared */
                            dataPoint.Add(new DataPoint(Frequency[x], ChLevel + 10 * Math.Log10(smoothed[x])));
                        }
                    }
                    else
                    {
                        for (int x = 0; x < responseData.Length / 2; x++)
                        {
                            /* 10*log10 for previously MagnitudeSquared */
                            dataPoint.Add(new DataPoint(Frequency[x], ChLevel + 10 * Math.Log10(complexData[x].MagnitudeSquared)));
                        }
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