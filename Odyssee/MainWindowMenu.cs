using System;
using System.Windows;
using Audyssey.MultEQTcp;
using Audyssey.MultEQTcpSniffer;
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
        private void MenuItem_DetectReceiver_OnClick(object sender, RoutedEventArgs e)
        {
            SearchForReceiverIpAddress(cmbInterfaceComputer.Text.IndexOf(' ') > -1 ? cmbInterfaceComputer.Text.Substring(0, cmbInterfaceComputer.Text.IndexOf(' ')) : cmbInterfaceComputer.Text);
        }

        private void MenuItem_ConnectReceiver_OnClick(object sender, RoutedEventArgs e)
        {
            ConnectReceiver();
        }

        private void ConnectReceiver()
        {
            if (connectReceiver.IsChecked)
            {
                // if there is no IP address text
                if (string.IsNullOrEmpty(cmbInterfaceReceiver.Text))
                {
                    // uncheck the menu item
                    connectReceiver.IsChecked = false;
                    // display message to report error to user
                    MessageBox.Show("Enter select a valid receiver IP address.", "No valid receiver IP address found.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // receiver object does not exist
                    if (audysseyMultEQAvrTcp == null)
                    {
                        // create receiver tcp instance and strip receiver name and keep receiver IP Address
                        audysseyMultEQAvrTcp = new AudysseyMultEQAvrTcp(ref audysseyMultEQAvr, cmbInterfaceReceiver.Text.IndexOf(' ') > -1 ? cmbInterfaceReceiver.Text.Substring(0, cmbInterfaceReceiver.Text.IndexOf(' ')) : cmbInterfaceReceiver.Text);
                        // open connection to receiver
                        audysseyMultEQAvrTcp.Open();
                    }
                }
            }
            else
            {
                if (audysseyMultEQAvrTcp != null)
                {
                    audysseyMultEQAvrTcp.Close();
                    // immediately clean up the object
                    audysseyMultEQAvrTcp = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }

        private void MenuItem_ConnectSniffer_OnClick(object sender, RoutedEventArgs e)
        {
            ConnectSniffer();
        }

        private void ConnectSniffer()
        {
            if (connectSniffer.IsChecked)
            {
                // sniffer must be elevated to capture raw packets
                if (!IsElevated())
                {
                    // we cannot create the sniffer...
                    connectSniffer.IsChecked = false;
                    // but we can ask the user to elevate the program!
                    RunAsAdmin();
                }
                else
                {
                    // sniffer object does not exist
                    if (audysseyMultEQTcpSniffer == null)
                    {
                        //
                        if (audysseyMultEQAvrTcp != null)
                        {
                            // create sniffer attached to receiver and tcp
                            // strip computer ethernet adapter name and keep computer IP Address
                            // strip receiver name and keep receiver IP Address
                            audysseyMultEQTcpSniffer = new AudysseyMultEQTcpSniffer(
                                cmbInterfaceComputer.Text.IndexOf(' ') > -1 ? cmbInterfaceComputer.Text.Substring(0, cmbInterfaceComputer.Text.IndexOf(' ')) : cmbInterfaceComputer.Text,
                                cmbInterfaceReceiver.Text.IndexOf(' ') > -1 ? cmbInterfaceReceiver.Text.Substring(0, cmbInterfaceReceiver.Text.IndexOf(' ')) : cmbInterfaceReceiver.Text,
                                0, 1256, 0, 0, audysseyMultEQAvrTcp.Log, null, null, audysseyMultEQAvrTcp.Populate);
                            // open connection to receiver
                            audysseyMultEQTcpSniffer.Open();
                        }
                        else
                        {
                            // or we cannot connect because the receiver was not connected to begin with
                            MessageBox.Show("Connnect to receiver IP address prior to attaching sniffer.", "Could not attach sniffer to receiver.", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                if (audysseyMultEQTcpSniffer != null)
                {
                    audysseyMultEQTcpSniffer.Close();
                    // immediately clean up the object
                    audysseyMultEQTcpSniffer = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }

        private void MenuItem_AvrInfo_OnClick(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvrTcp != null)
            {
                audysseyMultEQAvrTcp.GetAvrInfo(OnCmdAckAvrInfo);
            }
            else
            {
                ReceiverInfo.IsChecked = false;
                audysseyMultEQAvr.Serialized += "No receiver connected\n";
            }
        }

        public void OnCmdAckAvrInfo(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvr.AvrInfo_IsChecked = true;
            }
            else
            {
                audysseyMultEQAvr.AvrInfo_IsChecked = false;
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        private void MenuItem_AvrStatus_OnClick(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvrTcp != null)
            {
                audysseyMultEQAvrTcp.GetAvrStatus(OnCmdAckAvrStatus);
            }
            else
            {
                ReceiverStatus.IsChecked = false;
                audysseyMultEQAvr.Serialized += "No receiver connected\n";
            }
        }

        public void OnCmdAckAvrStatus(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvr.AvrStatus_IsChecked = true;
            }
            else
            {
                audysseyMultEQAvr.AvrStatus_IsChecked = false;
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        private void MenuItem_AudysseyMode_OnClick(object sender, RoutedEventArgs e)
        {
            if (AudysseyMode.IsChecked)
            {
                if (audysseyMultEQAvrTcp != null)
                {
                    audysseyMultEQAvrTcp.EnterAudysseyMode(OnCmdAckEnterAudysseyMode);
                }
                else
                {
                    AudysseyMode.IsChecked = false;
                    audysseyMultEQAvr.Serialized += "No receiver connected\n";
                }
            }
            else
            {
                if (audysseyMultEQAvrTcp != null)
                {
                    audysseyMultEQAvrTcp.ExitAudysseyMode(OnCmdAckExitAudysseyMode);
                }
                else
                {
                    AudysseyMode.IsChecked = false;
                }
            }
        }

        public void OnCmdAckEnterAudysseyMode(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvr.AudysseyMode_IsChecked = true;
            }
            else
            {
                audysseyMultEQAvr.AudysseyMode_IsChecked = false;
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        public void OnCmdAckExitAudysseyMode(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvr.AudysseyMode_IsChecked = false;
                audysseyMultEQAvr.AvrInfo_IsChecked = false;
                audysseyMultEQAvr.AvrStatus_IsChecked = false;
                audysseyMultEQAvr.AvrLvlm_IsChecked = false;
                audysseyMultEQAvr.AudyFinFlag_IsChecked = false;
            }
            else
            {
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        private void MenuItem_AvrLvLm_OnClick(object sender, RoutedEventArgs e)
        {
            if (AvrLvLm.IsChecked)
            {
                if (audysseyMultEQAvrTcp != null)
                {
                    audysseyMultEQAvrTcp.StartLvLm(OnCmdAckLvLm);
                    InitOxyPlotLvlm();
                }
                else
                {
                    AvrLvLm.IsChecked = false;
                    audysseyMultEQAvr.Serialized += "No receiver connected\n";
                }
            }
            else
            {
                if (audysseyMultEQAvrTcp != null)
                {
                    audysseyMultEQAvrTcp.AbortOprt(OnCmdAckAbortOprt);
                }
                else
                {
                    AvrLvLm.IsChecked = false;
                    audysseyMultEQAvr.Serialized += "No receiver connected\n";
                }
            }
        }

        public void InitOxyPlotLvlm()
        {
            PlotModel.Axes.Clear();
            PlotModel.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Left});

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
                        var y = audysseyMultEQAvr.SPLValue > 0 ? Math.Log10((double)audysseyMultEQAvr.SPLValue) : -Math.Log10((double)(0-audysseyMultEQAvr.SPLValue));
                        Console.WriteLine(y);
                        s.Points.Add(new DataPoint(s.Points.Count, y));
                        PlotModel.InvalidatePlot(true);
                    }
                }
            }
        }

        public void OnCmdAckLvLm(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvr.AvrLvlm_IsChecked = true;
                AddOxyPlotLvlm();
            }
            else
            {
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        private void MenuItem_AvrAbortOprt_OnClick(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvrTcp != null)
            {
                audysseyMultEQAvrTcp.AbortOprt(OnCmdAckAbortOprt);
            }
            else
            {
                AvrAbortOprt.IsChecked = false;
                audysseyMultEQAvr.Serialized += "No receiver connected\n";
            }
        }

        public void OnCmdAckAbortOprt(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvr.AvrLvlm_IsChecked = false;
            }
            else
            {
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        private void MenuItem_SetAvrSetPosNum_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO
            audysseyMultEQAvr.Serialized += "Not implemented\n";
        }

        private void MenuItem_SetAvrStartChnl_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO
            audysseyMultEQAvr.Serialized += "Not implemented\n";
        }

        private void MenuItem_SetAvrGetRespon_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO
            audysseyMultEQAvr.Serialized += "Not implemented\n";
        }

        private void MenuItem_SetAvrSetAmp_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO
            audysseyMultEQAvr.Serialized += "Not implemented\n";
        }

        private void MenuItem_SetAvrSetAudy_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO
            audysseyMultEQAvr.Serialized += "Not implemented\n";
        }

        private void MenuItem_SetAvrSetDisFil_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO
            audysseyMultEQAvr.Serialized += "Not implemented\n";
        }

        private void MenuItem_SetAvrInitCoefs_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO
            audysseyMultEQAvr.Serialized += "Not implemented\n";
        }

        private void MenuItem_SetAvrSetCoefDt_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO
            audysseyMultEQAvr.Serialized += "Not implemented\n";
        }

        private void MenuItem_AudyFinFlag_OnClick(object sender, RoutedEventArgs e)
        {
            {
                if (audysseyMultEQAvrTcp != null)
                {
                    audysseyMultEQAvrTcp.AudyFinFlag(OnCmdAckAudyFinFlag);
                }
                else
                {
                    AudyFinFlag.IsChecked = false;
                    audysseyMultEQAvr.Serialized += "No receiver connected\n";
                }
            }
        }

        public void OnCmdAckAudyFinFlag(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvr.AudyFinFlag_IsChecked = true;
            }
            else
            {
                audysseyMultEQAvr.AudyFinFlag_IsChecked = false;
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }
    }
}