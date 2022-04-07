using System.Windows;

namespace Odyssee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void OnButtonClick_ConnectReceiver(object sender, RoutedEventArgs e)
        {
            connectReceiver.IsChecked = !connectReceiver.IsChecked;
            ConnectReceiver();
        }

        private void OnButtonClick_Inspector(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvrTcp != null)
            {
                audysseyMultEQAvrTcp.GetAvrInfo(OnInspectorCmdAckAvrInfo);
            }
            else
            {
                ReceiverInfo.IsChecked = false;
                ReceiverStatus.IsChecked = false;
                audysseyMultEQAvr.Serialized += "No receiver connected\n";
            }
        }

        public void OnInspectorCmdAckAvrInfo(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvr.AvrInfo_IsChecked = true;
                audysseyMultEQAvrTcp.GetAvrStatus(OnCmdAckAvrStatus);
            }
            else
            {
                audysseyMultEQAvr.AvrInfo_IsChecked = false;
                audysseyMultEQAvr.AvrStatus_IsChecked = false;
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        private void OnButtonClick_SubwooferLevel(object sender, RoutedEventArgs e)
        {
            if (AvrLvLm.IsChecked)
            {
                if (audysseyMultEQAvrTcp != null)
                {
                    audysseyMultEQAvrTcp.AbortOprt(OnCmdAckAbortOprt);
                }
                else
                {
                    audysseyMultEQAvr.Serialized += "No receiver connected\n";
                }
            }
            else
            {
                if (audysseyMultEQAvrTcp != null)
                {
                    audysseyMultEQAvrTcp.StartLvLm(OnCmdAckLvLm);
                }
                else
                {
                    audysseyMultEQAvr.Serialized += "No receiver connected\n";
                }
            }
        }

        private void OnButtonClick_Microphone(object sender, RoutedEventArgs e)
        {
            // TODO
            audysseyMultEQAvr.Serialized += "Not implemented\n";
        }

        private void OnButtonClick_Speaker(object sender, RoutedEventArgs e)
        {
            // TODO
            audysseyMultEQAvr.Serialized += "Not implemented\n";
        }
    }
}