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
            var HostName = System.Net.Dns.GetHostName();
            var HostEntry = await System.Net.Dns.GetHostEntryAsync(HostName);

            audysseyMultEQAvr.ComputerDeviceInfo = new();
            audysseyMultEQAvr.ReceiverDeviceInfo = new();

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
                                        Audyssey.MultEQAvr.ComputerDeviceInfo _ComputerDeviceInfo = new()
                                        {
                                            IpAddress = IP.ToString(),
                                            Description = nic.Description,
                                            Name = nic.Name,
                                        };
                                        audysseyMultEQAvr.ComputerDeviceInfo.Add(_ComputerDeviceInfo);
                                        cmbInterfaceComputer.SelectedItem = _ComputerDeviceInfo;
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

        /// <summary>
        /// Get receiver's IP address(es) and add to listbox, select last one by default
        /// </summary>
        private async void SearchForReceiverIpAddress(string ComputerIpAddress)
        {
            using var deviceLocator = new SsdpDeviceLocator(new Rssdp.Infrastructure.SsdpCommunicationsServer(new SocketFactory(ComputerIpAddress)));
            // Can pass search arguments here (device type, uuid). No arguments means all devices.
            var foundDevices = await deviceLocator.SearchAsync("upnp:rootdevice");

            audysseyMultEQAvr.ReceiverDeviceInfo = new();

            foreach (var foundDevice in foundDevices)
            {
                try
                {
                    //Can retrieve the full device description easily though.
                    var fullDevice = await foundDevice.GetDeviceInfo();

                    //Device data returned only contains basic device details and location of full device description.
                    Console.WriteLine("Found " + fullDevice.FriendlyName + " with " + foundDevice.Usn + " at " + foundDevice.DescriptionLocation.ToString());

                    // build avripinfo from fulldevice
                    if (fullDevice.CustomProperties.Contains("DMH:X_AudysseyPort"))
                    {
                        Audyssey.MultEQAvr.ReceiverDeviceInfo _ReceiverDeviceInfo = new()
                        {
                            Manufacturer = fullDevice.Manufacturer,
                            FriendlyName = fullDevice.FriendlyName,
                            ModelName = fullDevice.ModelName,
                            ModelNumber = fullDevice.ModelNumber,
                            SerialNumber = fullDevice.SerialNumber,
                            IpAddress = foundDevice.DescriptionLocation.Host,
                            AudysseyPort = int.Parse(fullDevice.CustomProperties["DMH:X_AudysseyPort"].Value)
                        };
                        // add receiver to list
                        audysseyMultEQAvr.ReceiverDeviceInfo.Add(_ReceiverDeviceInfo);
                    }
                }
                catch
                {
                }
            }

            // select the last item
            cmbInterfaceReceiver.SelectedIndex = cmbInterfaceReceiver.Items.Count - 1;
        }
    }
}