using System;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;

namespace MercadinhoRFID
{
    public partial class DetalheMaquina : Form
    {
        public static Form1 MainWindow { get; set; }

        public static DetalheMaquina Instance
        {
            get { return _instance ?? (_instance = new DetalheMaquina()); }
        }

        private System.Timers.Timer _timer;
        private static DetalheMaquina _instance;

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
            _timer = new System.Timers.Timer(250);
            _timer.Elapsed += Elapsed;
            _timer.Start();
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            Invoke(new MethodInvoker(() =>
            {
                dataGridView2.DataSource = MainWindow.Current.GetDetails();
            }));
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
