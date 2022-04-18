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
            audysseyMultEQAvrTcp?.GetAvrInfo(OnInspectorCmdAckAvrInfo);
        }

        private void OnInspectorCmdAckAvrInfo(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvrTcp?.GetAvrStatus();
            }
        }

        private void OnButtonClick_Audyssey(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvr.AudysseyMode_IsChecked)
            {
                audysseyMultEQAvrTcp?.EnterAudysseyMode();
            }
            else
            {
                audysseyMultEQAvrTcp?.ExitAudysseyMode();
            }
        }

        private void OnButtonClick_SubwooferLevel(object sender, RoutedEventArgs e)
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
                RadioButton_AudyCurveFilter.Visibility = Visibility.Hidden;

                audysseyMultEQAvrTcp?.StartLvLm();
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
                RadioButton_AudyCurveFilter.Visibility = Visibility.Visible;

                audysseyMultEQAvrTcp?.AbortOprt();
            }
        }

        private void OnButtonClick_Microphone(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvr.DetectedChannels = null;
            audysseyMultEQAvrTcp?.EnterAudysseyMode(OnCmdAckMicrophone_EnterAudysseyMode);
        }

        private void OnCmdAckMicrophone_EnterAudysseyMode(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvrTcp?.SetPosNum(OnCmdAckMicrophone_SetPosNum);
            }
        }

        private void OnCmdAckMicrophone_SetPosNum(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvrTcp?.StartChnl(OnCmdAckMicrophone_StartChnl);
            }
        }

        private void OnCmdAckMicrophone_StartChnl(bool IsAck)
        {
            if (IsAck)
            {
                audysseyMultEQAvrTcp?.GetRespon(OnCmdAckMicrophone_GetRespon);
            }
        }

        private void OnCmdAckMicrophone_GetRespon(bool IsAck)
        {
            if (IsAck)
            {
                if (audysseyMultEQAvr.IsNextGetRespon)
                {
                    audysseyMultEQAvrTcp?.StartChnl(OnCmdAckMicrophone_StartChnl);
                }
                else if (audysseyMultEQAvr.IsNextSetPosNum && MessageBoxResult.Yes == MessageBox.Show(
                        "Move microphone to next position: " + (audysseyMultEQAvr.DetectedChannels[0].ResponseData.Count + 1), 
                        "Proceed with callibration", MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    audysseyMultEQAvrTcp?.SetPosNum(OnCmdAckMicrophone_SetPosNum);
                }
                else
                {
                    audysseyMultEQAvrTcp?.ExitAudysseyMode();
                }
            }
        }

        private void OnButtonClick_Speaker(object sender, RoutedEventArgs e)
        {
            // TODO
            audysseyMultEQAvr.Serialized += "Not implemented\n";
        }
    }
}