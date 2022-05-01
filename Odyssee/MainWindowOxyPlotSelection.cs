using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void ListView_ResponseData_KeyUp(object sender, KeyEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (e.Key.Equals(Key.Escape))
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
    }
}