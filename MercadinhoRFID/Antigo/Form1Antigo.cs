﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;
using MercadinhoRFID.Monitor;
using MercadinhoRFID.Monitor.Object;

namespace MercadinhoRFID.Antigo
{
    public partial class DetalheMaquina : Form
    {
        private DualTagMonitor _monitor;
        private System.Timers.Timer _timer;
        private DualTagObject _current;

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

        public DetalheMaquina()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadMonitor();
            _current = _monitor.DualTagsObject.First();
            _timer = new System.Timers.Timer(250);
            _timer.Elapsed += Elapsed;

            LoadLog();
            DoLog("Aplicação Iniciada");
            dataGridView2.DataSource = new[]
            {
                new DualTagObjectDetail("Clique uma linha acima ", "para exibir detalhes")
            };
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
            Invoke(new MethodInvoker(() =>
            {
                dataGridView1.Refresh();
                if (_monitor.AlgumPerdido)
                {
                    button3.BackColor = Color.Red;
                }
                else if (_monitor.AlgumFora)
                {
                    button3.BackColor = Color.Yellow;
                }
                else
                {
                    button3.BackColor = Color.White;
                }
                dataGridView2.DataSource = _current.GetDetails();
            }));
        }


        private void LoadMonitor()
        {
            var tagsFileName = Path.Combine(RootPath, "Resources", "dual_tag_epcs.txt");
            _monitor = new DualTagMonitor(tagsFileName, ConfigFileName);
            _monitor.DualTagMonitorChange += DualTagMonitorChangeHandler;
            _monitor.DualTagMonitorLost += DualTagMonitorLostHandler;
            _monitor.DualTagMonitorIncoerente += DualTagMonitorIncoerenteHandler;
            _monitor.DualTagMonitorRemocao += DualTagMonitorRemocao;
            dataGridView1.DataSource = new BindingList<DualTagObject>(_monitor.DualTagsObject);
        }

        private void DualTagMonitorChangeHandler(object sender, DualTagObject args)
        {
            Invoke(new MethodInvoker(delegate
            {
                var logLine = string.Format("Item {0} está {1}", args.Id, args.Status);
                DoLog(logLine);
            }));
        }
        private void DualTagMonitorLostHandler(object sender, DualTagObject args)
        {
            Invoke(new MethodInvoker(delegate
            {
                var logLine = string.Format("Item {0} foi {1}", args.Id, args.HasLoose ? "Perdido" : "Encontrado");
                DoLog(logLine);
            }));
        }
        private void DualTagMonitorIncoerenteHandler(object sender, DualTagObject args)
        {
            Invoke(new MethodInvoker(delegate
            {
                var logLine = string.Format("Item {0} foi {1}. {2} / {3}", args.Id, args.IncoerenciaStatus ? "Incoerente" : "Coerente",
                    args.Tag1.Status, args.Tag2.Status);
                DoLog(logLine);
            }));
        }
        private void DualTagMonitorRemocao(object sender, DualTagObject args)
        {
            Invoke(new MethodInvoker(delegate
            {
                var logLine = string.Format("Item {0}: A etiqueta {1} foi removida.", args.Id, args.Lost1 ? "Interna" : "Externa");
                DoLog(logLine);
            }));
        }
        private void DoLog(string logLine)
        {
            logLine = string.Format("{0:dd/MM/yyyy HH:mm:ss} - {1}", DateTime.Now, logLine);
            File.AppendAllLines(LogFileName, new[] {logLine});
            listBox1.Items.Add(logLine);
            listBox1.Refresh();
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ControlBox = false;
            _timer.Start();
            _monitor.Start();
            button1.Enabled = false;
            button2.Enabled = true;
            button5.Enabled = false;
            DoLog("Aquisição Iniciada");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _monitor.Stop();
            _timer.Stop();
            ControlBox = true;
            button1.Enabled = true;
            button2.Enabled = false;
            button5.Enabled = true;
            DoLog("Aquisição Finalizada");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            DoLog("Aplicação Finalizada");
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
            if (dualTagObject.IsLost)
            {
                row.DefaultCellStyle.BackColor = Color.Red;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dataGridView1.Rows[e.RowIndex];
            _current = (DualTagObject)row.DataBoundItem;
            dataGridView2.DataSource = _current.GetDetails();
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
                File.WriteAllLines(ConfigFileName, new [] {ipAdress});
                _monitor.Address = ipAdress;
            }
        }
    }
}