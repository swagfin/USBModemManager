using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using USBModemManager.Services;
using USBModemManager.Services.Implementation;

namespace USBModemManager.UnitTests
{
    [TestClass]
    public class ModemServiceTests
    {


        [TestMethod]
        public void CanReturnActivePorts()
        {
            IModemService modemService = new ModemService();
            var all_ports = modemService.GetAvailableCOMPorts();
            Assert.IsTrue(all_ports.Count() > 0, "No Available Ports Found!");
        }


        [TestMethod]
        public void CanConnectToActivePort()
        {
            IModemService modemService = new ModemService();
            var all_ports = modemService.GetAvailableCOMPorts();
            var first_port = all_ports.FirstOrDefault();
            bool connected = modemService.ConnectToCOMPort(first_port);
            modemService.DisconnectFromCOMPort();
            Assert.IsTrue(connected, $"Unable to Connect to Port Selected COM: {first_port.AttachedTo} {first_port.Description}");
        }


        [TestMethod]
        public void CanRetrieveMessagePort()
        {
            IModemService modemService = new ModemService();
            var all_ports = modemService.GetAvailableCOMPorts();
            var first_port = all_ports.FirstOrDefault();
            bool connected = modemService.ConnectToCOMPort(first_port);
            modemService.GetSIMCardMessages();
            modemService.DisconnectFromCOMPort();
            Assert.IsTrue(connected, $"Unable to Connect to Port Selected COM: {first_port.AttachedTo} {first_port.Description}");
        }



    }
}
