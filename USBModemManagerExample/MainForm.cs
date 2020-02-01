using System;
using System.Windows.Forms;
using USBModemManager.Entity;
using USBModemManager.Services;
using USBModemManager.Services.Implementation;

namespace USBModemManagerExample
{
    public partial class MainForm : Form
    {
        IModemService modemService;
        public MainForm()
        {
            //You can Inject the service if u prefer DI
            modemService = new ModemService();
            InitializeComponent();
            RefreshComs();
        }


        public void AppendStatus(string status)
        {
            StatusBox.Text += status + Environment.NewLine;
        }

        private async void RefreshComs()
        {
            try
            {
                AppendStatus("Getting Available Ports....");
                ComPortsComboBox.DisplayMember = "Description";
                ComPortsComboBox.ValueMember = "AttachedTo";
                ComPortsComboBox.DataSource = await modemService.GetAvailableCOMPortsAsync();
                ConnectBtn.Enabled = true;
                AppendStatus("DONE");
            }
            catch (Exception ex)
            {
                AppendStatus($"ERROR: " + ex.Message);
                ConnectBtn.Enabled = false;
            }

        }

        private void RefreshLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RefreshComs();
        }

        private async void ConnectBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectBtn.Enabled = false;
                LoadMessagesBtn.Enabled = false;
                DisconnectBtn.Enabled = false;

                if (ComPortsComboBox.SelectedItem == null)
                    throw new Exception("Select Valid COM Port");
                COMPort portt = (COMPort)ComPortsComboBox.SelectedItem;
                AppendStatus($"Connecting to PORT: {portt.AttachedTo} {portt.Description}");
                //Try Connect
                await modemService.ConnectToCOMPortAsync(portt);
                AppendStatus($"CONNECTED to PORT: {portt.AttachedTo} {portt.Description}");
                ConnectBtn.Enabled = true;
                LoadMessagesBtn.Enabled = true;
                DisconnectBtn.Enabled = true;
            }
            catch (Exception ex)
            {
                AppendStatus($"ERROR: " + ex.Message);
                LoadMessagesBtn.Enabled = false;
                DisconnectBtn.Enabled = false;


            }
        }

        private void LoadMessagesBtn_Click(object sender, EventArgs e)
        {
            try
            {
                LoadMessagesBtn.Enabled = false;
                AppendStatus($"Refreshing messages....");
                modemService.GetSIMCardMessages();
                AppendStatus($"DONE");
                LoadMessagesBtn.Enabled = true;
            }
            catch (Exception ex)
            {
                LoadMessagesBtn.Enabled = true;
                AppendStatus($"ERROR: " + ex.Message);
            }
        }

        private void DisconnectBtn_Click(object sender, EventArgs e)
        {
            modemService.DisconnectFromCOMPort();
            AppendStatus($"DISCONNECTED");
        }
    }
}
