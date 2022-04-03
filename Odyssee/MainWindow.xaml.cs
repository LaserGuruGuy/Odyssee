using System;
using System.Collections.Generic;
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

namespace Odyssee
{
	public class Test
    {
		public string Serialized { get; set; } = "DataContext bound debug window";
	}

	public partial class MainWindow : Window
    {
		Test test;

        public MainWindow()
        {
            InitializeComponent();

			SearchForComputerIpAddress();

			test = new();

			this.DataContext = test;
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

        private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
			var scrollViewer = sender as ScrollViewer;

			if (e.HeightChanged)
			{
				scrollViewer.ScrollToEnd();
			}
		}

        private void cmbInterfaceHost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			ComboBox comboBox = sender as ComboBox;

			if (comboBox.Items.Count > 0)
            {
				string selectedHost = (string)comboBox.SelectedItem;
				selectedHost = selectedHost.Substring(0, selectedHost.IndexOf('|')).TrimEnd(' ');

				SearchForReceiverIpAddress(selectedHost);
			}
		}
	}
}
