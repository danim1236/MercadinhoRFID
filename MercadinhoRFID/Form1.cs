using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;
using MercadinhoRFID.Driver;

namespace MercadinhoRFID
{
    public partial class Form1 : Form
    {
        private DualTagMonitor _monitor;
        private System.Timers.Timer _timer;

        public string RootPath
        {
            get
            {
                string path = Assembly.GetExecutingAssembly().Location;
                var directory = Path.GetDirectoryName(path);
                return directory;
            }
        }

        public string LogFileName
        {
            get { return Path.Combine(RootPath, "log.txt"); }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadMonitor();
            _timer = new System.Timers.Timer(250);
            _timer.Elapsed += Elapsed;

            if (File.Exists(LogFileName))
            {
                var log = File.ReadAllLines(LogFileName);
                listBox1.Items.AddRange(log.Cast<object>().ToArray());
            }
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            Invoke(new MethodInvoker(() => dataGridView1.Refresh()));
        }


        private void LoadMonitor()
        {
            var fileName = Path.Combine(RootPath, "Resources", "dual_tag_epcs.txt");
            _monitor = new DualTagMonitor(fileName);
            _monitor.DualTagMonitorChange += DualTagMonitorChangeHandler;
            dataGridView1.DataSource = new BindingList<DualTagObject>(_monitor.DualTagsObject);
        }

        private void DualTagMonitorChangeHandler(object sender, DualTagObject args)
        {
            Invoke(new MethodInvoker(delegate
            {
                var logLine = string.Format("Item {0} está {1}", args.Id, args.Status);
                File.AppendAllLines(LogFileName, new [] {logLine});
                listBox1.Items.Add(logLine);
                listBox1.Refresh();
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _timer.Start();
            _monitor.Start();
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _monitor.Stop();
            _timer.Stop();
            button1.Enabled = true;
            button2.Enabled = false;
        }
    }
}
