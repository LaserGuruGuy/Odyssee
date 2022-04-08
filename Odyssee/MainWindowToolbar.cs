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
            ConnectReceiver();
        }

        private void OnButtonClick_Inspector(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvrTcp != null)
            {
                audysseyMultEQAvrTcp.GetAvrInfo(OnInspectorCmdAckAvrInfo);
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
                audysseyMultEQAvr.Serialized += "Failed\n";
            }
        }

        private void OnButtonClick_Audyssey(object sender, RoutedEventArgs e)
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

        private void OnButtonClick_SubwooferLevel(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvrTcp != null)
            {
                if (audysseyMultEQAvr.AvrLvlm_IsChecked)
                {
                    audysseyMultEQAvrTcp.StartLvLm(OnCmdAckLvLm);
                    InitOxyPlotLvlm();
                }
                else
                {
                    audysseyMultEQAvrTcp.AbortOprt(OnCmdAckAbortOprt);
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