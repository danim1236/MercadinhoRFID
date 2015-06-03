using System;
using System.Collections.Generic;
using System.Linq;
using Impinj.OctaneSdk;
using PucRio.Cetuc.Ldhs.Rfid.Tools;

namespace PucRio.Cetuc.Ldhs.Rfid.Drivers.Model
{
    public class R220Configuration
    {
        [RegistryProperty(Default = "SpeedwayR-11-22-A7")]
        public string IpAddress { get; set; }

        [RegistryProperty(Default = (int)Impinj.OctaneSdk.ReaderMode.AutoSetDenseReader)]
        public int ReaderMode { get; set; }

        [RegistryProperty(Default = (int) Impinj.OctaneSdk.SearchMode.DualTarget)]
        public int SearchMode { get; set; }

        [RegistryProperty(Default = true)]
        public bool Ant1 { get; set; }

        [RegistryProperty(Default = true)]
        public bool Ant2 { get; set; }
        
        [RegistryProperty(Default = false)]
        public bool Ant3 { get; set; }
        
        [RegistryProperty(Default = false)]
        public bool Ant4 { get; set; }

        [RegistryProperty(Default = 2)]
        public int NumAnt { get; set; }

        [RegistryProperty(Default = 10000)]
        public int ReadDuration { get; set; }

        [RegistryProperty(Default = 1000)]
        public int ReadDurationQuick { get; set; }

        [RegistryProperty(Default = false)]
        public bool MaxTransmitPower { get; set; }

        [RegistryProperty(Default = 30)]
        public double TxPowerInDbm { get; set; }

        [RegistryProperty(Default = true)]
        public bool MaxRxSensitivity { get; set; }

        [RegistryProperty]
        public double RxSensitivityInDbm { get; set; }

        public static List<Mode> GetAllReaderModes()
        {
            return (from object value in Enum.GetValues(typeof (ReaderMode))
                    let s = value.ToString()
                    orderby s
                    select new Mode {Id = (int) value, Name = s}).ToList();
        }

        public static List<Mode> GetAllSearchModes()
        {
            return (from object value in Enum.GetValues(typeof (SearchMode))
                    let s = value.ToString()
                    orderby s
                    select new Mode {Id = (int) value, Name = s}).ToList();
        }
    }
}