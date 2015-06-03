using System;

namespace MercadinhoRFID.Driver
{
    public class DualTagObject
    {
        public int Id { get; set; }

        public TagObject Tag1 { get; set; }
        public TagObject Tag2 { get; set; }

        public DualTagStatus Status
        {
            get { return Tag1.LastTimeSeen > Tag2.LastTimeSeen ? Tag1.Status : Tag2.Status; }
        }
        public DualTagStatus LastStatus { get; set; }
    }

    public class TagObject
    {
        public string Epc { get; set; }
        public DateTime LTSAntenna1 { get; set; }
        public DateTime LTSAntenna2 { get; set; }

        public DateTime LastTimeSeen
        {
            get { return LTSAntenna1 > LTSAntenna2 ? LTSAntenna1 : LTSAntenna2; }
        }

        public DualTagStatus Status { get; set; }
    }

    public enum DualTagStatus
    {
        DENTRO,
        FORA
    }
}