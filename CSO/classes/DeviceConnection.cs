using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.Globalization;

namespace CSO
{
    public class DeviceConnection
    {
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string UdpIdentifier { get; set; }

        public DeviceConnection(string name, string ipAddress, int port, string udpIdentifier)
        {
            Name = name;
            IpAddress = ipAddress;
            Port = port;
            UdpIdentifier = udpIdentifier;
        }
    }
}