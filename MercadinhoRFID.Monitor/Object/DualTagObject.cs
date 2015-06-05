using System;
using System.Globalization;

namespace MercadinhoRFID.Monitor.Object
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

        public DateTime? FTSAntenna1 { get { return Tag1.LTSAntenna1 < Tag2.LTSAntenna1 ? Tag1.LTSAntenna1 : Tag2.LTSAntenna1; } }
        public DateTime? FTSAntenna2 { get { return Tag1.LTSAntenna2 < Tag2.LTSAntenna2 ? Tag1.LTSAntenna2 : Tag2.LTSAntenna2; } }

        public TimeSpan? ForaHa
        {
            get
            {
                return Status == TagStatus.FORA
                    ? DateTime.Now.Subtract(LTSAntenna1 > DateTime.MinValue || !FTSAntenna2.HasValue? LTSAntenna1 : FTSAntenna2.Value)
                    : (TimeSpan?) null;
            }
        }

        public DateTime LastTimeSeen
        {
            get { return LTSAntenna1 > LTSAntenna2 ? LTSAntenna1 : LTSAntenna2; }
        }
        public TimeSpan? PerdidoHa { get { return IsLost ? DateTime.Now.Subtract(LastTimeSeen) : (TimeSpan?)null; } }

        public TimeSpan? RemocaoHa
        {
            get
            {
                return HasRemocao
                    ? DateTime.Now.Subtract(Lost1 ? Tag1.LastTimeSeen : Tag2.LastTimeSeen)
                    : (TimeSpan?) null;
            }
        }

        public bool Lost1 { get { return Tag1.IsLost; } }
        public bool Lost2 { get { return Tag2.IsLost; } }
        public bool IsLost { get { return Lost1 && Lost2; } }
        public bool HasLoose { get { return Lost1 || Lost2; } }
        public bool HasRemocao { get { return HasLoose && !IsLost; } }

        public bool IncoerenciaStatus{get { return Tag1.Status != Tag2.Status; }}

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

        public DualTagObjectDetail[] GetDetails()
        {
            var now = DateTime.Now;
            return new []
            {
                new DualTagObjectDetail("Identificação", Id.ToString(CultureInfo.InvariantCulture)), 
                new DualTagObjectDetail("EPC Interno", Tag1.EpcLongo), 
                new DualTagObjectDetail("EPC Externo", Tag2.EpcLongo), 
                new DualTagObjectDetail("Estado Geral", IsLost ? "Perdido" : Status.ToString()), 
                new DualTagObjectDetail("Etiqueta Int.", Tag1.IsLost ? "Perdida" : Tag1.Status.ToString()), 
                new DualTagObjectDetail("Etiqueta Int. - Dentro", Tag1.LTSAntenna1 > DateTime.MinValue ? Tag1.LTSAntenna1.Subtract(now).ToString() : string.Empty), 
                new DualTagObjectDetail("Etiqueta Int. - Fora", Tag1.LTSAntenna2 > DateTime.MinValue ? Tag1.LTSAntenna2.Subtract(now).ToString() : string.Empty), 
                new DualTagObjectDetail("Etiqueta Ext.", Tag2.IsLost ? "Perdida" : Tag2.Status.ToString()), 
                new DualTagObjectDetail("Etiqueta Ext. - Dentro", Tag2.LTSAntenna1 > DateTime.MinValue ? Tag2.LTSAntenna1.Subtract(now).ToString() : string.Empty), 
                new DualTagObjectDetail("Etiqueta Ext. - Fora", Tag2.LTSAntenna2 > DateTime.MinValue ? Tag2.LTSAntenna2.Subtract(now).ToString() : string.Empty), 
            };
        }
    }

    public class DualTagObjectDetail
    {
        public DualTagObjectDetail(string chave, string valor)
        {
            Chave = chave;
            Valor = valor;
        }

        public string Chave { get; set; }
        public string Valor { get; set; }
    }
}