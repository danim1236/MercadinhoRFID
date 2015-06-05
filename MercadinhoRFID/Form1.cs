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
            _monitor.DualTagMonitorChange += DualTagMonitorChangeHandler;
        }

        private void DualTagMonitorChangeHandler(object sender, DualTagObject args)
        {
            Invoke(new MethodInvoker(delegate
            {
                listBox1.Items.Add(string.Format("Item {0} está {1}", args.Id, args.Status));
                listBox1.Refresh();
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _monitor.Start();
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _monitor.Stop();
            button1.Enabled = true;
            button2.Enabled = false;
        }
    }
}
