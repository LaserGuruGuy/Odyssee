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
            dlg.FileName = "Odyssee.aud";
            dlg.DefaultExt = ".aud";
            dlg.Filter = "Odyssee (*.aud)|*.aud";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                ParseFileToAudysseyMultEQAvr(dlg.FileName);
            }
        }

        private void SaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension 
            dlg.FileName = "Odyssee.aud";
            dlg.DefaultExt = ".aud";
            dlg.Filter = "Odyssee (.aud)|*.aud";

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                ParseAudysseyMultEQAvrToFile(dlg.FileName);
            }
        }

        private void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            audysseyMultEQAvr.Reset();
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
                    ParseFileToAudysseyMultEQAvr(file);
                }
            }
        }

        private void ParseFileToAudysseyMultEQAvr(string FileName)
        {
            if (File.Exists(FileName))
            {
                string Serialized = File.ReadAllText(FileName);

                JsonConvert.PopulateObject(Serialized, audysseyMultEQAvr, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });

                ConnectReceiver();
            }
        }
        private void ParseAudysseyMultEQAvrToFile(string FileName)
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