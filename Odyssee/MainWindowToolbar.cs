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
                    Label_SmoothingFactor.Visibility = Visibility.Hidden;
                    Slider_SmoothingFactor.Visibility = Visibility.Hidden;

                    CheckBox_LogarithmicAxis.Visibility = Visibility.Hidden;
                    RadioButton_RangeChirp.Visibility = Visibility.Hidden;
                    RadioButton_RangeSubwoofer.Visibility = Visibility.Hidden;
                    RadioButton_RangeFull.Visibility = Visibility.Hidden;

                    CheckBox_CurveFilter.Visibility = Visibility.Hidden;
                    RadioButton_FlatCurveFilter.Visibility = Visibility.Hidden;
                    RadioButton_ReferenceCurveFilter.Visibility = Visibility.Hidden;

                    audysseyMultEQAvrTcp.StartLvLm(OnCmdAckLvLm);
                }
                else
                {
                    Label_SmoothingFactor.Visibility = Visibility.Visible;
                    Slider_SmoothingFactor.Visibility = Visibility.Visible;

                    CheckBox_LogarithmicAxis.Visibility = Visibility.Visible;
                    RadioButton_RangeChirp.Visibility = Visibility.Visible;
                    RadioButton_RangeSubwoofer.Visibility = Visibility.Visible;
                    RadioButton_RangeFull.Visibility = Visibility.Visible;

                    CheckBox_CurveFilter.Visibility = Visibility.Visible;
                    RadioButton_FlatCurveFilter.Visibility = Visibility.Visible;
                    RadioButton_ReferenceCurveFilter.Visibility = Visibility.Visible;

                    audysseyMultEQAvrTcp.AbortOprt(OnCmdAckAbortOprt);
                }
            }
        }

        private void OnButtonClick_Microphone(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvrTcp != null)
            {
                audysseyMultEQAvr.DetectedChannels = null;
                audysseyMultEQAvrTcp.EnterAudysseyMode(OnCmdAckMicrophone_EnterAudysseyMode);
            }
        }

        public void OnCmdAckMicrophone_EnterAudysseyMode(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvrTcp.SetPosNum(OnCmdAckMicrophone_SetPosNum);
            }
            else
            {
                audysseyMultEQAvr.Serialized += "Failed\r\n";
            }
        }

        public void OnCmdAckMicrophone_SetPosNum(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvrTcp.StartChnl(OnCmdAckMicrophone_StartChnl);
            }
        }

        public void OnCmdAckMicrophone_StartChnl(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvrTcp.GetRespon(OnCmdAckMicrophone_GetRespon);
            }
        }

         public void OnCmdAckMicrophone_GetRespon(bool IsAck)
        {
            if (IsAck)
            {
                if (audysseyMultEQAvr.IsNextGetRespon)
                {
                    audysseyMultEQAvrTcp.StartChnl(OnCmdAckMicrophone_StartChnl);
                }
                else if (audysseyMultEQAvr.IsNextSetPosNum)
                {
                    MessageBoxResult result = MessageBox.Show("Proceed callibration", "Next position: " + audysseyMultEQAvr.DetectedChannels[0].ResponseData.Count, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.OK)
                    {
                        audysseyMultEQAvrTcp.SetPosNum(OnCmdAckMicrophone_SetPosNum);
                    }
                }
                audysseyMultEQAvrTcp.ExitAudysseyMode(OnCmdAckMicrophone_ExitAudysseyMode);
            }
        }

        public void OnCmdAckMicrophone_ExitAudysseyMode(bool IsAck)
        {

        }

        private void OnButtonClick_Speaker(object sender, RoutedEventArgs e)
        {
            // TODO
            audysseyMultEQAvr.Serialized += "Not implemented\n";
        }
    }
}