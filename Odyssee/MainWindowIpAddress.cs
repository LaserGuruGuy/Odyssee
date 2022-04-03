using System;
using System.Windows;
using System.Net.NetworkInformation;
using Rssdp;

namespace Odyssee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Get this computer's IP address(es) and add to listbox, select last one by default
        /// </summary>
        private async void SearchForComputerIpAddress()
        {
            cmbInterfaceHost.Items.Clear();
            cmbInterfaceHost.Items.Add("Searching...");
            cmbInterfaceHost.SelectedIndex = cmbInterfaceClient.Items.Count - 1;
            var HostName = System.Net.Dns.GetHostName();
            var HostEntry = await System.Net.Dns.GetHostEntryAsync(HostName);
            cmbInterfaceHost.Items.Clear();
            if (HostEntry.AddressList.Length > 0)
            {
                foreach (System.Net.IPAddress IP in HostEntry.AddressList)
                {
                    try
                    {
                        if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                            {
                                foreach (UnicastIPAddressInformation UnicastAddress in nic.GetIPProperties().UnicastAddresses)
                                {
                                    if (UnicastAddress.Address.Equals(IP))
                                    {
                                        cmbInterfaceHost.Items.Add(IP.ToString() + " | " + nic.Description);
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                cmbInterfaceHost.SelectedIndex = cmbInterfaceHost.Items.Count - 1;
            }
        }

        /// <summary>
        /// Get receiver's IP address(es) and add to listbox, select last one by default
        /// </summary>
        private async void SearchForReceiverIpAddress(string ComputerIpAddress)
        {
            cmbInterfaceClient.Items.Clear();
            cmbInterfaceClient.Items.Add("Searching...");
            cmbInterfaceClient.SelectedIndex = cmbInterfaceClient.Items.Count - 1;
            using var deviceLocator = new SsdpDeviceLocator(new Rssdp.Infrastructure.SsdpCommunicationsServer(new SocketFactory(ComputerIpAddress)));
            // Can pass search arguments here (device type, uuid). No arguments means all devices.
            var foundDevices = await deviceLocator.SearchAsync("upnp:rootdevice");
            cmbInterfaceClient.Items.Clear();
            foreach (var foundDevice in foundDevices)
            {
                try
                {
                    //Can retrieve the full device description easily though.
                    var fullDevice = await foundDevice.GetDeviceInfo();

                    //Device data returned only contains basic device details and location of full device description.
                    Console.WriteLine("Found " + fullDevice.FriendlyName + " with " + foundDevice.Usn + " at " + foundDevice.DescriptionLocation.ToString());

                    if (fullDevice.FriendlyName.Contains("Denon AVR"))
                    {
                        cmbInterfaceClient.Items.Add(foundDevice.DescriptionLocation.Host + " | " + fullDevice.FriendlyName);
                        cmbInterfaceClient.SelectedIndex = cmbInterfaceClient.Items.Count - 1;
                    }
                }
                catch
                {
                }
            }
        }
    }
}