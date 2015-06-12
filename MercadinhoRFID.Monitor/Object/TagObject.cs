using System;

namespace MercadinhoRFID.Monitor.Object
{
    public class TagObject
    {
        public static int StatusChangeThreshold = 500;
        public static int LostThreshold = 10*1000;
        public string Epc { get; set; }

        public string EpcLongo
        {
            get
            {
                var result = string.Empty;
                for (int i = 0; i < Epc.Length; i++)
                {
                    if (i > 0 && i % 4 == 0)
                        result += '-';
                    result += Epc[i];
                }
                return result;
            }
        }

        public void DetectedAntenna1()
        {
            lock (_lock)
            {
                _count1++;
            }
        }
        public void DetectedAntenna2()
        {
            lock (_lock)
            {
                _count2++;
            }
        }

        public DateTime LastTimeDentro { get; private set; }
        public DateTime LastTimeFora { get; private set; }
        public DateTime FirstTimeDentro
        {
            get { return _ftsAntenna1 ?? DateTime.MinValue; }
        }

        public DateTime FirstTimeFora
        {
            get { return _ftsAntenna2 ?? DateTime.MinValue; }
        }

        public DateTime LastTimeSeen
        {
            get { return LastTimeDentro > LastTimeFora ? LastTimeDentro : LastTimeFora; }
        }

        public int MaxCount { get; private set; }

        public bool IsPresente { get; private set; }

        private volatile int _count1;
        private volatile int _count2;
        private DateTime? _ftsAntenna1;
        private DateTime? _ftsAntenna2;
        private readonly object _lock = new object();


        public void CheckStatus()
        {
            lock (_lock)
            {
                IsPresente = _count1 > 0 || _count2 > 0;
                if (IsPresente)
                {
                    if (_count1 > _count2)
                    {
                        Status = TagStatus.DENTRO;
                        LastTimeDentro = DateTime.Now;
                        if (!_ftsAntenna1.HasValue)
                            _ftsAntenna1 = DateTime.Now;
                    }
                    else
                    {
                        Status = TagStatus.FORA;
                        LastTimeFora = DateTime.Now;
                        if (!_ftsAntenna2.HasValue)
                            _ftsAntenna2 = DateTime.Now;
                    }
                    MaxCount = Math.Max(_count1, _count2);
                    Count1 = _count1;
                    Count2 = _count2;
                    _count1 = _count2 = 0;
                }
            }
        }
        public TagStatus Status { get; set; }

        public int Count1 { get; private set; }
        public int Count2 { get; private set; }
    }
}