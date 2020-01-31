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
        /// <summary>
        /// Gets Available USB COM Ports
        /// </summary>
        /// <returns></returns>
        List<COMPort> GetAvailableCOMPorts();
        /// <summary>
        /// Gets Available COMPorts Async
        /// </summary>
        /// <returns></returns>
        Task<List<COMPort>> GetAvailableCOMPortsAsync();

        /// <summary>
        /// Checks if there is an Open Connection in the Instance
        /// </summary>
        /// <returns></returns>
        bool IsCOMPortOpen();
        /// <summary>
        /// Closes a Current Open COM Port that has Active Connection
        /// </summary>
        void CloseCOMPortConnection();
        /// <summary>
        /// Connects to the COM Port Provided
        /// </summary>
        /// <param name="port">The COMPort Information</param>
        /// <returns></returns>
        bool ConnectToCOMPort(COMPort port);
        /// <summary>
        /// Connects to COM Port Async
        /// </summary>
        /// <param name="port">The COMPort Information</param>
        /// <returns></returns>
        Task<bool> ConnectToCOMPortAsync(COMPort port);
    }
}
