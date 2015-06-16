using System.Windows.Forms;

namespace MercadinhoRFID
{
    public partial class CfgAddress : Form
    {
        public string IpAddress
        {
            get { return textBox1 != null ? textBox1.Text : string.Empty; }
        }

        public bool IsSengleSensor
        {
            get { return radioButtonSingle.Checked; }
        }

        public CfgAddress(string ipAdress, bool isSingleSensor)
        {
            InitializeComponent();
            textBox1.Text = ipAdress;
            if (isSingleSensor)
                radioButtonSingle.Checked = true;
            else
                radioButtonDual.Checked = true;
        }
    }
}