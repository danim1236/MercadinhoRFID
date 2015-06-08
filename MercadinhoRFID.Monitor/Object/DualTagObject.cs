using System;
using System.Globalization;

namespace MercadinhoRFID.Monitor.Object
{
    public class DualTagObject
    {
        private TagStatus _lastStatus;
        public int Id { get; set; }

        public TagObject Tag1 { get; set; }
        public TagObject Tag2 { get; set; }

        public TagStatus Status
        {
            get { return Tag1.MaxCount > Tag2.MaxCount ? Tag1.Status : Tag2.Status; }
        }
        
        public DateTime LTSAntenna1 { get { return Tag1.LTSAntenna1 > Tag2.LTSAntenna1 ? Tag1.LTSAntenna1 : Tag2.LTSAntenna1; } }
        public DateTime LTSAntenna2 { get { return Tag1.LTSAntenna2 > Tag2.LTSAntenna2 ? Tag1.LTSAntenna2 : Tag2.LTSAntenna2; } }

        public DateTime FTSAntenna1 { get { return Tag1.FTSAntenna1 < Tag2.FTSAntenna1 ? Tag1.FTSAntenna1 : Tag2.FTSAntenna1; } }
        public DateTime FTSAntenna2 { get { return Tag1.FTSAntenna2 < Tag2.FTSAntenna2 ? Tag1.FTSAntenna2 : Tag2.FTSAntenna2; } }

        public TimeSpan? TempoAusente
        {
            get
            {
                if (Status == TagStatus.FORA)
                {
                    var lastTimeDentro = LTSAntenna1;
                    if (lastTimeDentro == DateTime.MinValue)
                        lastTimeDentro = FTSAntenna2;
                    return DateTime.Now.Subtract(lastTimeDentro);
                }
                return null;
            }
        }

        public bool IsPresente { get { return !Tag1.IsLost || !Tag2.IsLost; } }
        public bool IsRemovida { get { return IsPresente && (Tag1.IsLost || Tag2.IsLost); } }

        public DualTagObjectDetail[] GetDetails()
        {
            var now = DateTime.Now;
            return new []
            {
                new DualTagObjectDetail("Identificação", Id.ToString(CultureInfo.InvariantCulture)), 
                new DualTagObjectDetail("EPC Interno", Tag1.EpcLongo), 
                new DualTagObjectDetail("EPC Externo", Tag2.EpcLongo), 
                new DualTagObjectDetail("Estado Geral", IsPresente ? Status.ToString() : "Perdido"), 
                new DualTagObjectDetail("Etiqueta Int.", Tag1.IsLost ? "Perdida" : Tag1.Status.ToString()), 
                new DualTagObjectDetail("Etiqueta Int. - Dentro", Tag1.LTSAntenna1 > DateTime.MinValue ? Tag1.LTSAntenna1.Subtract(now).ToString() : string.Empty), 
                new DualTagObjectDetail("Etiqueta Int. - Fora", Tag1.LTSAntenna2 > DateTime.MinValue ? Tag1.LTSAntenna2.Subtract(now).ToString() : string.Empty), 
                new DualTagObjectDetail("Etiqueta Ext.", Tag2.IsLost ? "Perdida" : Tag2.Status.ToString()), 
                new DualTagObjectDetail("Etiqueta Ext. - Dentro", Tag2.LTSAntenna1 > DateTime.MinValue ? Tag2.LTSAntenna1.Subtract(now).ToString() : string.Empty), 
                new DualTagObjectDetail("Etiqueta Ext. - Fora", Tag2.LTSAntenna2 > DateTime.MinValue ? Tag2.LTSAntenna2.Subtract(now).ToString() : string.Empty)
            };
        }

        public bool CheckStatus()
        {
            Tag1.CheckStatus();
            Tag2.CheckStatus();
            var ret = Status != _lastStatus;
            if (ret)
            {
                _lastStatus = Status;
            }
            return ret;
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