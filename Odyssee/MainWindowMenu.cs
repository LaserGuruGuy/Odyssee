using System;
using System.Windows;
using Audyssey.MultEQTcp;
using Audyssey.MultEQTcpSniffer;

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
            if (audysseyMultEQAvr.AvrConnect_IsChecked)
            {
                // if there is no IP address text
                if (string.IsNullOrEmpty(cmbInterfaceReceiver.Text))
                {
                    // uncheck the menu item
                    audysseyMultEQAvr.AvrConnect_IsChecked = false;
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
                    audysseyMultEQAvr.Reset();
                    // close the connection
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
            if (audysseyMultEQAvr.SnifferAttach_IsChecked)
            {
                // sniffer must be elevated to capture raw packets
                if (!IsElevated())
                {
                    // we cannot create the sniffer...
                    audysseyMultEQAvr.SnifferAttach_IsChecked = false;
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
                                0, 1256, 0, 0, audysseyMultEQAvrTcp.AvrSnifferCallback, null, null, audysseyMultEQAvrTcp.Populate);
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
        }

        public void OnCmdAckAvrInfo(bool IsAck)
        {
            if (!IsAck)
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
        }

        public void OnCmdAckAvrStatus(bool IsAck)
        {
            if (!IsAck)
            {
                audysseyMultEQAvr.AvrStatus_IsChecked = false;
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        private void MenuItem_AudysseyMode_OnClick(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvrTcp != null)
            {
                if (audysseyMultEQAvr.AudysseyMode_IsChecked)
                {
                    audysseyMultEQAvrTcp.EnterAudysseyMode(OnCmdAckEnterAudysseyMode);
                }
                else
                {
                    audysseyMultEQAvrTcp.ExitAudysseyMode(OnCmdAckExitAudysseyMode);
                }
            }
        }

        public void OnCmdAckEnterAudysseyMode(bool IsAck)
        {
            if (!IsAck)
            {
                audysseyMultEQAvr.AudysseyMode_IsChecked = false;
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        public void OnCmdAckExitAudysseyMode(bool IsAck)
        {
            if (!IsAck)
            {
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        private void MenuItem_AvrLvLm_OnClick(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvrTcp != null)
            {
                if (audysseyMultEQAvr.AvrLvlm_IsChecked)
                {
                    audysseyMultEQAvrTcp.StartLvLm(OnCmdAckLvLm);
                }
                else
                {
                    audysseyMultEQAvrTcp.AbortOprt(OnCmdAckAbortOprt);
                }
            }
        }

        public void OnCmdAckLvLm(bool IsAck)
        {
            if (!IsAck)
            {
                audysseyMultEQAvr.AvrLvlm_IsChecked = false;
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        public void OnCmdAckAbortOprt(bool IsAck)
        {
            if (!IsAck)
            {
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        private void MenuItem_SetAvrSetPosNum_OnClick(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvrTcp != null)
            {
                audysseyMultEQAvrTcp.SetPosNum(OnCmdAckSetPosNum);
            }
        }

        public void OnCmdAckSetPosNum(bool IsAck)
        {
            if (!IsAck)
            {
                audysseyMultEQAvr.SetPosNum_IsChecked = false;
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        private void MenuItem_SetAvrStartChnl_OnClick(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvrTcp != null)
            {
                audysseyMultEQAvrTcp.StartChnl(OnCmdAckStartChnl);
            }
        }

        public void OnCmdAckStartChnl(bool IsAck)
        {
            if (!IsAck)
            {
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        private void MenuItem_SetAvrGetRespon_OnClick(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvrTcp != null)
            {
                audysseyMultEQAvrTcp.GetRespon(OnCmdAckGetRespon);
            }
        }

        public void OnCmdAckGetRespon(bool IsAck)
        {
            if (!IsAck)
            {
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        private void MenuItem_SetAvrSetAmp_OnClick(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvrTcp != null)
            {
                audysseyMultEQAvrTcp.SetAmp(OnCmdAckSetAmp);
            }
            else
            {
                audysseyMultEQAvr.SetAmp_IsChecked = false;
            }
        }

        public void OnCmdAckSetAmp(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvr.SetAmp_IsChecked = true;
            }
            else
            {
                audysseyMultEQAvr.SetAmp_IsChecked = false;
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        private void MenuItem_SetAvrSetAudy_OnClick(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvrTcp != null)
            {
                audysseyMultEQAvrTcp.SetAudy(OnCmdAckSetAudy);
            }
            else
            {
                audysseyMultEQAvr.SetAudy_IsChecked = false;
            }
        }

        public void OnCmdAckSetAudy(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvr.SetAudy_IsChecked = true;
            }
            else
            {
                audysseyMultEQAvr.SetAudy_IsChecked = false;
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
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
            }
        }

        public void OnCmdAckAudyFinFlag(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvr.AudyFinFlag_IsChecked = true;
            }
            {
                audysseyMultEQAvr.AudyFinFlag_IsChecked = false;
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }
    }
}