using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

        private string selectedCurveFilter = string.Empty;
        bool LogarithmicAxis = false;

        private Dictionary<string, Brush> ResponseDataTraceColor = new Dictionary<string, Brush> { { "0", Brushes.Black }, { "1", Brushes.Blue }, { "2", Brushes.Violet }, { "3", Brushes.Green }, { "4", Brushes.Orange }, { "5", Brushes.Red }, { "6", Brushes.Cyan }, { "7", Brushes.DeepPink } };
        private Dictionary<string, Brush> FlatCurveFilterTraceColor = new Dictionary<string, Brush> { { "coefficient48kHz", Brushes.BlueViolet }, { "coefficient441kHz", Brushes.BlueViolet }, { "coefficient32kHz", Brushes.BlueViolet }, { "dispLargeData", Brushes.BlueViolet }, { "dispSmallData", Brushes.BlueViolet } };
        private Dictionary<string, Brush> ReferenceCurveFilterTraceColor = new Dictionary<string, Brush> { { "coefficient48kHz", Brushes.Maroon }, { "coefficient441kHz", Brushes.Maroon }, { "coefficient32kHz", Brushes.Maroon }, { "dispLargeData", Brushes.Maroon }, { "dispSmallData", Brushes.Maroon } };

        private string selectedAxisLimits = "RadioButton_RangeChirp";
        private Dictionary<string, AxisLimit> AxisLimits = new Dictionary<string, AxisLimit>()
        {
            {"RadioButton_RangeFull", new AxisLimit { XMin = 10, XMax = 24000, YMin = -35, YMax = 20, MajorStep = 5, MinorStep = 1 } },
            {"RadioButton_RangeSubwoofer", new AxisLimit { XMin = 10, XMax = 1000, YMin = -35, YMax = 20, MajorStep = 5, MinorStep = 1 } },
            {"RadioButton_RangeChirp", new AxisLimit { XMin = 0, XMax = SecondsToMilliseconds*16384.0/48000.0, YMin = -0.1, YMax = 0.1, MajorStep = 0.01, MinorStep = 0.001 } }
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
                    PlotModel.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Bottom, Title = "ms", Minimum = Limits.XMin, Maximum = Limits.XMax, MajorGridlineStyle = LineStyle.Dot });
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
                    PlotModel.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Bottom, Title = "Hz", Minimum = Limits.XMin, Maximum = Limits.XMax, MajorGridlineStyle = LineStyle.Dot });
                }
                else
                {
                    PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Hz", Minimum = Limits.XMin, Maximum = Limits.XMax, MajorGridlineStyle = LineStyle.Dot });
                }
                PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "dB", Minimum = Limits.YMin, Maximum = Limits.YMax, MajorStep = Limits.MajorStep, MinorStep = Limits.MinorStep, MajorGridlineStyle = LineStyle.Solid });
            }
        }

        private void PlotLine(DetectedChannel selectedChannel, bool secondaryChannel = false, int SmoothingFactor = 0)
        {
            if (selectedChannel != null)
            {
                ///* Plot selected position: curve filter coefficients and response data */
                OxyColor CurveColor = new();

                /* Selected channel response data key and value are null if there is no channel selected in the GUI */
                if ((selectedChannel.SelectedResponseData.Key != null) && (selectedChannel.SelectedResponseData.Value != null))
                {
                    CurveColor = OxyColor.Parse(ResponseDataTraceColor[selectedChannel.SelectedResponseData.Key].ToString());
                    PlotCurve(48000d, selectedChannel.SelectedResponseData.Value, SmoothingFactor, CurveColor, secondaryChannel ? LineStyle.Dot : LineStyle.Solid, 1);
                }

                /* Iterate ove all the sticky positions */
                foreach (var stickyResponseData in selectedChannel.StickyResponseData)
                {
                    /* Sticky channel response data key and value are null if there is no channel selected in the GUI */
                    if ((stickyResponseData.Key != null) && (stickyResponseData.Value != null))
                    {
                        /* Avoid duplication of response data if the sticky channel was also a selected channel */
                        if (!stickyResponseData.Equals(selectedChannel.SelectedResponseData))
                        {
                            CurveColor = OxyColor.Parse(ResponseDataTraceColor[stickyResponseData.Key].ToString());
                            PlotCurve(48000d, stickyResponseData.Value, SmoothingFactor, CurveColor, secondaryChannel ? LineStyle.Dot : LineStyle.Solid, 1);
                        }
                    }
                }
            }
        }
        private void PlotCurve(double sampleRate, float[] responseData, int smoothingFactor, OxyColor oxyColor, LineStyle lineStyle, double strokeThickness)
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
                    complexData[j] = (Complex)responseData[j];
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
            if ((PlotModel.Title != "Subwoofer Level") && (audysseyMultEQAvr.SPLValue != 0))
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

                audysseyMultEQAvr.ResetSPLAvg();
            }
        }

        public void AddOxyPlotLvlm()
        {
            if ((PlotModel != null) && (audysseyMultEQAvr.SPLValue != 0))
            {
                if (PlotModel.Series != null)
                {
                    for(int i = 0; i < PlotModel.Series.Count; i++)
                    {
                        var s = (LineSeries)PlotModel.Series[i];
                        switch (i)
                        {
                            case 0:
                                s.Points.Add(new DataPoint(s.Points.Count, (double)audysseyMultEQAvr.SPLValuedB));
                                break;
                            case 1:
                                s.Points.Add(new DataPoint(s.Points.Count, 70));
                                break;
                            case 2:
                                s.Points.Add(new DataPoint(s.Points.Count, 80));
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