using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Threading.Tasks;
using USBModemManager.Entity;

namespace USBModemManager.Services.Implementation
{
    public class ModemService : IModemService
    {
        private SerialPort ActiveSerialPort;
        public ModemService()
        {
            ActiveSerialPort = new SerialPort();
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

        public void CloseCOMPortConnection()
        {
            if (IsCOMPortOpen())
                ActiveSerialPort.Close();
        }
        public bool ConnectToCOMPort(COMPort port)
        {
            //Will Close any Active or Open Port
            CloseCOMPortConnection();
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


    }
}
