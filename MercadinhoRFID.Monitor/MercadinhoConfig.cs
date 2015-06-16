using System;
using System.Collections.Generic;
using System.Data;

namespace MercadinhoRFID.Monitor
{
    public class MercadinhoConfig
    {
        public string IpAddress { get; set; }
        public bool IsSingleSensor { get; set; }

        public MercadinhoConfig(string[] lines)
        {
            GetIpAdress(lines);
            GetIsSingleSensor(lines);
        }

        public MercadinhoConfig()
        {
            IpAddress = "192.168.1.159";
            IsSingleSensor = false;
        }

        private void GetIsSingleSensor(string[] lines)
        {
            try
            {
                var values = new Dictionary<string, bool>
                {
                    {"SINGLE", true},
                    {"DUAL", false}
                };
                IsSingleSensor = values[lines[1].Trim()];
            }
            catch
            {
                IsSingleSensor = false;
            }
        }

        private void GetIpAdress(string[] lines)
        {
            try
            {
                IpAddress = lines[0].Trim();
                var parts = IpAddress.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < 4; i++)
                {
                    var a = int.Parse(parts[i]);
                    if (a < 0 || a > 255)
                        throw new VersionNotFoundException();
                }
            }
            catch
            {
                IpAddress = "192.168.1.159";
            }
        }
    }
}