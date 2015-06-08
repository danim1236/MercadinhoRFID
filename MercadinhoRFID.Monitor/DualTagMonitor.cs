using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Impinj.OctaneSdk;
using MercadinhoRFID.Monitor.Driver;
using MercadinhoRFID.Monitor.Object;

namespace MercadinhoRFID.Monitor
{
    public class DualTagMonitor
    {
        public string Address { get; set; }
        public int TimeThreshold = 500;
        private readonly DualTagObject[] _dualTagsObject;
        private readonly Dictionary<string, TagObject> _tagsByEpc;
        private readonly R220Continuous _driver;
        private readonly Timer _timer;

        public DualTagObject[] DualTagsObject
        {
            get { return _dualTagsObject; }
        }

        public R220Configuration Configuration { get; set; }
        public bool AlgumFora { get { return _dualTagsObject.Any(_ => _.Status == TagStatus.FORA); } }

        public DualTagMonitor(string tagsFileName, string cfgFileName)
            : this(ReadFromFile(tagsFileName), LoadAddress(cfgFileName))
        {
        }

        private static DualTagObject[] ReadFromFile(string tagsFileName)
        {
            return LoadTags(tagsFileName);
        }

        private static string LoadAddress(string cfgFileName)
        {
            string result = null;
            if (File.Exists(cfgFileName))
            {
                result =
                    File.ReadAllLines(cfgFileName).Select(_ => _.Trim()).FirstOrDefault(_ => !string.IsNullOrEmpty(_));
            }
            return result ?? "192.168.1.159";
        }

        private static DualTagObject[] LoadTags(string tagsFileName)
        {
            var lines = File.ReadAllLines(tagsFileName);
            int count = 1;
            return (from line in lines
                let parts = line.Split(new[] {' ', '\t', ',', ';'}, StringSplitOptions.RemoveEmptyEntries)
                where parts.Length == 2
                select new DualTagObject
                {
                    Id = count++,
                    Tag1 = new TagObject
                    {
                        Epc = parts[0].Replace("-", "")
                    },
                    Tag2 = new TagObject
                    {
                        Epc = parts[1].Replace("-", "")
                    }
                }).ToArray();
        }

        public DualTagMonitor(DualTagObject[] dualTagsObject, string ipAddress)
        {
            Configuration = RegistryConfig.New<R220Configuration>();
            Address = ipAddress;
            Configuration.IpAddress = ipAddress;
            _dualTagsObject = dualTagsObject;
            _tagsByEpc = new Dictionary<string, TagObject>();
            foreach (var dualTagObject in _dualTagsObject)
            {
                _tagsByEpc[dualTagObject.Tag1.Epc] = dualTagObject.Tag1;
                _tagsByEpc[dualTagObject.Tag2.Epc] = dualTagObject.Tag2;
            }
            _presente = _dualTagsObject.ToDictionary(_ => _.Id, _ => false);
            _remocao = _dualTagsObject.ToDictionary(_ => _.Id, _ => false);
            _driver = new R220Continuous(Configuration);
            _driver.TagsReported += TagsReported;
            _timer = new Timer(1000);
            _timer.Elapsed += Elapsed;
        }

        private readonly Dictionary<int, bool> _presente;
        private readonly Dictionary<int, bool> _remocao;

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var dualTagObject in _dualTagsObject)
            {
                if (dualTagObject.CheckStatus())
                {
                    OnDualTagMonitorChange(dualTagObject);
                }
                if (_presente[dualTagObject.Id] != dualTagObject.IsPresente)
                {
                    _presente[dualTagObject.Id] = dualTagObject.IsPresente;
                    OnDualTagMonitorLost(dualTagObject);
                    OnDualTagMonitorChange(dualTagObject);
                }
                if (_remocao[dualTagObject.Id] != dualTagObject.IsRemovida)
                {
                    _remocao[dualTagObject.Id] = dualTagObject.IsRemovida;
                    OnDualTagMonitorRemocao(dualTagObject);
                }
            }
        }

        public bool Start()
        {
            _timer.Start();
            _driver.Configuration.IpAddress = Address;
            return _driver.StartRead();
        }

        public bool Stop()
        {
            _timer.Stop();
            return _driver.StopRead();
        }

        private void TagsReported(object sender, ImpinjReader reader, TagReport report)
        {
            foreach (var tag in report.Tags)
            {
                var epc = tag.Epc.ToHexString();
                if (_tagsByEpc.ContainsKey(epc))
                {
                    var tagObject = _tagsByEpc[epc];
                    if (tag.AntennaPortNumber == 1)
                    {
                        tagObject.DetectedAntenna1();
                    }
                    else
                    {
                        tagObject.DetectedAntenna2();
                    }
                }
            }
        }

        public event DualTagMonitorChange DualTagMonitorChange;
        public event DualTagMonitorChange DualTagMonitorLost;
        public event DualTagMonitorChange DualTagMonitorIncoerente;
        public event DualTagMonitorChange DualTagMonitorRemocao;

        protected virtual void OnDualTagMonitorChange(DualTagObject args)
        {
            var handler = DualTagMonitorChange;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnDualTagMonitorLost(DualTagObject args)
        {
            var handler = DualTagMonitorLost;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnDualTagMonitorIncoerente(DualTagObject args)
        {
            var handler = DualTagMonitorIncoerente;
            if (handler != null) handler(this, args);
        }
        protected virtual void OnDualTagMonitorRemocao(DualTagObject args)
        {
            var handler = DualTagMonitorRemocao;
            if (handler != null) handler(this, args);
        }
    }

    public delegate void DualTagMonitorChange(object sender, DualTagObject args);
}