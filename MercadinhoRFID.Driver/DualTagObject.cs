using System;

namespace MercadinhoRFID.Driver
{
    public class DualTagObject
    {
        public int Id { get; set; }

        public TagObject Tag1 { get; set; }
        public TagObject Tag2 { get; set; }

        public TagStatus Status
        {
            get { return Tag1.LastTimeSeen > Tag2.LastTimeSeen ? Tag1.Status : Tag2.Status; }
        }

        private TagStatus _lastStatus;

        public DualTagObject()
        {
            _lastStatus = TagStatus.INDEFINIDO;
        }

        public DateTime LTSAntenna1 { get { return Tag1.LTSAntenna1 > Tag2.LTSAntenna1 ? Tag1.LTSAntenna1 : Tag2.LTSAntenna1; } }
        public DateTime LTSAntenna2 { get { return Tag1.LTSAntenna2 > Tag2.LTSAntenna2 ? Tag1.LTSAntenna2 : Tag2.LTSAntenna2; } }

        public bool Lost1 { get { return Tag1.IsLost; } }
        public bool Lost2 { get { return Tag2.IsLost; } }
        public bool HasLost{get { return Lost1 || Lost2; }}

        public bool CheckStatus()
        {
            var status = Status;
            var statusHasChanged = _lastStatus != status;
            if (statusHasChanged)
            {
                _lastStatus = status;
            }
            return statusHasChanged;
        }
    }

    public class TagObject
    {
        public static int StatusChangeThreshold = 1000;
        public static int LostThreshold = 5*60*1000;
        public string Epc { get; set; }
        public DateTime LTSAntenna1 { get; set; }
        public DateTime LTSAntenna2 { get; set; }

        public DateTime LastTimeSeen
        {
            get { return LTSAntenna1 > LTSAntenna2 ? LTSAntenna1 : LTSAntenna2; }
        }
        
        public bool IsLost
        {
            get { return DateTime.Now.Subtract(LastTimeSeen).TotalMilliseconds > LostThreshold; }
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

    public enum TagStatus
    {
        INDEFINIDO,
        DENTRO,
        FORA
    }
}