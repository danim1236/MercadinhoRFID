using System;
using System.Globalization;

namespace MercadinhoRFID.Monitor.Object
{
    public class DualTagObject
    {
        private TagStatus _lastStatus;
        public int Id { get; set; }

        public string Nome { get; set; }

        public TagObject Tag1 { get; set; }
        public TagObject Tag2 { get; set; }

        public TagStatus Status
        {
            get { return Tag1.MaxCount > Tag2.MaxCount ? Tag1.Status : Tag2.Status; }
        }

        public string StatusString
        {
            get
            {
                switch (Status)
                {
                    case TagStatus.DENTRO:
                        return "PRESENTE";
                    case TagStatus.FORA:
                        return "AUSENTE";
                    default:
                        return string.Empty;
                }
            }
        }

        public DateTime LastTimeDentro { get { return Tag1.LastTimeDentro > Tag2.LastTimeDentro ? Tag1.LastTimeDentro : Tag2.LastTimeDentro; } }
        public DateTime LastTimeFora { get { return Tag1.LastTimeFora > Tag2.LastTimeFora ? Tag1.LastTimeFora : Tag2.LastTimeFora; } }

        public DateTime FirstTimeDentro { get { return Tag1.FirstTimeDentro < Tag2.FirstTimeDentro ? Tag1.FirstTimeDentro : Tag2.FirstTimeDentro; } }
        public DateTime FirstTimeFora { get { return Tag1.FirstTimeFora < Tag2.FirstTimeFora ? Tag1.FirstTimeFora : Tag2.FirstTimeFora; } }

        public string TempoAusente
        {
            get
            {
                if (Status == TagStatus.FORA)
                {
                    var lastTimeDentro = LastTimeDentro;
                    if (lastTimeDentro == DateTime.MinValue)
                        lastTimeDentro = FirstTimeFora;
                    var tempoAusente = DateTime.Now.Subtract(lastTimeDentro);

                    return string.Format("{0} {1:00}:{2:00}:{3:00}", tempoAusente.Days, tempoAusente.Hours,tempoAusente.Minutes, tempoAusente.Seconds);
                }
                return null;
            }
        }

        public bool IsPresente { get { return Tag1.IsPresente|| Tag2.IsPresente; } }
        public bool IsRemovida { get { return Tag1.IsPresente ^ Tag2.IsPresente; } }

        public DualTagObjectDetail[] GetDetails()
        {
            return new []
            {
                new DualTagObjectDetail("Identificação", Id.ToString(CultureInfo.InvariantCulture)), 
                new DualTagObjectDetail("EPC Interno", Tag1.EpcLongo), 
                new DualTagObjectDetail("EPC Externo", Tag2.EpcLongo), 
                new DualTagObjectDetail("Estado Geral", IsPresente ? Status.ToString() : "Perdido"), 
                new DualTagObjectDetail("Estado Tag1", IsPresente ? Tag1.Status.ToString() : "Perdido"), 
                new DualTagObjectDetail("Contagem Tag1 - Dentro", Tag1.Count1.ToString(CultureInfo.InvariantCulture)), 
                new DualTagObjectDetail("Contagem Tag1 - Fora", Tag1.Count2.ToString(CultureInfo.InvariantCulture)), 
                new DualTagObjectDetail("Last Tag1 - Dentro", Tag1.LastTimeDentro.ToString("T")), 
                new DualTagObjectDetail("Last Tag1 - Fora", Tag1.LastTimeFora.ToString("T")), 
                new DualTagObjectDetail("Estado Tag2", IsPresente ? Tag2.Status.ToString() : "Perdido"), 
                new DualTagObjectDetail("Contagem Tag2 - Dentro", Tag2.Count1.ToString(CultureInfo.InvariantCulture)), 
                new DualTagObjectDetail("Contagem Tag2 - Fora", Tag2.Count2.ToString(CultureInfo.InvariantCulture)),
                new DualTagObjectDetail("Last Tag2 - Dentro", Tag2.LastTimeDentro.ToString("T")), 
                new DualTagObjectDetail("Last Tag2 - Fora", Tag2.LastTimeFora.ToString("T"))
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