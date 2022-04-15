﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Audyssey.MultEQAvr;
using Audyssey.MultEQTcp;
using Audyssey.MultEQTcpSniffer;

namespace Odyssee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AudysseyMultEQAvr audysseyMultEQAvr = null;
        private AudysseyMultEQAvrTcp audysseyMultEQAvrTcp = null;
        private AudysseyMultEQTcpSniffer audysseyMultEQTcpSniffer = null;

        public MainWindow()
        {
            InitializeComponent();

            SearchForComputerIpAddress();

            audysseyMultEQAvr = new();
            audysseyMultEQAvr.PropertyChanged += PropertyChanged;
            audysseyMultEQAvr.AvrStatus.PropertyChanged += PropertyChanged;
            audysseyMultEQAvr.AvrInfo.PropertyChanged += PropertyChanged;

            this.DataContext = audysseyMultEQAvr;
        }

        private static void RunAsAdmin()
        {
            try
            {
                var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                using var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path, "/run_elevated_action")
                {
                    Verb = "runas"
                });
                process?.WaitForExit();

            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 1223)
                {
                    MessageBox.Show("Run the app elevated.", "Elevated rights required to capture packets from a raw socket.", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    throw;
                }
            }
        }

        private static bool IsElevated()
        {
            using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);

            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        private void HandleDroppedFile(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in files)
                {
                    // TODO: Implementation of Open
                }
            }
        }

        private void ComboBox_InterfaceComputer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (comboBox.Items.Count > 0)
            {
                string selectedHost = (string)comboBox.SelectedItem;
                selectedHost = selectedHost.Substring(0, selectedHost.IndexOf('|')).TrimEnd(' ');

                SearchForReceiverIpAddress(selectedHost);
            }
        }

        private void ScrollViewer_Serialized_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;

            if (e.HeightChanged)
            {
                scrollViewer.ScrollToEnd();
            }
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine(e.PropertyName);
            if (e.PropertyName.Equals("SPLValue"))
            {
                InitOxyPlotLvlm();
                AddOxyPlotLvlm();
            }
            if (e.PropertyName.Equals("ChSetup"))
            {
                audysseyMultEQAvr.DetectedChannels = null;
            }
        }

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
                listBox.SelectedItem = new KeyValuePair<string, string[]>();
                DrawChart();
            }
        }

        private void ListView_CheckBox_Checked_ResponseDataSticky(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            KeyValuePair<string, float[]> keyValuePair = (KeyValuePair<string, float[]>)checkBox.DataContext;
            if (!audysseyMultEQAvr.SelectedChannel.StickyResponseData.Contains(keyValuePair))
            {
                audysseyMultEQAvr.SelectedChannel.StickyResponseData.Add(keyValuePair);
                DrawChart();
            }
        }

        private void ListView_CheckBox_Unchecked_ResponseDataSticky(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            KeyValuePair<string, float[]> keyValuePair = (KeyValuePair<string, float[]>)checkBox.DataContext;
            audysseyMultEQAvr.SelectedChannel.StickyResponseData.Remove(keyValuePair);
            DrawChart();
        }

        private void ListView_CheckBox_Initialized_ResponseDataSticky(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            KeyValuePair<string, float[]> keyValuePair = (KeyValuePair<string, float[]>)checkBox.DataContext;
            checkBox.IsChecked = audysseyMultEQAvr.SelectedChannel.StickyResponseData.Contains(keyValuePair);
            checkBox.Background = ResponseDataTraceColor[keyValuePair.Key];
        }

        private void ListView_CheckBox_KeyUp_ResponseDataSticky(object sender, KeyEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (e.Key.Equals(Key.Escape))
            {
                listBox.SelectedIndex = -1;
                listBox.SelectedItem = new KeyValuePair<string, string[]>();
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
