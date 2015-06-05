﻿using System;
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
            _monitor.DualTagMonitorLost += DualTagMonitorLostHandler;
            _monitor.DualTagMonitorIncoerente += DualTagMonitorIncoerenteHandler;
            dataGridView1.DataSource = new BindingList<DualTagObject>(_monitor.DualTagsObject);
        }

        private void DualTagMonitorChangeHandler(object sender, DualTagObject args)
        {
            Invoke(new MethodInvoker(delegate
            {
                var logLine = string.Format("{0:dd/MM/yyyy HH:mm:ss} - Item {1} está {2}", DateTime.Now, args.Id, args.Status);
                DoLog(logLine);
            }));
        }
        private void DualTagMonitorLostHandler(object sender, DualTagObject args)
        {
            Invoke(new MethodInvoker(delegate
            {
                var logLine = string.Format("{0:dd/MM/yyyy HH:mm:ss} - Item {1} foi {2}", DateTime.Now, 
                    args.Id, args.HasLost ? "Perdido" : "Encontrado");
                DoLog(logLine);
            }));
        }
        private void DualTagMonitorIncoerenteHandler(object sender, DualTagObject args)
        {
            Invoke(new MethodInvoker(delegate
            {
                var logLine = string.Format("{0:dd/MM/yyyy HH:mm:ss} - Item {1} foi {2}. {3} / {4}", DateTime.Now,
                    args.Id, args.IncoerenciaStatus ? "Incoerente" : "Coerente",
                    args.Tag1.Status, args.Tag2.Status);
                DoLog(logLine);
            }));
        }
        private void DoLog(string logLine)
        {
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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _monitor.Stop();
            _timer.Stop();
            ControlBox = true;
            button1.Enabled = true;
            button2.Enabled = false;
        }
    }
}
