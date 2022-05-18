using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Audyssey.MultEQ.List;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;


namespace Odyssee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void ListView_ResponseData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawChart();
        }

        // find all T in the VisualTree
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            List<T> foundChilds = new List<T>();

            int childrenCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);

                T childType = child as T;
                if (childType == null)
                {
                    foreach (var other in FindVisualChildren<T>(child))
                        yield return other;
                }
                else
                {
                    yield return (T)child;
                }
            }
        }

        private void ListView_ResponseData_KeyUp(object sender, KeyEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (e.Key.Equals(Key.Space))
            {
                if (listBox.SelectedItem != null)
                {
                    IEnumerable<CheckBox> ChildCheckBoxes = FindVisualChildren<CheckBox>(listBox);
                    foreach (CheckBox cb in ChildCheckBoxes)
                    {
                        if (cb.Name == "Average")
                        {
                            if (((KeyValuePair<string, double[]>)cb.DataContext).Equals((KeyValuePair<string, double[]>)listBox.SelectedItem))
                            {
                                if (cb.IsChecked == true)
                                {
                                    cb.IsChecked = false;
                                }
                                else
                                {
                                    cb.IsChecked = true;
                                }
                            }
                        }
                    }
                }
            }
            else if (e.Key.Equals(Key.Escape))
            {
                listBox.SelectedIndex = -1;
                listBox.SelectedItem = new KeyValuePair<string, double[]>();
                DrawChart();
            }

        }

        private void ListView_CheckBox_ResponseDataSticky_Initialized(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            KeyValuePair<string, double[]> keyValuePair = (KeyValuePair<string, double[]>)checkBox.DataContext;
            checkBox.IsChecked = audysseyMultEQAvr.SelectedChannel.StickyResponseData.Contains(keyValuePair);
            checkBox.Background = ResponseDataTraceColor[keyValuePair.Key];
        }

        private void ListView_CheckBox_ResponseDataSticky_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            KeyValuePair<string, double[]> keyValuePair = (KeyValuePair<string, double[]>)checkBox.DataContext;
            if (!audysseyMultEQAvr.SelectedChannel.StickyResponseData.Contains(keyValuePair))
            {
                audysseyMultEQAvr.SelectedChannel.StickyResponseData.Add(keyValuePair);
                DrawChart();
            }
        }

        private void ListView_CheckBox_ResponseDataSticky_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            KeyValuePair<string, double[]> keyValuePair = (KeyValuePair<string, double[]>)checkBox.DataContext;
            audysseyMultEQAvr.SelectedChannel.StickyResponseData.Remove(keyValuePair);
            DrawChart();
        }

        private void ListView_CheckBox_ResponseDataAverage_Initialized(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            KeyValuePair<string, double[]> keyValuePair = (KeyValuePair<string, double[]>)checkBox.DataContext;
            checkBox.IsChecked = audysseyMultEQAvr.SelectedChannel.AverageResponseData.Contains(keyValuePair);
        }

        private void ListView_CheckBox_ResponseDataAverage_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            KeyValuePair<string, double[]> keyValuePair = (KeyValuePair<string, double[]>)checkBox.DataContext;
            if (!audysseyMultEQAvr.SelectedChannel.AverageResponseData.Contains(keyValuePair))
            {
                audysseyMultEQAvr.SelectedChannel.AverageResponseData.Add(keyValuePair);
                DrawChart();
            }
        }

        private void ListView_CheckBox_ResponseDataAverage_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            KeyValuePair<string, double[]> keyValuePair = (KeyValuePair<string, double[]>)checkBox.DataContext;
            audysseyMultEQAvr.SelectedChannel.AverageResponseData.Remove(keyValuePair);
            DrawChart();
        }

        private void ListView_FlatCurveFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawChart();
        }

        private void ListView_FlatCurveFilter_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (e.Key.Equals(Key.Escape))
            {
                listBox.SelectedIndex = -1;
                listBox.SelectedItem = new KeyValuePair<string, double[]>();
                DrawChart();
            }
        }

        private void ListView_AudyCurveFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawChart();
        }

        private void ListView_AudyCurveFilter_KeyUp(object sender, KeyEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (e.Key.Equals(Key.Escape))
            {
                listBox.SelectedIndex = -1;
                listBox.SelectedItem = new KeyValuePair<string, double[]>();
                DrawChart();
            }
        }

        private void RadioButton_Range_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            selectedAxisLimits = radioButton.Name;
            DrawChart();
        }

        private void RadioButton_CurveFilter_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            selectedCurveFilter = radioButton.Name;
            DrawChart();
        }

        private void CheckBox_CurveFilter_Checked(object sender, RoutedEventArgs e)
        {
            DrawChart();
        }

        private void CheckBox_CurveFilter_Unchecked(object sender, RoutedEventArgs e)
        {
            DrawChart();
        }

        private void Slider_SmoothingFactor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DrawChart();
        }

        private void CheckBox_LogarithmicAxis_Checked(object sender, RoutedEventArgs e)
        {
            LogarithmicAxis = true;
            DrawChart();
        }

        private void CheckBox_LogarithmicAxis_Unchecked(object sender, RoutedEventArgs e)
        {
            LogarithmicAxis = false;
            DrawChart();
        }

        private void Plot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Get the PlotView
            OxyPlot.Wpf.PlotView PlotView = sender as OxyPlot.Wpf.PlotView;

            //Get the plot point
            Point point = e.GetPosition(PlotView);

            //Get the axis point from the plot point
            DataPoint DataPoint = PlotView.ActualModel.DefaultXAxis.InverseTransform(point.X, point.Y, PlotView.ActualModel.DefaultYAxis);

            //Get the screen point
            ScreenPoint ScreenPoint = new ScreenPoint(point.X, point.Y);
            //Get the series
            Series Series = PlotView.ActualModel.GetSeriesFromPoint(ScreenPoint);

            if (Series != null)
            {
                TrackerHitResult TrackerHitResult = Series.GetNearestPoint(ScreenPoint, false);
                if (TrackerHitResult != null)
                {
                    // data point nearest to the click
                    DataPoint = new(TrackerHitResult.DataPoint.X, DataPoint.Y);

                    //Audy
                    if (Series.Title.Contains(MultEQList.AudyEqSetList[0]))
                    {
                        //dispLargeData
                        if (Series.Title.Contains(MultEQList.DispDataList[0]))
                        {
                            //Store curvepoint
                            audysseyMultEQAvr.CurvePoint = new(MultEQList.AudyEqSetList[0], MultEQList.DispDataList[0], DataPoint.X, DataPoint.Y);
                        }
                        //dispSmallData
                        if (Series.Title.Contains(MultEQList.DispDataList[1]))
                        {
                            //Store curvepoint
                            audysseyMultEQAvr.CurvePoint = new(MultEQList.AudyEqSetList[0], MultEQList.DispDataList[1], DataPoint.X, DataPoint.Y);
                        }
                    }

                    //Flat
                    if (Series.Title.Contains(MultEQList.AudyEqSetList[1]))
                    {
                        //dispLargeData
                        if (Series.Title.Contains(MultEQList.DispDataList[0]))
                        {
                            //Store curvepoint
                            audysseyMultEQAvr.CurvePoint = new(MultEQList.AudyEqSetList[1], MultEQList.DispDataList[0], DataPoint.X, DataPoint.Y);
                        }
                        //dispSmallData
                        if (Series.Title.Contains(MultEQList.DispDataList[1]))
                        {
                            //Store curvepoint
                            audysseyMultEQAvr.CurvePoint = new(MultEQList.AudyEqSetList[1], MultEQList.DispDataList[1], DataPoint.X, DataPoint.Y);
                        }
                    }
                }
            }
            else
            {
                audysseyMultEQAvr.CurvePoint = null;
            }
        }

        private void Plot_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            audysseyMultEQAvr.CurvePoint = null;
        }

        private void plot_MouseMove(object sender, MouseEventArgs e)
        {
            if (audysseyMultEQAvr.CurvePoint != null)
            {
                //Get the PlotView
                OxyPlot.Wpf.PlotView PlotView = sender as OxyPlot.Wpf.PlotView;

                //Get the plot point
                Point point = e.GetPosition(PlotView);

                //Get the axis point from the plot point
                DataPoint DataPoint = PlotView.ActualModel.DefaultXAxis.InverseTransform(point.X, point.Y, PlotView.ActualModel.DefaultYAxis);

                //Update the coordinate
                audysseyMultEQAvr.CurvePoint.Y = DataPoint.Y;

                //Audy
                if (audysseyMultEQAvr.CurvePoint.AudyEqSet.Equals(MultEQList.AudyEqSetList[0]))
                {
                    //dispLargeData
                    if (audysseyMultEQAvr.CurvePoint.DispData.Equals(MultEQList.DispDataList[0]))
                    {
                        int index = ChannelList.FilterFrequencies.IndexOf(audysseyMultEQAvr.CurvePoint.X);
                        audysseyMultEQAvr.SelectedChannel.AudyCurveFilter[MultEQList.DispDataList[0]].SetValue(audysseyMultEQAvr.CurvePoint.Y, index);
                        DrawChart();
                    }
                    //dispSmallData
                    if (audysseyMultEQAvr.CurvePoint.DispData.Equals(MultEQList.DispDataList[1]))
                    {
                        int index = ChannelList.DisplayFrequencies.IndexOf(audysseyMultEQAvr.CurvePoint.X);
                        audysseyMultEQAvr.SelectedChannel.AudyCurveFilter[MultEQList.DispDataList[1]].SetValue(audysseyMultEQAvr.CurvePoint.Y, index);
                        DrawChart();
                    }
                }

                //Flat
                if (audysseyMultEQAvr.CurvePoint.AudyEqSet.Equals(MultEQList.AudyEqSetList[1]))
                {
                    //dispLargeData
                    if (audysseyMultEQAvr.CurvePoint.DispData.Equals(MultEQList.DispDataList[0]))
                    {
                        int index = ChannelList.FilterFrequencies.IndexOf(audysseyMultEQAvr.CurvePoint.X);
                        audysseyMultEQAvr.SelectedChannel.FlatCurveFilter[MultEQList.DispDataList[0]].SetValue(audysseyMultEQAvr.CurvePoint.Y, index);
                        DrawChart();
                    }
                    //dispSmallData
                    if (audysseyMultEQAvr.CurvePoint.DispData.Equals(MultEQList.DispDataList[1]))
                    {
                        int index = ChannelList.DisplayFrequencies.IndexOf(audysseyMultEQAvr.CurvePoint.X);
                        audysseyMultEQAvr.SelectedChannel.FlatCurveFilter[MultEQList.DispDataList[1]].SetValue(audysseyMultEQAvr.CurvePoint.Y, index);
                        DrawChart();
                    }
                }
            }
        }
    }
}