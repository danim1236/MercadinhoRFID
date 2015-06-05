using System;

namespace MercadinhoRFID.Monitor.Object
{
    public class TagObject
    {
        public static int StatusChangeThreshold = 500;
        public static int LostThreshold = 10*1000;
        public string Epc { get; set; }
        public DateTime LTSAntenna1 { get; set; }
        public DateTime LTSAntenna2 { get; set; }

        public DateTime? FTSAntenna1 { get; set; }
        public DateTime? FTSAntenna2 { get; set; }

        public DateTime LastTimeSeen
        {
            get { return LTSAntenna1 > LTSAntenna2 ? LTSAntenna1 : LTSAntenna2; }
        }
        
        public bool IsLost
        {
            get { return LastTimeSeen > DateTime.MinValue && DateTime.Now.Subtract(LastTimeSeen).TotalMilliseconds > LostThreshold; }
        }

        private TagStatus _status;
        public TagStatus Status
        {
            get
            {
                if (LTSAntenna1.Subtract(LTSAntenna2).TotalMilliseconds > StatusChangeThreshold)
                {
                    _status = TagStatus.DENTRO;
                }
                else if (LTSAntenna2.Subtract(LTSAntenna1).TotalMilliseconds > StatusChangeThreshold)
                {
                    _status = TagStatus.FORA;
                }
                return _status;
            }
        }
    }
}