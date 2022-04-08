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

        private string ValueAxisLabelFormatter(double input)
        {
            return ("");
        }

        public void InitOxyPlotLvlm()
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

                audysseyMultEQAvr.ResetSPLAvg();
            }
        }

        public void AddOxyPlotLvlm()
        {
            if (PlotModel != null)
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