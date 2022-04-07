using System;
using System.Windows;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Odyssee
{
    public partial class MainWindow : Window
    {
        public PlotModel PlotModel { get; private set; } = new();

        public void InitOxyPlotLvlm()
        {
            PlotModel.Axes.Clear();
            PlotModel.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Left });

            LineSeries Lineserie = new()
            {
                DataFieldX = "Time",
                DataFieldY = "dB",
                StrokeThickness = 2,
                MarkerSize = 0,
                LineStyle = LineStyle.Solid,
                Color = OxyColor.FromRgb(0, 0, 0),
                MarkerType = MarkerType.None,
            };

            PlotModel.Series.Clear();
            PlotModel.Series.Add(Lineserie);

            PlotModel.InvalidatePlot(true);
        }

        public void AddOxyPlotLvlm()
        {
            if (PlotModel != null)
            {
                if (PlotModel.Series != null)
                {
                    if (PlotModel.Series.Count > 0)
                    {
                        var s = (LineSeries)PlotModel.Series[0];
                        var y = audysseyMultEQAvr.SPLValue > 0 ? Math.Log10((double)audysseyMultEQAvr.SPLValue) : 0f - Math.Log10((double)(0 - audysseyMultEQAvr.SPLValue));
                        Console.WriteLine("audysseyMultEQAvr.SPLValue " + audysseyMultEQAvr.SPLValue + " " + y);
                        // TODO how to fix negative dB to show on Oxyplot?
                        // Add half-scale and set vertical axis limits from max neg to max pos?
                        s.Points.Add(new DataPoint(s.Points.Count, y));
                        PlotModel.InvalidatePlot(true);
                    }
                }
            }
        }
    }
}
