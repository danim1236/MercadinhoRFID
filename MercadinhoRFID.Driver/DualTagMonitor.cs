using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Impinj.OctaneSdk;
using Timer = System.Timers.Timer;

namespace MercadinhoRFID.Driver
{
    public class DualTagMonitor
    {
        public int TimeThreshold = 500;
        private readonly DualTagObject[] _dualTagsObject;
        private readonly Dictionary<string, TagObject> _tagsByEpc;
        private readonly R220Continuous _driver;
        private readonly Timer _timer;

        public DualTagObject[] DualTagsObject{get { return _dualTagsObject; }}

        public R220Configuration Configuration { get; set; }

        public DualTagMonitor(string fileName)
            : this(ReadFromFile(fileName))
        {
        }

        private static DualTagObject[] ReadFromFile(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
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

        public DualTagMonitor(DualTagObject[] dualTagsObject)
        {
            Configuration = RegistryConfig.New<R220Configuration>();
            Configuration.IpAddress = "192.168.1.159";
            _dualTagsObject = dualTagsObject;
            _tagsByEpc = new Dictionary<string, TagObject>();
            foreach (var dualTagObject in _dualTagsObject)
            {
                _tagsByEpc[dualTagObject.Tag1.Epc] = dualTagObject.Tag1;
                _tagsByEpc[dualTagObject.Tag2.Epc] = dualTagObject.Tag2;
            }
            _driver = new R220Continuous(Configuration);
            _driver.TagsReported += TagsReported;
            _timer = new Timer(500);
            _timer.Elapsed += Elapsed;
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var dualTagObject in _dualTagsObject)
            {
                if (dualTagObject.CheckStatus())
                {
                    OnDualTagMonitorChange(dualTagObject);
                }
            }
        }

        public bool Start()
        {
            _timer.Start();
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
                        //tagObject.LTSAntenna1 = tag.LastSeenTime.LocalDateTime;
                        tagObject.LTSAntenna1 = DateTime.Now;
                        if (!tagObject.FTSAntenna1.HasValue)
                            tagObject.FTSAntenna1 = DateTime.Now;
                    }
                    else
                    {
                        //tagObject.LTSAntenna2 = tag.LastSeenTime.LocalDateTime;
                        tagObject.LTSAntenna2 = DateTime.Now;
                        if (!tagObject.FTSAntenna2.HasValue)
                            tagObject.FTSAntenna2 = DateTime.Now;
                    }
                }
            }
        }

        public event DualTagMonitorChange DualTagMonitorChange;

        protected virtual void OnDualTagMonitorChange(DualTagObject args)
        {
            var handler = DualTagMonitorChange;
            if (handler != null) handler(this, args);
        }
    }

    public delegate void DualTagMonitorChange(object sender, DualTagObject args);
}