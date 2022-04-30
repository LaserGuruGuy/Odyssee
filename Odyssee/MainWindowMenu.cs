using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using System.Windows;
using Audyssey.MultEQ.List;
using Audyssey.MultEQTcp;
using Audyssey.MultEQTcpSniffer;
using MathNet.Numerics.IntegralTransforms;
using NAudio.Wave;

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
            // uncheck the menu item
            audysseyMultEQAvr.AvrConnect_IsChecked = false;
            // receiver object does not exist
            if (audysseyMultEQAvrTcp == null)
            {
                // if there is no IP address text
                if (string.IsNullOrEmpty(cmbInterfaceReceiver.Text))
                {
                    // display message to report error to user
                    MessageBox.Show("Enter select a valid receiver IP address.", "No valid receiver IP address found.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // create receiver tcp instance and strip receiver name and keep receiver IP Address
                    audysseyMultEQAvrTcp = new AudysseyMultEQAvrTcp(ref audysseyMultEQAvr, cmbInterfaceReceiver.Text.IndexOf(' ') > -1 ? cmbInterfaceReceiver.Text.Substring(0, cmbInterfaceReceiver.Text.IndexOf(' ')) : cmbInterfaceReceiver.Text);
                    // open connection to receiver
                    audysseyMultEQAvrTcp.Open();
                }
            }
            else
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

        private void MenuItem_ConnectSniffer_OnClick(object sender, RoutedEventArgs e)
        {
            ConnectSniffer();
        }

        private void ConnectSniffer()
        {
            // uncheck the menu item
            audysseyMultEQAvr.SnifferAttach_IsChecked = false;
            // sniffer object does not exist
            if (audysseyMultEQTcpSniffer == null)
            { 
                // sniffer must be elevated to capture raw packets
                if (!IsElevated())
                {
                    // but we can ask the user to elevate the program!
                    RunAsAdmin();
                }
                else
                {
                    if (audysseyMultEQAvrTcp == null)
                    {
                        audysseyMultEQAvrTcp = new AudysseyMultEQAvrTcp(ref audysseyMultEQAvr);
                    }
                    // create sniffer attached to receiver and tcp
                    // strip computer ethernet adapter name and keep computer IP Address
                    // strip receiver name and keep receiver IP Address
                    audysseyMultEQTcpSniffer = new AudysseyMultEQTcpSniffer(
                        cmbInterfaceComputer.Text.IndexOf(' ') > -1 ? cmbInterfaceComputer.Text.Substring(0, cmbInterfaceComputer.Text.IndexOf(' ')) : cmbInterfaceComputer.Text,
                        cmbInterfaceReceiver.Text.IndexOf(' ') > -1 ? cmbInterfaceReceiver.Text.Substring(0, cmbInterfaceReceiver.Text.IndexOf(' ')) : cmbInterfaceReceiver.Text,
                        0, 1256, 0, 0, AvrSnifferCallback, null, null, audysseyMultEQAvrTcp.Populate);
                    // open sniffer connection to receiver
                    audysseyMultEQTcpSniffer.Open();
                    // close TCP traffic
                    audysseyMultEQAvrTcp.Close();
                }
            }
            else
            {
                audysseyMultEQTcpSniffer.Close();
                // immediately clean up the object
                audysseyMultEQTcpSniffer = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public void AvrSnifferCallback(bool IsConnected, string Result)
        {
            audysseyMultEQAvr.SnifferAttach_IsChecked = IsConnected;
            audysseyMultEQAvr.Serialized += Result;
        }

        private void MenuItem_AvrInfo_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvr.AvrInfo_IsChecked = (bool)audysseyMultEQAvrTcp?.GetAvrInfo(OnCmdResponse);
        }

        private void MenuItem_AvrStatus_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvr.AvrStatus_IsChecked = (bool)audysseyMultEQAvrTcp?.GetAvrStatus(OnCmdResponse);
        }

        private void MenuItem_AudysseyMode_OnClick(object sender, RoutedEventArgs e)
        {
            if (audysseyMultEQAvr.AudysseyMode_IsChecked)
            {
                audysseyMultEQAvr.AudysseyMode_IsChecked = (bool)audysseyMultEQAvrTcp?.EnterAudysseyMode(OnCmdResponse);
            }
            else
            {
                audysseyMultEQAvr.AudysseyMode_IsChecked = (bool)audysseyMultEQAvrTcp?.ExitAudysseyMode(OnCmdResponse);
            }
        }

        private void MenuItem_AvrLvLm_SW1_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvr.AvrLvlm_SW1_IsChecked = (bool)audysseyMultEQAvrTcp?.StartLvLmSw1(OnCmdResponse);
        }

        private void MenuItem_AvrLvLm_SW2_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvr.AvrLvlm_SW2_IsChecked = (bool)audysseyMultEQAvrTcp?.StartLvLmSw2(OnCmdResponse);
        }

        private void MenuItem_Abort_Oprt_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvrTcp?.AbortOprt(OnCmdResponse);
        }

        private void MenuItem_SetAvrSetPosNum_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvr.SetPosNum_IsChecked = (bool)audysseyMultEQAvrTcp?.SetPosNum(OnCmdResponse);
        }

        private void MenuItem_SetAvrStartChnl_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvr.StartChnl_IsChecked = (bool)audysseyMultEQAvrTcp?.StartChnl(OnCmdResponse);
        }

        private void MenuItem_SetAvrGetRespon_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvr.GetRespon_IsChecked = (bool)audysseyMultEQAvrTcp?.GetRespon(OnCmdResponse);
        }

        private void MenuItem_SetAvrSetAmp_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvr.SetAmp_IsChecked = (bool)audysseyMultEQAvrTcp?.SetAmp(OnCmdResponse);
        }

        private void MenuItem_SetAvrSetAudy_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvr.SetAudy_IsChecked = (bool)audysseyMultEQAvrTcp?.SetAudy(OnCmdResponse);
        }

        private void MenuItem_SetAvrSetDisFil_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvr.SetDisFil_IsChecked = (bool)audysseyMultEQAvrTcp?.SetAvrSetDisFil(OnCmdResponse);
        }

        private void MenuItem_SetAvrInitCoefs_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvr.InitCoefs_IsChecked = (bool)audysseyMultEQAvrTcp?.SetAvrInitCoefs(OnCmdResponse);
        }

        private void MenuItem_SetAvrSetCoefDt_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO
            audysseyMultEQAvr.Serialized += "Not implemented\n";
        }

        private void MenuItem_AudyFinFlag_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvr.AudyFinFlag_IsChecked = (bool)audysseyMultEQAvrTcp?.AudyFinFlag(OnCmdResponseAudyFinFlag);
        }

        private void OnCmdResponseAudyFinFlag(string Response)
        {
            if (Response.Equals("ACK"))
            {
                audysseyMultEQAvr.AudyFinFlag_IsChecked = true;
            }
            else
            {
                audysseyMultEQAvr.AudyFinFlag_IsChecked = false;
                audysseyMultEQAvr.Serialized += Response + "\n";
            }
        }

        private void OnCmdResponse(string Response)
        {
            if (Response.Equals("ACK"))
            {
            }
            else
            {
                audysseyMultEQAvr.Serialized += Response + "\n";
            }
        }

        private void MenuItem_Export_ChannelResponseData_WaveFile_OnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.Description = "Select the directory to export the .wav files to.";
            folderBrowserDialog.ShowNewFolderButton = true;

            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Write_ChannelResponseData_WaveFile(folderBrowserDialog.SelectedPath);
            }
        }

        private void Write_ChannelResponseData_WaveFile(string SelectedPath)
        {
            foreach (var ch in audysseyMultEQAvr.DetectedChannels)
            {
                if (ch.Skip == false)
                {
                    foreach (var rspd in ch.ResponseData)
                    {
                        int SampleRate = 48000;
                        string FileName = SelectedPath + "\\" + rspd.Value.Length / 1024 + "k_MeasChirp_" + SampleRate + "Hz_" + ch.Channel + "_" + rspd.Key + ".wav";
                        WriteWaveFile(FileName, DoubleToFloatArray(rspd.Value), SampleRate);
                    }
                }
            }
        }

        private float[] DoubleToFloatArray(double[] response)
        {
            float[] result = new float[response.Length];
            for (int i = 0; i < response.Length / 4; i++)
            {
                result[i] = (float)response[i];
            }
            return result;
        }

        private void WriteWaveFile(string FileName, float[] WaveData, int SampleRate, int Channels = 1)
        {
            WaveFormat WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Channels);
            using (WaveFileWriter writer = new WaveFileWriter(FileName, WaveFormat))
            {
                try
                {
                    writer.WriteSamples(WaveData, 0, WaveData.Length);
                }
                catch
                {
                }
            }
        }

        private void MenuItem_Import_CurveFilterCoeff_WaveFile_OnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.Description = "Select the directory to import the .wav files from.\ncoefficient48kHz_FL_FlatCurveFilter.wav";
            folderBrowserDialog.ShowNewFolderButton = true;

            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Read_CurveFilterCoeff_WaveFile(folderBrowserDialog.SelectedPath);
            }
        }

        private void Read_CurveFilterCoeff_WaveFile(string SelectedPath)
        {
            foreach (var ch in audysseyMultEQAvr.DetectedChannels)
            {
                if (ch.Skip == false)
                {
                    ch.AudyCurveFilter = new();
                    ch.FlatCurveFilter = new();
                    string SampleRate = string.Empty;
                    string CurveFilter = string.Empty;
                    for (var SampleRateIndex = 0; SampleRateIndex < MultEQList.SampleRateList.Count; SampleRateIndex++)
                    {
                        SampleRate = MultEQList.SampleRateList[SampleRateIndex];
                        for (var CurveFilterIndex = 0; CurveFilterIndex < MultEQList.CurveFilterList.Count; CurveFilterIndex++)
                        {
                            CurveFilter = MultEQList.CurveFilterList[CurveFilterIndex];
                            string FileName = SelectedPath + "\\" + SampleRate + "_" + ch.Channel + "_" + CurveFilter + ".wav";
                            if (CurveFilterIndex == 0)
                            {
                                ch.AudyCurveFilter.Add(SampleRate, FloatToDoubleArray(ReadWaveFile(FileName, MultEQList.SampleFrequencyList[SampleRateIndex])));
                            }
                            else if (CurveFilterIndex == 1)
                            {
                                ch.FlatCurveFilter.Add(SampleRate, FloatToDoubleArray(ReadWaveFile(FileName, 48000)));
                            }
                        }
                    }
                }
            }
        }

        private double[] FloatToDoubleArray(float[] response)
        {
            double[] result = new double[response.Length];
            for (int i = 0; i < response.Length / 4; i++)
            {
                result[i] = (double)response[i];
            }
            return result;
        }

        private float[] ReadWaveFile(string FileName, int SampleRate, int Channels = 1)
        {
            WaveFormat WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Channels);

            var readback = new List<float>();
            try
            {
                using (WaveFileReader reader = new WaveFileReader(FileName))
                {
                    float[] sampleframe;
                    do
                    {
                        sampleframe = reader.ReadNextSampleFrame();
                        if (sampleframe != null)
                        {
                            readback.Add(sampleframe[0]);
                        }
                    } while (sampleframe != null);
                }
                return readback.ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, FileName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return new float[1024];
        }

        private void MenuItem_Export_ChannelResponseData_FrdFile_OnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.Description = "Select the directory to export the .frd files to.";
            folderBrowserDialog.ShowNewFolderButton = true;

            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Write_ChannelResponseData_FrdFile(folderBrowserDialog.SelectedPath);
            }
        }

        private void Write_ChannelResponseData_FrdFile(string SelectedPath)
        {
            foreach (var ch in audysseyMultEQAvr.DetectedChannels)
            {
                foreach (var rspd in ch.ResponseData)
                {
                    int SampleRate = 48000;
                    string FileName = SelectedPath + "\\" + rspd.Value.Length / 1024 + "k_MeasChirp_" + SampleRate + "Hz_" + ch.Channel + "_" + rspd.Key + ".frd";
                    WriteFrdFile(FileName, rspd.Value, SampleRate);
                }
            }
        }

        private void WriteFrdFile(string FileName, double[] WaveData, int SampleRate)
        {
            double[] Frequency = new double[WaveData.Length];
            Complex[] ComplexData = new Complex[WaveData.Length];

            for (int j = 0; j < WaveData.Length; j++)
            {
                Frequency[j] = (double)j / WaveData.Length * SampleRate;
                ComplexData[j] = (Complex)(WaveData[j]);
            }
            Frequency[0] = 0.5 / WaveData.Length * SampleRate;

            MathNet.Numerics.IntegralTransforms.Fourier.Forward(ComplexData, FourierOptions.NoScaling);

            try
            {
                StreamWriter sw = new StreamWriter(FileName, false, Encoding.ASCII);
                for (int i = 0; i < Frequency.Length / 2; i++)
                {
                    sw.WriteLine(Frequency[i].ToString(CultureInfo.InvariantCulture) + "," + ComplexData[i].Magnitude.ToString(CultureInfo.InvariantCulture) + "," + ComplexData[i].Phase.ToString(CultureInfo.InvariantCulture));
                }
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }
    }
}