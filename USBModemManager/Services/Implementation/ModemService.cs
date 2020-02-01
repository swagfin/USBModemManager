using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using USBModemManager.Entity;

namespace USBModemManager.Services.Implementation
{
    public class ModemService : IModemService
    {
        private SerialPort ActiveSerialPort;
        private string DataReceivedString;
        public ModemService()
        {
            ActiveSerialPort = new SerialPort();
            DataReceivedString = string.Empty;
            ActiveSerialPort.DataReceived += SerialPortDataReceivedEvent;
        }
        //Event for Receiving Data
        private void SerialPortDataReceivedEvent(object sender, SerialDataReceivedEventArgs e)
        {
            string datain = string.Empty;
            int numbytes = ActiveSerialPort.BytesToRead;
            for (int i = 1, loopTo = numbytes; i <= loopTo; i++)
                datain += (char)ActiveSerialPort.ReadChar();
            DataReceivedString += datain;
        }

        public List<COMPort> GetAvailableCOMPorts()
        {
            var all_found = new List<COMPort>();
            var searcher = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_POTSModem");
            //Loop All
            foreach (ManagementObject queryObject in searcher.Get())
            {
                if ((string)queryObject["Status"] == "OK")
                {
                    all_found.Add(new COMPort { AttachedTo = (string)queryObject["AttachedTo"], Description = (string)queryObject["Description"] });
                }
            }

            return all_found;
        }
        public Task<List<COMPort>> GetAvailableCOMPortsAsync()
        {
            return Task.Run(() =>
             {
                 return GetAvailableCOMPorts();
             });
        }
        public bool IsCOMPortOpen()
        {
            if (ActiveSerialPort.IsOpen)
                return true;
            return false;
        }
        public void DisconnectFromCOMPort()
        {
            if (IsCOMPortOpen())
                ActiveSerialPort.Close();
        }
        public bool ConnectToCOMPort(COMPort port)
        {
            //Will Close any Active or Open Port
            DisconnectFromCOMPort();
            ActiveSerialPort.PortName = port.AttachedTo;
            ActiveSerialPort.BaudRate = 9600;
            ActiveSerialPort.Parity = System.IO.Ports.Parity.None;
            ActiveSerialPort.DataBits = 8;
            ActiveSerialPort.StopBits = System.IO.Ports.StopBits.One;
            ActiveSerialPort.Handshake = System.IO.Ports.Handshake.None;
            ActiveSerialPort.RtsEnable = true;
            ActiveSerialPort.ReceivedBytesThreshold = 1;
            ActiveSerialPort.NewLine = Environment.NewLine;
            ActiveSerialPort.ReadTimeout = 1000;
            ActiveSerialPort.Open();
            return IsCOMPortOpen();
        }
        public Task<bool> ConnectToCOMPortAsync(COMPort port)
        {
            return Task.Run(() =>
           {
               return ConnectToCOMPort(port);
           });
        }

        public string GetDataReceived()
        {
            return this.DataReceivedString;
        }
        public void CleaDataReceived()
        {
            DataReceivedString = string.Empty;
        }


        public void SendCOMCommand(string command, int sleep = 1)
        {
            if (!IsCOMPortOpen())
                throw new Exception("No Active COM Port Opened");
            ActiveSerialPort.Write($"{command}{Environment.NewLine}");
            if (sleep > 0)
                Thread.Sleep(sleep);
        }


        public void GetSIMCardMessages()
        {
            CleaDataReceived();
            SendCOMCommand("AT");
            SendCOMCommand("AT+CMGF=1");
            //SendCOMCommand("AT+CPMS=\"SM\"");
            SendCOMCommand("AT+CMGL=\"ALL\"", 2000);
            //Read Current Data
            string lineoftext = GetDataReceived();
            //String separator  
            string[] stringSeparators = new string[] { "+CMGL" };
            string[] arytextfile = lineoftext.Split(stringSeparators, StringSplitOptions.None);
            foreach (string line in arytextfile)
            {
                //Expected Line
                //'+CMGL: 1,"REC UNREAD","+31628870634",,"11/01/09,10:26:26+04"
                //+CMGL: index,message_status,address,[address_text],[service_center_time_stamp][,address_type,sms_message_body_length]<CR><LF>sms_message_body[<CR><LF>+CMGL: ...]
                string pattern = "(:)|(,\")|(\",\")";
                string[] result = Regex.Split(line, pattern);
                //Lets Test
                string someTest = result[0];
            }


        }
    }
}
