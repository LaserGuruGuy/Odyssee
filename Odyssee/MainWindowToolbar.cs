﻿using System.Windows;

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
            audysseyMultEQAvr.AvrInfo_IsChecked = false;
            audysseyMultEQAvr.AvrStatus_IsChecked = false;
            audysseyMultEQAvrTcp.GetAvrInfo(OnInspectorCmdResponseAvrInfo);
        }

        private void OnInspectorCmdResponseAvrInfo(string Response)
        {
            if (Response.Equals("ACK"))
            {
                audysseyMultEQAvrTcp.GetAvrStatus();
            }
        }

        private void OnButtonClick_Audyssey(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvr.AudysseyMode_IsChecked)
            {
                audysseyMultEQAvr.AudysseyMode_IsChecked = audysseyMultEQAvrTcp.EnterAudysseyMode();
            }
            else
            {
                audysseyMultEQAvr.AudysseyMode_IsChecked = !audysseyMultEQAvrTcp.ExitAudysseyMode();
            }
        }

        private void OnButtonClick_SubwooferOneLevel(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvr.AvrLvlm_SW1_IsChecked)
            {
                if (audysseyMultEQAvrTcp.StartLvLmSw1())
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
                }
                else
                {
                    audysseyMultEQAvr.AvrLvlm_SW1_IsChecked = false;
                }
            }
            else
            {
                if (audysseyMultEQAvrTcp.AbortOprt())
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
                }
            }
        }

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
                audysseyMultEQAvr.Serialized += Response + "\n";
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
                audysseyMultEQAvr.Serialized += Response + "\n";
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
                audysseyMultEQAvr.Serialized += Response + "\n";
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
                    audysseyMultEQAvrTcp.ExitAudysseyMode();
                }
            }
            else if (Response.Equals("TIMEOUT") && MessageBoxResult.OK == MessageBox.Show(
                        "Error during receiving responsedata: " + Response,
                        "Continue with last position ", MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                audysseyMultEQAvr.Serialized += Response + "\n";
                audysseyMultEQAvrTcp.GetRespon(OnCmdAckMicrophone_GetRespon);
            }
            else
            {
                audysseyMultEQAvr.Serialized += Response + "\n";
            }
        }

        private void OnButtonClick_Speaker(object sender, RoutedEventArgs e)
        {
            // TODO
            audysseyMultEQAvr.Serialized += "Not implemented\n";
        }
    }
}