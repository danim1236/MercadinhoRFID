using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Impinj.OctaneSdk;
using MercadinhoRFID.Monitor;
using MercadinhoRFID.Monitor.Object;

namespace MercadinhoRFID
{
    public partial class Form1 : Form
    {
        private DualTagMonitor _monitor;
        private System.Timers.Timer _timer;
        public DualTagObject Current { get; private set; }
        private bool _closing;

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

        public string ConfigFileName
        {
            get { return Path.Combine(RootPath, "Resources", "cfg.txt"); }
        }

        public string LoginFileName
        {
            get { return Path.Combine(RootPath, "Resources", "login.txt"); }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel5.Size = new Size(83, 165);
            LoadMonitor();
            Current = _monitor.DualTagsObject.First();
            _timer = new System.Timers.Timer(250);

            DetalheMaquina.MainWindow = this;
            LoadLog();
            DoLog("Aplicação Iniciada");
        }

        public void ToggleSuper()
        {
            if (panel4.Visible)
            {
                panel4.Visible = false;
                panel5.Size = new Size(83, 165);
            }
            else if (FormLogin.TryLogin(LoginFileName))
            {

                panel4.Visible = true;
                panel5.Size = new Size(83, 37);
            }
        }

        private void LoadLog()
        {
            listBox1.Items.Clear();
            if (File.Exists(LogFileName))
            {
                var log = File.ReadAllLines(LogFileName);
                listBox1.Items.AddRange(log.Cast<object>().ToArray());
            }
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_closing)
            {
                Invoke(new MethodInvoker(() =>
                {
                    dataGridView1.Refresh();
                    button3.BackColor = _monitor.AlgumFora ? Color.Yellow : Color.White;
                }));
            }
        }

        private void LoadMonitor()
        {
            var tagsFileName = Path.Combine(RootPath, "Resources", "dual_tag_epcs.txt");
            _monitor = new DualTagMonitor(tagsFileName, ConfigFileName);
            _monitor.DualTagMonitorChange += DualTagMonitorChangeHandler;
            _monitor.DualTagMonitorLost += DualTagMonitorLostHandler;
            _monitor.DualTagMonitorRemocao += DualTagMonitorRemocao;
            dataGridView1.DataSource = new BindingList<DualTagObject>(_monitor.DualTagsObject);
        }

        private void DualTagMonitorChangeHandler(object sender, DualTagObject args)
        {
            Invoke(new MethodInvoker(delegate
            {
                var logLine = string.Format("Item {0} está {1}", args.Nome, args.StatusString);
                DoLog(logLine);
            }));
        }

        private void DualTagMonitorLostHandler(object sender, DualTagObject args)
        {
            Invoke(new MethodInvoker(delegate
            {
                var logLine = string.Format("Item {0} {1} foi detectado", args.Id,
                    args.IsPresente ? string.Empty : "não");
                DoLog(logLine);
            }));
        }

        private void DualTagMonitorRemocao(object sender, DualTagObject args)
        {
            Invoke(new MethodInvoker(delegate
            {
                var logLine = string.Format("Item {0}: A etiqueta {1} foi removida.", args.Id,
                    !args.Tag1.IsPresente ? "Interna" : "Externa");
                DoLog(logLine);
            }));
        }

        private void DoLog(string logLine)
        {
            logLine = WriteLog(logLine);
            listBox1.Items.Add(logLine);
            listBox1.Refresh();
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        public string WriteLog(string logLine)
        {
            logLine = string.Format("{0:dd/MM/yyyy HH:mm:ss} - {1}", DateTime.Now, logLine);
            File.AppendAllLines(LogFileName, new[] {logLine});
            return logLine;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void Start()
        {
            try
            {
                _timer.Elapsed += Elapsed;
                _timer.Start();
                _monitor.Start();
                button1.Enabled = false;
                button2.Enabled = true;
                button5.Enabled = false;
                DoLog("Aquisição Iniciada");
            }
            catch (OctaneSdkException e)
            {
                _monitor.Stop();
                _timer.Elapsed -= Elapsed;
                _timer.Stop();
                WriteLog(e.Message);
            }
            catch (SystemException e)
            {
                WriteLog(e.Message);
            }
        }

        private void Stop()
        {
            _monitor.Stop();
            _timer.Elapsed -= Elapsed;
            _timer.Stop();
            Thread.Sleep(500);
            button1.Enabled = true;
            button2.Enabled = false;
            button5.Enabled = true;
            DoLog("Aquisição Finalizada");
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dataGridView1.Rows[e.RowIndex];
            var dualTagObject = (DualTagObject) row.DataBoundItem;
            if (dualTagObject.Status == TagStatus.DENTRO)
            {
                row.DefaultCellStyle.BackColor = Color.White;
            }
            if (dualTagObject.Status == TagStatus.FORA)
            {
                row.DefaultCellStyle.BackColor = Color.Yellow;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dataGridView1.Rows[e.RowIndex];
            Current = (DualTagObject) row.DataBoundItem;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Tem certeza de que deseja remover o log?", @"Atenção!", MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {
                File.Delete(LogFileName);
                LoadLog();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var cfgAddress = new CfgAddress(_monitor.Address);
            if (cfgAddress.ShowDialog() == DialogResult.OK)
            {
                var ipAdress = cfgAddress.IpAddress;
                File.WriteAllLines(ConfigFileName, new[] {ipAdress});
                _monitor.Address = ipAdress;
            }
        }

        private void MenuDetalhe_Click(object sender, EventArgs e)
        {
            DetalheMaquina.ToggleShow();
        }

        private void MenuSuperUsuario_Click(object sender, EventArgs e)
        {
            ToggleSuper();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _closing = true;

            Stop();
            DetalheMaquina.HideForm();
            DoLog("Aplicação Finalizada");
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            // Autostart
            Start();
        }
    }
}