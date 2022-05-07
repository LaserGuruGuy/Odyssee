﻿using System;
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

            cmbInterfaceComputer.Items.Clear();
            
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
                                        cmbInterfaceComputer.Items.Add(IP.ToString() + " | " + nic.Description);
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                cmbInterfaceComputer.SelectedIndex = cmbInterfaceComputer.Items.Count - 1;
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

            cmbInterfaceReceiver.SelectedIndex = cmbInterfaceReceiver.Items.Count - 1;

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
                    if (fullDevice.Manufacturer.Equals("Denon") || fullDevice.Manufacturer.Equals("Marantz"))
                    {
                        Audyssey.MultEQAvr.ReceiverDeviceInfo _ReceiverDeviceInfo = new()
                        {
                            Manufacturer = fullDevice.Manufacturer,
                            FriendlyName = fullDevice.FriendlyName,
                            ModelName = fullDevice.ModelName,
                            ModelNumber = fullDevice.ModelNumber,
                            SerialNumber = fullDevice.SerialNumber,
                            IpAddress = foundDevice.DescriptionLocation.Host,
                            Port = fullDevice.CustomProperties.Contains("DMH:X_AudysseyPort") ? int.Parse(fullDevice.CustomProperties["DMH:X_AudysseyPort"].Value) : 1256
                        };
                        // add receiver to list
                        audysseyMultEQAvr.ReceiverDeviceInfo.Add(_ReceiverDeviceInfo);
                    }
                }
                catch
                {
                }
            }

            // select the last added item
            cmbInterfaceReceiver.SelectedIndex = cmbInterfaceReceiver.Items.Count - 1;
        }
    }
}