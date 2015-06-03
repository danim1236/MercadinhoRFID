﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Impinj.OctaneSdk;

namespace MercadinhoRFID.Driver
{
    public class DualTagMonitor
    {
        public int TimeThreshold = 1000;
        private readonly DualTagObject[] _dualTagsObject;
        private readonly Dictionary<string, TagObject> _tagsByEpc;

        private readonly R220Continuous _driver;
        public R220Configuration Configuration { get; set; }

        public DualTagMonitor(string fileName)
            : this(ReadFromFile(fileName))
        {
        }

        private static DualTagObject[] ReadFromFile(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            return (from line in lines
                    let parts = line.Split(new[] {' ', '\t', ',', ';'}, StringSplitOptions.RemoveEmptyEntries)
                    where parts.Length == 2
                    select new DualTagObject
                    {
                        Tag1 = new TagObject
                        {
                            Epc = parts[0]
                        },
                        Tag2 = new TagObject
                        {
                            Epc = parts[1]
                        }
                    }).ToArray();
        }

        public DualTagMonitor(DualTagObject[] dualTagsObject)
        {
            Configuration = RegistryConfig.New<R220Configuration>();
            Configuration.IpAddress = "139.82.28.24";
            _dualTagsObject = dualTagsObject;
            _tagsByEpc = new Dictionary<string, TagObject>();
            foreach (var dualTagObject in _dualTagsObject)
            {
                _tagsByEpc[dualTagObject.Tag1.Epc] = dualTagObject.Tag1;
                _tagsByEpc[dualTagObject.Tag2.Epc] = dualTagObject.Tag2;
            }
            _driver = new R220Continuous(Configuration);
            _driver.TagsReported += TagsReported;
        }

        public bool Start()
        {
            return _driver.StartRead();
        }

        public bool Stop()
        {
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
                        tagObject.LTSAntenna1 = tag.LastSeenTime.LocalDateTime;
                        if (tagObject.LTSAntenna1.Subtract(tagObject.LTSAntenna2).TotalMilliseconds > TimeThreshold)
                        {
                            tagObject.Status = DualTagStatus.DENTRO;
                        }
                    }
                    else
                    {
                        tagObject.LTSAntenna2 = tag.LastSeenTime.LocalDateTime;
                        if (tagObject.LTSAntenna2.Subtract(tagObject.LTSAntenna1).TotalMilliseconds > TimeThreshold)
                        {
                            tagObject.Status = DualTagStatus.FORA;
                        }
                    }
                }
            }
            foreach (var dualTagObject in _dualTagsObject)
            {
                if (dualTagObject.Status != dualTagObject.LastStatus)
                {
                    dualTagObject.LastStatus = dualTagObject.Status;
                    OnDualTagMonitorChange(dualTagObject);
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