using Audyssey.MultEQ.List;
using Audyssey.MultEQTcp;
using Audyssey.MultEQTcpSniffer;
using MathNet.Numerics.IntegralTransforms;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using System.Windows;

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
            folderBrowserDialog.Description = "Select folder to export .wav files";
            folderBrowserDialog.ShowNewFolderButton = true;

            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ParseResponseDataToWaveFilePath(folderBrowserDialog.SelectedPath);
            }
        }

        private void MenuItem_Export_ChannelResponseData_FrdFile_OnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.Description = "Select folder to export .frd files";
            folderBrowserDialog.ShowNewFolderButton = true;

            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ParseResponseDataToFrdFilePath(folderBrowserDialog.SelectedPath);
            }
        }

        private void MenuItem_Import_ChannelResponseData_WaveFile_OnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.Description = "Select folder to import .wav files";
            folderBrowserDialog.ShowNewFolderButton = true;

            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string[] FileNames = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.wav");
                foreach(var FileName in FileNames)
                {
                    ParseWaveFileNameToResponseData(FileName);
                }
            }
        }

        private void MenuItem_Import_CurveFilterCoeff_WaveFile_OnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.Description = "Select folder to import .wav files";
            folderBrowserDialog.ShowNewFolderButton = true;

            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ParseWaveFilePathToFilterCoeff(folderBrowserDialog.SelectedPath);
            }
        }

        private void MenuItem_Export_FilterCoefficients_WaveFile_OnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.Description = "Select folder to export .wav files";
            folderBrowserDialog.ShowNewFolderButton = true;

            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ParseFilterCoeffToWaveFilePath(folderBrowserDialog.SelectedPath);
            }
        }

        private void ParseWaveFilePathToFilterCoeff(string FilePath)
        {
            foreach (var ch in audysseyMultEQAvr.DetectedChannels)
            {
                if (ch.Skip == false)
                {
                    foreach (var SampleRate in MultEQList.SampleRateList)
                    {
                        foreach (var CurveFilter in MultEQList.CurveFilterList)
                        {
                            string FileName = FilePath + "\\" + SampleRate + "_" + CurveFilter + "_" + ch.Channel + ".wav";
                            if (CurveFilter.Equals(MultEQList.CurveFilterList[0]))
                            {
                                if (ch.AudyCurveFilter.ContainsKey(SampleRate))
                                {
                                    if (ch.AudyCurveFilter.Remove(SampleRate))
                                    {
                                        ch.AudyCurveFilter.Add(SampleRate, FloatToDoubleArray(ReadWaveFile(FileName, MultEQList.SampleFrequencyList[MultEQList.SampleRateList.IndexOf(SampleRate)])));
                                    }
                                }
                                else
                                {
                                    ch.AudyCurveFilter.Add(SampleRate, FloatToDoubleArray(ReadWaveFile(FileName, MultEQList.SampleFrequencyList[MultEQList.SampleRateList.IndexOf(SampleRate)])));
                                }
                            }
                            else if (CurveFilter.Equals(MultEQList.CurveFilterList[1]))
                            {
                                if (ch.FlatCurveFilter.ContainsKey(SampleRate))
                                {
                                    if (ch.FlatCurveFilter.Remove(SampleRate))
                                    {
                                        ch.FlatCurveFilter.Add(SampleRate, FloatToDoubleArray(ReadWaveFile(FileName, MultEQList.SampleFrequencyList[MultEQList.SampleRateList.IndexOf(SampleRate)])));
                                    }
                                }
                                else
                                {
                                    ch.FlatCurveFilter.Add(SampleRate, FloatToDoubleArray(ReadWaveFile(FileName, MultEQList.SampleFrequencyList[MultEQList.SampleRateList.IndexOf(SampleRate)])));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ParseResponseDataToWaveFilePath(string FilePath)
        {
            const int SampleRate = 48000;
            foreach (var ch in audysseyMultEQAvr.DetectedChannels)
            {
                if (ch.Skip == false)
                {
                    foreach (var rspd in ch.ResponseData)
                    {
                        string FileName = FilePath + "\\" + rspd.Value.Length + "_ImpulseResponse_" + SampleRate + "_" + ch.Channel + "_" + rspd.Key + "_" + ch.ChannelReport.Delay + ".wav";
                        WriteWaveFile(FileName, DoubleToFloatArray(rspd.Value), SampleRate);
                    }
                }
            }
        }

        private void ParseResponseDataToFrdFilePath(string FilePath)
        {
            const int SampleRate = 48000;
            foreach (var ch in audysseyMultEQAvr.DetectedChannels)
            {
                if (ch.Skip == false)
                {
                    foreach (var rspd in ch.ResponseData)
                    {
                        string FileName = FilePath + "\\" + rspd.Value.Length + "_ImpulseResponse_" + SampleRate + "_" + ch.Channel + "_" + rspd.Key + ".frd";
                        WriteFrdFile(FileName, rspd.Value, SampleRate);
                    }
                }
            }
        }

        private void ParseFilterCoeffToWaveFilePath(string FilePath)
        {
            foreach (var ch in audysseyMultEQAvr.DetectedChannels)
            {
                if (ch.Skip == false)
                {
                    foreach (var curve in ch.AudyCurveFilter)
                    {
                        foreach (var samplerate in MultEQList.SampleRateList)
                        {
                            if (samplerate.Equals(curve.Key))
                            {
                                string FileName = FilePath + "\\" + curve.Key + "_" + MultEQList.CurveFilterList[0] + "_" + ch.Channel + ".wav";
                                WriteWaveFile(FileName, DoubleToFloatArray(curve.Value), MultEQList.SampleFrequencyList[MultEQList.SampleRateList.IndexOf(curve.Key)]);
                            }
                        }
                    }
                    foreach (var curve in ch.FlatCurveFilter)
                    {
                        foreach (var samplerate in MultEQList.SampleRateList)
                        {
                            if (samplerate.Equals(curve.Key))
                            {
                                string FileName = FilePath + "\\" + curve.Key + "_" + MultEQList.CurveFilterList[1] + "_" + ch.Channel + ".wav";
                                WriteWaveFile(FileName, DoubleToFloatArray(curve.Value), MultEQList.SampleFrequencyList[MultEQList.SampleRateList.IndexOf(curve.Key)]);
                            }
                        }
                    }
                }
            }
        }

        private void ParseWaveFileNameToResponseData(string FileName)
        {
            if (File.Exists(FileName))
            {
                string[] keywords = System.IO.Path.GetFileNameWithoutExtension(FileName).Split('_');
                if ((keywords.Length == 6 || keywords.Length == 5) && keywords[1] == "ImpulseResponse")
                {
                    int SampleSize = 0;
                    int SampleRate = 0;
                    string Channel = string.Empty;
                    string Position = string.Empty;
                    decimal? Delay = null;

                    try
                    {
                        SampleSize = int.Parse(keywords[0]);
                        SampleRate = int.Parse(keywords[2]);
                        Channel = keywords[3];
                        Position = keywords[4];
                        Delay = decimal.Parse(keywords[5]);
                    }
                    catch
                    {

                    }

                    double[] Data = FloatToDoubleArray(ReadWaveFile(FileName, SampleRate));

                    try
                    {
                        if (Data.Length == SampleSize)
                        {
                            if (audysseyMultEQAvr.DetectedChannels != null)
                            {
                                foreach (var ch in audysseyMultEQAvr.DetectedChannels)
                                {
                                    if (ch.Channel.Equals(Channel))
                                    {
                                        ch.ChannelReport.Delay = Delay;
                                        if (ch.ResponseData.ContainsKey(Position))
                                        {
                                            ch.ResponseData.Remove(Position);
                                        }
                                        ch.ResponseData.Add(Position, Data);
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void ParseWaveFileNameToFilterCoeff(string FileName)
        {
            if (File.Exists(FileName))
            {
                string[] keywords = System.IO.Path.GetFileNameWithoutExtension(FileName).Split('_');
                if (keywords.Length == 3)
                {
                    foreach (var ch in audysseyMultEQAvr.DetectedChannels)
                    {
                        if (ch.Channel.Equals(keywords[2]))
                        {
                            foreach (var curve in MultEQList.CurveFilterList)
                            {
                                if (curve.Equals(keywords[1]))
                                {
                                    foreach (var samplerate in MultEQList.SampleRateList)
                                    {
                                        if (samplerate.Equals(keywords[0]))
                                        {
                                            double[] Coefficients = FloatToDoubleArray(ReadWaveFile(FileName, MultEQList.SampleFrequencyList[MultEQList.SampleRateList.IndexOf(samplerate)]));
                                            if (curve.Equals(MultEQList.CurveFilterList[0]))
                                            {
                                                if (ch.AudyCurveFilter.ContainsKey(samplerate))
                                                {
                                                    if (ch.AudyCurveFilter.Remove(samplerate))
                                                    {
                                                        ch.AudyCurveFilter.Add(samplerate, Coefficients);
                                                    }
                                                }
                                                else
                                                {
                                                    ch.AudyCurveFilter.Add(samplerate, Coefficients);
                                                }
                                            }
                                            else if (curve.Equals(MultEQList.CurveFilterList[1]))
                                            {
                                                if (ch.FlatCurveFilter.ContainsKey(samplerate))
                                                {
                                                    if (ch.FlatCurveFilter.Remove(samplerate))
                                                    {
                                                        ch.FlatCurveFilter.Add(samplerate, Coefficients);
                                                    }
                                                }
                                                else
                                                {
                                                    ch.FlatCurveFilter.Add(samplerate, Coefficients);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
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

        private double[] FloatToDoubleArray(float[] response)
        {
            double[] result = new double[response.Length];
            for (int i = 0; i < response.Length / 4; i++)
            {
                result[i] = (double)response[i];
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

        private float[] ReadWaveFile(string FileName, int SampleRate)
        {
            var readback = new List<float>();
            try
            {
                using (WaveFileReader reader = new WaveFileReader(FileName))
                {
                    if ((reader.WaveFormat.Channels == 1) &&
                        (reader.WaveFormat.BitsPerSample == 32) &&
                        (reader.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat) &&
                        (reader.WaveFormat.SampleRate == SampleRate) &&
                        ((reader.SampleCount == MultEQList.SampleCountList[0]) || (reader.SampleCount == MultEQList.SampleCountList[1])))
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
                    else
                    {
                        string Message = "Channels: " + reader.WaveFormat.Channels +
                                         "\nBitsPerSample: " + reader.WaveFormat.BitsPerSample +
                                         "\nEncoding: " + reader.WaveFormat.Encoding.ToString() +
                                         "\nSampleRate: " + reader.WaveFormat.SampleRate +
                                         "\nSampleCount: " + reader.SampleCount;
                        MessageBox.Show(Message, FileName.Substring(FileName.LastIndexOf('\\')), MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, FileName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return readback.ToArray();
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