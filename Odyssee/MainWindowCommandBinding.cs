using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Odyssee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.FileName = "Odyssee.ody";
            dlg.DefaultExt = ".ody";
            dlg.Filter = "Odyssee (*.ody)|*.ody";

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                ParseOdyFileToAudysseyMultEQAvr(dlg.FileName);
            }
        }

        private void SaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension 
            dlg.FileName = "Odyssee.ody";
            dlg.DefaultExt = ".ody";
            dlg.Filter = "Odyssee (*.ody)|*.ody";

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                ParseAudysseyMultEQAvrToOdyFile(dlg.FileName);
            }
        }

        private void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            audysseyMultEQAvr.Reset();
            PlotModel.ResetAllAxes();
        }

        private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void HandleDroppedFile(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in files)
                {
                    if (file.EndsWith(".ody"))
                    {
                        ParseOdyFileToAudysseyMultEQAvr(file);
                    }
                    else if (file.EndsWith(".wav"))
                    {
                        ParseWaveFileNameToResponseData(file);
                        ParseWaveFileNameToFilterCoeff(file);
                    }
                }
            }
        }

        private void ParseOdyFileToAudysseyMultEQAvr(string FileName)
        {
            if (File.Exists(FileName))
            {
                string Serialized = File.ReadAllText(FileName);

                JsonConvert.PopulateObject(Serialized, audysseyMultEQAvr, new JsonSerializerSettings
                {
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                });
            }
        }

        private void ParseAudysseyMultEQAvrToOdyFile(string FileName)
        {
            if (audysseyMultEQAvr != null)
            {
                string Serialized = JsonConvert.SerializeObject(audysseyMultEQAvr, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                if ((Serialized != null) && (!string.IsNullOrEmpty(FileName)))
                {
                    File.WriteAllText(FileName, Serialized);
                }
            }
        }
    }
}