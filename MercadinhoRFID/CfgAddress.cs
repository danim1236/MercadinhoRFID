using System.Windows.Forms;

namespace MercadinhoRFID
{
    public partial class CfgAddress : Form
    {
        public string IpAddress { get { return textBox1 != null ? textBox1.Text : string.Empty; } }

        public CfgAddress(string ipAdress)
        {
            InitializeComponent();
            textBox1.Text = ipAdress;
        }
    }
}
