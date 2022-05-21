using System.Threading.Tasks;
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
            audysseyMultEQAvrTcp.GetAvrInfo(OnInspectorCmdResponseAvrInfo);
        }

        private void OnInspectorCmdResponseAvrInfo(string Response)
        {
            if (Response.Equals("ACK"))
            {
                audysseyMultEQAvrTcp.GetAvrStatus(OnCmdResponse);
            }
        }

        private void OnButtonClick_Audyssey(object sender, RoutedEventArgs e)
        {
            if (ToggleButton_Audyssey.IsChecked == true)
            {
                audysseyMultEQAvrTcp.EnterAudysseyMode(OnCmdResponse);
            }
            else
            {
                audysseyMultEQAvrTcp.ExitAudysseyMode(OnCmdResponse);
            }
        }

        private void OnButtonClick_SubwooferOneLevel(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvr.AvrLvlm_SW1_IsChecked)
            {
                audysseyMultEQAvrTcp.StartLvLmSw1(OnCmdResponse);

                Label_SmoothingFactor.Visibility = Visibility.Hidden;
                Slider_SmoothingFactor.Visibility = Visibility.Hidden;

                CheckBox_LogarithmicAxis.Visibility = Visibility.Hidden;
                RadioButton_ImpulseResponse.Visibility = Visibility.Hidden;
                RadioButton_RangeSubw.Visibility = Visibility.Hidden;
                RadioButton_RangeFull.Visibility = Visibility.Hidden;

                CheckBox_CurveFilter.Visibility = Visibility.Hidden;
                RadioButton_FlatCurveFilter.Visibility = Visibility.Hidden;
                RadioButton_AudyCurveFilter.Visibility = Visibility.Hidden;
            }
            else
            {
                audysseyMultEQAvrTcp.AbortOprt(OnCmdResponse);

                Label_SmoothingFactor.Visibility = Visibility.Visible;
                Slider_SmoothingFactor.Visibility = Visibility.Visible;

                CheckBox_LogarithmicAxis.Visibility = Visibility.Visible;
                RadioButton_ImpulseResponse.Visibility = Visibility.Visible;
                RadioButton_RangeSubw.Visibility = Visibility.Visible;
                RadioButton_RangeFull.Visibility = Visibility.Visible;

                CheckBox_CurveFilter.Visibility = Visibility.Visible;
                RadioButton_FlatCurveFilter.Visibility = Visibility.Visible;
                RadioButton_AudyCurveFilter.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void OnButtonClick_Microphone(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvrTcp.EnterAudysseyMode(OnCmdResponseMicrophone_EnterAudysseyMode);
        }

        private void OnCmdResponseMicrophone_EnterAudysseyMode(string Response)
        {
            if (Response.Equals("ACK"))
            {
                audysseyMultEQAvrTcp.SetPosNum(OnCmdResponseMicrophone_SetPosNum);
            }
            else
            {
                audysseyMultEQAvr.StatusBar(Response);
            }
        }

        private void OnCmdResponseMicrophone_SetPosNum(string Response)
        {
            if (Response.Equals("ACK"))
            {
                audysseyMultEQAvrTcp.StartChnl(OnCmdResponseMicrophone_StartChnl);
            }
            else
            {
                audysseyMultEQAvr.StatusBar(Response);
            }
        }

        private void OnCmdResponseMicrophone_StartChnl(string Response)
        {
            if (Response.Equals("ACK"))
            {
                audysseyMultEQAvrTcp.GetRespon(OnCmdAckMicrophone_GetRespon);
            }
            else
            {
                audysseyMultEQAvr.StatusBar(Response);
            }
        }

        private void OnCmdAckMicrophone_GetRespon(string Response)
        {
            if (Response.Equals("ACK"))
            {
                if (audysseyMultEQAvr.IsNextGetRespon)
                {
                    audysseyMultEQAvrTcp.StartChnl(OnCmdResponseMicrophone_StartChnl);
                }
                else if (audysseyMultEQAvr.IsNextSetPosNum && MessageBoxResult.Yes == MessageBox.Show(
                        "Move microphone to next position: " + (audysseyMultEQAvr.DetectedChannels[0].ResponseData.Count + 1), 
                        "Proceed with callibration", MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    audysseyMultEQAvrTcp.SetPosNum(OnCmdResponseMicrophone_SetPosNum);
                }
                else
                {
                    audysseyMultEQAvrTcp.ExitAudysseyMode(OnCmdResponse);
                }
            }
            else if (Response.Equals("TIMEOUT") && MessageBoxResult.OK == MessageBox.Show(
                        "Error during receiving responsedata: " + Response,
                        "Continue with last position ", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                audysseyMultEQAvr.StatusBar(Response);
                audysseyMultEQAvrTcp.GetRespon(OnCmdAckMicrophone_GetRespon);
            }
            else
            {
                audysseyMultEQAvr.StatusBar(Response);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void OnButtonClick_Speaker(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvrTcp.EnterAudysseyMode(OnCmdResponseSpeaker_EnterAudysseyModeEscape);
        }

        private void OnCmdResponseSpeaker_EnterAudysseyModeEscape(string Response)
        {
            if (Response.Equals("ACK"))
            {
                audysseyMultEQAvrTcp.Escape(OnCmdResponseSpeaker_Escape);
            }
            else
            {
                audysseyMultEQAvr.StatusBar(Response);
            }
        }

        private void OnCmdResponseSpeaker_Escape(string Response)
        {
            if (Response.Equals("NACK"))
            {
                audysseyMultEQAvrTcp.GetAvrInfo(OnCmdResponseSpeaker_GetAvrInfo);
            }
            else
            {
                audysseyMultEQAvr.StatusBar(Response);
            }
        }

        private void OnCmdResponseSpeaker_GetAvrInfo(string Response)
        {
            if (Response.Equals("ACK"))
            {
                audysseyMultEQAvrTcp.EnterAudysseyMode(OnCmdResponseSpeaker_EnterAudysseyMode);
            }
            else
            {
                audysseyMultEQAvr.StatusBar(Response);
            }
        }

        private void OnCmdResponseSpeaker_EnterAudysseyMode(string Response)
        {
            if (Response.Equals("ACK"))
            {
                audysseyMultEQAvrTcp.SetAmp(OnCmdResponseSpeaker_SetAmp);
            }
            else
            {
                audysseyMultEQAvr.StatusBar(Response);
            }
        }

        private void OnCmdResponseSpeaker_SetAmp(string Response)
        {
            if (Response.Equals("ACK"))
            {
                audysseyMultEQAvrTcp.SetAudy(OnCmdResponseSpeaker_SetAudy);
            }
            else
            {
                audysseyMultEQAvr.StatusBar(Response);
            }
        }

        private void OnCmdResponseSpeaker_SetAudy(string Response)
        {
            if (Response.Equals("ACK"))
            {
                Task.Run(() => audysseyMultEQAvrTcp.SetAvrSetDisFil(OnCmdResponseSpeaker_SetAvrSetDisFil));
            }
            else
            {
                audysseyMultEQAvr.StatusBar(Response);
            }
        }

        private void OnCmdResponseSpeaker_SetAvrSetDisFil(string Response)
        {
            if (Response.Equals("ACK"))
            {
                if (audysseyMultEQAvr.SetDisFil_IsChecked)
                {
                    audysseyMultEQAvrTcp.SetAvrInitCoefs(OnCmdResponseSpeaker_SetAvrInitCoefs);
                }
            }
            else
            {
                audysseyMultEQAvr.StatusBar(Response);
            }
        }

        private void OnCmdResponseSpeaker_SetAvrInitCoefs(string Response)
        {
            if (Response.Equals("ACK"))
            {
                Task.Run(() => audysseyMultEQAvrTcp.SetAvrSetCoefDt(OnCmdResponseSpeaker_SetAvrSetCoefDt));
            }
            else
            {
                audysseyMultEQAvr.StatusBar(Response);
            }
        }

        private void OnCmdResponseSpeaker_SetAvrSetCoefDt(string Response)
        {
            if (Response.Equals("ACK"))
            {
                if(audysseyMultEQAvr.SetCoefDt_IsChecked)
                {
                    audysseyMultEQAvrTcp.AudyFinFlag(OnCmdResponseSpeaker_AudyFinFlag);
                }
            }
            else
            {
                audysseyMultEQAvr.StatusBar(Response);
            }
        }

        private void OnCmdResponseSpeaker_AudyFinFlag(string Response)
        {
            if (Response.Equals("ACK"))
            {
                audysseyMultEQAvrTcp.ExitAudysseyMode(OnCmdResponse);
            }
            else
            {
                audysseyMultEQAvr.StatusBar(Response);
            }
        }
    }
}