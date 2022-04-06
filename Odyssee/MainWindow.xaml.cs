using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public MainWindow()
        {
            InitializeComponent();

			SearchForComputerIpAddress();

            audysseyMultEQAvr = new();
            audysseyMultEQAvr.PropertyChanged += AudysseyHomeTabAvr_PropertyChanged;

            this.DataContext = audysseyMultEQAvr;
        }

        private void AudysseyHomeTabAvr_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("BlaBla"))
            {
            }
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
                    System.Windows.MessageBox.Show("Run the app elevated.", "Elevated rights required to capture packets from a raw socket.", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void HandleDroppedFile(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

				foreach (var file in files)
				{
					// TODO: Implementation of Open
				}
			}
		}

		private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			//TODO: Implementation of Open
		}

		private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			//TODO: Implementation of Save
		}

		private void SaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			//TODO: Implementation of saveAs
		}

		private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.Close();
		}

		private void OnButtonClick_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ScrollViewer_Serialized_SizeChanged(object sender, SizeChangedEventArgs e)
        {
			var scrollViewer = sender as ScrollViewer;

			if (e.HeightChanged)
			{
				scrollViewer.ScrollToEnd();
			}
		}

        private void ComboBox_InterfaceComputer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			ComboBox comboBox = sender as ComboBox;

			if (comboBox.Items.Count > 0)
            {
				string selectedHost = (string)comboBox.SelectedItem;
				selectedHost = selectedHost.Substring(0, selectedHost.IndexOf('|')).TrimEnd(' ');

				SearchForReceiverIpAddress(selectedHost);
			}
		}

        private void ConnectReceiverSniffer()
        {
            // connection to receiver requested
            if (connectReceiver.IsChecked)
            {
                // if there is no IP address text
                if (string.IsNullOrEmpty(cmbInterfaceReceiver.Text))
                {
                    // uncheck the menu item
                    connectReceiver.IsChecked = false;
                    // display message to report error to user
                    MessageBox.Show("Please enter receiver IP address.");
                }
                else
                {
                    // receiver object does not exist
                    if (audysseyMultEQAvrTcp == null)
                    {
                        // create receiver tcp instance and strip receiver name and keep receiver IP Address
                        audysseyMultEQAvrTcp = new AudysseyMultEQAvrTcp(ref audysseyMultEQAvr, cmbInterfaceReceiver.Text.IndexOf(' ') > -1 ? cmbInterfaceReceiver.Text.Substring(0, cmbInterfaceReceiver.Text.IndexOf(' ')) : cmbInterfaceReceiver.Text);
                        // open connection to receiver
                        audysseyMultEQAvrTcp.Open();
                    }
                    // attach sniffer
                    if (connectSniffer.IsChecked)
                    {
                        // sniffer must be elevated to capture raw packets
                        if (!IsElevated())
                        {
                            // we cannot create the sniffer...
                            connectSniffer.IsChecked = false;
                            // but we can ask the user to elevate the program!
                            RunAsAdmin();
                        }
                        else
                        {
                            // sniffer object does not exist
                            if (audysseyMultEQTcpSniffer == null)
                            {
                                // create sniffer attached to receiver and tcp
                                // strip computer ethernet adapter name and keep computer IP Address
                                // strip receiver name and keep receiver IP Address
                                audysseyMultEQTcpSniffer = new AudysseyMultEQTcpSniffer(
                                    cmbInterfaceComputer.Text.IndexOf(' ') > -1 ? cmbInterfaceComputer.Text.Substring(0, cmbInterfaceComputer.Text.IndexOf(' ')) : cmbInterfaceComputer.Text,
                                    cmbInterfaceReceiver.Text.IndexOf(' ') > -1 ? cmbInterfaceReceiver.Text.Substring(0, cmbInterfaceReceiver.Text.IndexOf(' ')) : cmbInterfaceReceiver.Text,
                                    0, 1256, 0, 0, audysseyMultEQAvrTcp.Log, null, null, audysseyMultEQAvrTcp.Populate);
                                // open connection to receiver
                                audysseyMultEQTcpSniffer.Open();
                            }
                        }
                    }
                    else
                    {
                        if (audysseyMultEQTcpSniffer != null)
                        {
                            audysseyMultEQTcpSniffer.Close();
                            // immediately clean up the object
                            audysseyMultEQTcpSniffer = null;
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }
                    }
                }
            }
            else
            {
                if (audysseyMultEQTcpSniffer != null)
                {
                    audysseyMultEQTcpSniffer.Close();
                    // immediately clean up the object
                    audysseyMultEQTcpSniffer = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                if (audysseyMultEQAvrTcp != null)
                {
                    audysseyMultEQAvrTcp.Close();
                    // immediately clean up the object
                    audysseyMultEQAvrTcp = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                // if we want to connect to the sniffer...
                if (connectSniffer.IsChecked)
                {
                    // we cannot create the sniffer...
                    connectSniffer.IsChecked = false;
                    // because sniffer must be elevated to capture raw packets
                    if (!IsElevated())
                    {
                        // but we can ask the user to elevate the program!
                        RunAsAdmin();
                    }
                    else
                    {
                        // or we cannot connect because the receiver was not connected to begin with
                        System.Windows.MessageBox.Show("Connnect to receiver IP address.", "Could not attach sniffer to receiver.", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void OnButtonClick_Connect(object sender, RoutedEventArgs e)
        {
            connectReceiver.IsChecked = !connectReceiver.IsChecked;
            ConnectReceiverSniffer();
        }

        private void OnButtonClick_Inspector(object sender, RoutedEventArgs e)
        {
            connectSniffer.IsChecked = !connectSniffer.IsChecked;
            ConnectReceiverSniffer();
        }

        private void OnButtonClick_Microphone(object sender, RoutedEventArgs e)
		{

		}

		private void OnButtonClick_Speaker(object sender, RoutedEventArgs e)
		{

		}

        private void DetectReceiver_OnClick(object sender, RoutedEventArgs e)
        {
            SearchForReceiverIpAddress(cmbInterfaceComputer.Text.IndexOf(' ') > -1 ? cmbInterfaceComputer.Text.Substring(0, cmbInterfaceComputer.Text.IndexOf(' ')) : cmbInterfaceComputer.Text);
        }

        private void ConnectReceiver_OnClick(object sender, RoutedEventArgs e)
        {
            ConnectReceiverSniffer();
        }

        private void ConnectSniffer_OnClick(object sender, RoutedEventArgs e)
        {
            ConnectReceiverSniffer();
        }

        private void getReceiverInfo_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvrTcp.GetAvrInfo();
        }

        private void getReceiverStatus_OnClick(object sender, RoutedEventArgs e)
        {
            audysseyMultEQAvrTcp.GetAvrStatus();
        }

        private void ConnectAudyssey_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void setAvrLvLm_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void setAvrAbortOprt_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void setAvrSetPosNum_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void setAvrStartChnl_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void setAvrGetRespon_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void setAvrSetAmp_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void setAvrSetAudy_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void setAvrSetDisFil_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void setAvrInitCoefs_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void setAvrSetCoefDt_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void setAudysseyFinishedFlag_OnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
