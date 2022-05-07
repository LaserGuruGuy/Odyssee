using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Audyssey.MultEQAvr;
using Audyssey.MultEQTcp;
using Audyssey.MultEQTcpSniffer;
using MultEQAvrAdapter.MultEQApp;

namespace Odyssee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AudysseyMultEQAvr audysseyMultEQAvr = null;
        private AudysseyMultEQAvrTcp audysseyMultEQAvrTcp = null;
        private AudysseyMultEQTcpSniffer audysseyMultEQTcpSniffer = null;
        private AudysseyMultEQAvrAdapter audysseyMultEQAvrAdapter = null;


        public MainWindow()
        {
            InitializeComponent();

            SearchForComputerIpAddress();

            audysseyMultEQAvr = new();
            audysseyMultEQAvr.PropertyChanged += PropertyChanged;
            audysseyMultEQAvr.AvrStatus.PropertyChanged += PropertyChanged;
            audysseyMultEQAvr.AvrInfo.PropertyChanged += PropertyChanged;

            audysseyMultEQAvrTcp = new(ref audysseyMultEQAvr);
            audysseyMultEQAvrAdapter = new(ref audysseyMultEQAvr);

            this.DataContext = audysseyMultEQAvr;
        }

        private static void RunAsAdmin()
        {
            try
            {
                var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                using var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path, "/run_elevated_action")
                {
                    Verb = "runas"
                });
                process?.WaitForExit();

            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 1223)
                {
                    MessageBox.Show("Run the app elevated", "Elevated rights required to capture packets from a raw socket", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    throw;
                }
            }
        }

        private static bool IsElevated()
        {
            using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);

            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        private void ComboBox_InterfaceComputer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (comboBox.Items.Count > 0)
            {
                SearchForReceiverIpAddress((comboBox.SelectedItem as ComputerDeviceInfo).IpAddress);
            }
        }

        private void ScrollViewer_Serialized_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;

            if (e.HeightChanged)
            {
                scrollViewer.ScrollToEnd();
            }
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine(e.PropertyName);
            if (e.PropertyName.Equals("SPLValue"))
            {
                InitOxyPlotLvlm();
                AddOxyPlotLvlm();
            }
            if (e.PropertyName.Equals("ChSetup"))
            {
                audysseyMultEQAvr.DetectedChannels = null;
            }
        }
    }
}
