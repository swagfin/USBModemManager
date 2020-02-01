using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBModemManager.Entity;

namespace USBModemManager.Services
{
    public interface IModemService
    {
        bool ConnectToCOMPort(COMPort port);
        Task<bool> ConnectToCOMPortAsync(COMPort port);
        void DisconnectFromCOMPort();
        List<COMPort> GetAvailableCOMPorts();
        Task<List<COMPort>> GetAvailableCOMPortsAsync();
        string GetDataReceived();
        void CleaDataReceived();
        void GetSIMCardMessages();
        bool IsCOMPortOpen();
        void SendCOMCommand(string command, int sleep = 1);
    }
}
