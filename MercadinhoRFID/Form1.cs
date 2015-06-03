using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using MercadinhoRFID.Driver;

namespace MercadinhoRFID
{
    public partial class Form1 : Form
    {
        private DualTagMonitor _monitor;

        public string RootPath
        {
            get
            {
                string path = Assembly.GetExecutingAssembly().Location;
                var directory = Path.GetDirectoryName(path);
                return directory;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadMonitor();
        }

        private void LoadMonitor()
        {
            var fileName = Path.Combine(RootPath, "Resources", "dual_tag_epcs.txt");
            _monitor = new DualTagMonitor(fileName);
        }
    }
}
