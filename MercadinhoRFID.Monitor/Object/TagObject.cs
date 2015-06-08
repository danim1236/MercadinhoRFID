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
            var now = DateTime.Now;
            LTSAntenna1 = now;
            if (!_ftsAntenna1.HasValue)
                FTSAntenna1 = now;
        }
        public void DetectedAntenna2()
        {
            lock (_lock)
            {
                _count2++;
            }
            var now = DateTime.Now;
            LTSAntenna2 = now;
            if (!_ftsAntenna2.HasValue)
                FTSAntenna2 = now;
        }

        public DateTime FTSAntenna1
        {
            get { return _ftsAntenna1 ?? DateTime.MinValue; }
            set { _ftsAntenna1 = value; }
        }

        public DateTime FTSAntenna2
        {
            get { return _ftsAntenna2 ?? DateTime.MinValue; }
            set { _ftsAntenna2 = value; }
        }

        public DateTime LTSAntenna1 { get; set; }
        public DateTime LTSAntenna2 { get; set; }

        public DateTime LastTimeSeen
        {
            get { return LTSAntenna1 > LTSAntenna2 ? LTSAntenna1 : LTSAntenna2; }
        }

        public int MaxCount { get; private set; }

        public bool IsLost
        {
            get { return _count1 == 0 && _count1 == 0; }
        }

        private volatile int _count1;
        private volatile int _count2;
        private DateTime? _ftsAntenna1;
        private DateTime? _ftsAntenna2;
        private readonly object _lock = new object();


        public void CheckStatus()
        {
            lock (_lock)
            {
                if (_count1 > 0 || _count2 > 0)
                {
                    Status = _count1 > _count2 ? TagStatus.DENTRO : TagStatus.FORA;
                    MaxCount = Math.Max(_count1, _count2);
                    _count1 = _count2 = 0;
                }
            }
        }
        public TagStatus Status { get; set; }
    }
}