using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Impinj.OctaneSdk;

namespace MercadinhoRFID.Monitor.Driver
{
    public class R220Continuous : IDisposable
    {
        private static readonly List<string> ConnectedDrivers = new List<string>();

        public R220Configuration Configuration
        {
            get { return _configuration; }
        }

        public bool IsConnected { get { return _reader != null && _reader.IsConnected; } }

        private readonly R220Configuration _configuration;
        private volatile bool _readWorking;

        public static HashSet<string> EpcsToIgnore
        {
            get { return _epcsToIgnore ?? (_epcsToIgnore = new HashSet<string>()); }
        }

        private ImpinjReader _reader;
        private static HashSet<string> _epcsToIgnore;
        private string[] _continuousEpcs;

        public string[] ContinuousEpcs
        {
            get
            {
                if (_continuousEpcs == null || _continuousEpcs.Length != ContinuousTags.Count)
                {
                    _continuousEpcs = ContinuousTags.Keys.ToArray();
                }
                return _continuousEpcs;
            }
        }

        public Dictionary<string, Tag> ContinuousTags { get; set; }

        public R220Continuous(R220Configuration configuration)
        {
            _configuration = configuration;
        }

        public void MarkEpcsToIgnore(string[] epcs)
        {
            _epcsToIgnore = new HashSet<string>(epcs);
        }

        public void ResetEpcsToIgnore()
        {
            EpcsToIgnore.Clear();
        }

        private void Connect()
        {
            if (IsConnected)
                throw new InvalidOperationException("Tentando usar um driver em uso");
            var address = _configuration.IpAddress;

            ConnectedDrivers.Add(address);

            _reader = new ImpinjReader();
            _reader.Connect(address);
        }

        private void Disconnect()
        {
            if (IsConnected)
            {
                _reader.Disconnect();
                _reader = null;
                ConnectedDrivers.Remove(Configuration.IpAddress);
            }
        }

        #region [ Read ]

        private void ReaderStarted(ImpinjReader reader, ReaderStartedEvent readerStartedEvent)
        {
            _readWorking = true;
        }

        private void ReaderStopped(ImpinjReader reader, ReaderStoppedEvent e)
        {
            _readWorking = false;
        }

        public bool StartRead()
        {
            try
            {
                Connect();
                var settings = SetupForRead();
                settings.Report.Mode = ReportMode.Individual;
                _reader.ApplySettings(settings);

                _reader.TagsReported += OnContinuousReadTagsReported;
                _reader.ReaderStarted += ReaderStarted;
                _reader.ReaderStopped += ReaderStopped;
                _readWorking = true;
                _continuousEpcs = null;
                ContinuousTags = new Dictionary<string, Tag>();
                _reader.Start();
                return true;
            }
            catch (OctaneSdkException e)
            {
                Console.WriteLine("Octane SDK Exception: {0}", e.Message);
                MessageBox.Show(string.Format("{1}{0}{2}", Environment.NewLine,
                    @"Erro ao acessar o leitor. Verifique se o mesmo está ligado e conectado à rede, bem como o este computador.",
                    @"Caso esteja ligado e conectado e mesmo assim não funcione entre em contato com o suporte."),
                    "Erro de Acesso ao Leitor RFId");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                throw;
            }
        }

        private void OnContinuousReadTagsReported(ImpinjReader reader, TagReport report)
        {
            OnTagsReported(reader, report);
        }

        public bool StopRead()
        {
            try
            {
                if (IsConnected)
                {
                    _reader.Stop();

                    var init = DateTime.Now;
                    while (DateTime.Now.Subtract(init).TotalMilliseconds < 1000)
                    {
                        if (!_readWorking)
                            break;
                        Thread.Sleep(100);
                    }
                    Disconnect();
                    return true;
                }
                return false;
            }
            catch (OctaneSdkException e)
            {
                Console.WriteLine("Octane SDK Exception: {0}", e.Message);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                throw;
            }
        }

        #endregion
        
        #region [ Setup ]

        private Settings SetupForRead()
        {
            var settings = _reader.QueryDefaultSettings();
            settings.Report.IncludeAntennaPortNumber = true;
            settings.Report.IncludeCrc = true;
            settings.Report.IncludeFirstSeenTime = true;
            settings.Report.IncludeLastSeenTime = true;
            settings.Report.IncludeSeenCount = true;
            settings.Report.Mode = ReportMode.BatchAfterStop;
            settings.SearchMode = SearchMode.DualTarget;
            settings.ReaderMode = ReaderMode.AutoSetDenseReader;
            settings.TagPopulationEstimate = 50;

            //settings.Filters.Mode = TagFilterMode.OnlyFilter1;
            //settings.Filters.TagFilter1.MemoryBank = MemoryBank.Epc;
            //settings.Filters.TagFilter1.BitPointer = 0x20;
            //settings.Filters.TagFilter1.BitCount = 16;
            //settings.Filters.TagFilter1.TagMask = "41C9";
            //settings.Filters.TagFilter1.FilterOp = TagFilterOp.Match;

            SetupAntennas(settings);

            return settings;
        }

        private void SetupAntennas(Settings settings)
        {
            for (ushort i = 1; i <= settings.Antennas.Length; i++)
            {
                var antenna = settings.Antennas.GetAntenna(i);
                if (antenna.IsEnabled)
                {
                    if (Configuration.MaxRxSensitivity)
                    {
                        antenna.MaxRxSensitivity = true;
                    }
                    else
                    {
                        antenna.MaxRxSensitivity = false;
                        antenna.RxSensitivityInDbm = Configuration.RxSensitivityInDbm;
                    }
                    if (Configuration.MaxTransmitPower)
                    {
                        antenna.MaxTransmitPower = true;
                    }
                    else
                    {
                        antenna.MaxTransmitPower = false;
                        antenna.TxPowerInDbm = Configuration.TxPowerInDbm;
                    }
                }
            }
        }

        #endregion

        #region [ Events ]

        public event TagsReportedEvent TagsReported;

        protected virtual void OnTagsReported(ImpinjReader reader, TagReport report)
        {
            var handler = TagsReported;
            if (handler != null) handler(this, reader, report);
        }
        
        #endregion

        public bool IsConnectable()
        {
            if (IsConnected)
                return true;

            var isConnectable = false;
            _reader = new ImpinjReader();
            try
            {
                Connect();
                var settings = SetupForRead();
                _reader.ApplySettings(settings);
                _reader.ReaderStarted += ReaderStarted;
                _reader.ReaderStopped += ReaderStopped;

                _reader.Start();

                var init = DateTime.Now;
                while (DateTime.Now.Subtract(init).TotalMilliseconds < 1000)
                {
                    if (_readWorking)
                        break;
                    Thread.Sleep(50);
                }

                _reader.Stop();

                init = DateTime.Now;
                while (DateTime.Now.Subtract(init).TotalMilliseconds < 1000)
                {
                    if (!_readWorking)
                        break;
                    Thread.Sleep(50);
                }
                
                Disconnect();
                isConnectable = true;
            }
            catch (OctaneSdkException)
            {
                Disconnect();
            }
            return isConnectable;
        }

        public void Dispose()
        {
            _reader.Stop();
            Disconnect();
        }
    }

    public delegate void TagsReportedEvent(object sender, ImpinjReader reader, TagReport report);
}