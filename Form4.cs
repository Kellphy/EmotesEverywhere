using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KEE
{
    public partial class Form4 : Window
    {
        public event ClosePanelHandler ClosePanel;
        public delegate void ClosePanelHandler(object sender, EventArgs e);
        public Form4()
        {
            InitializeComponent();
            Border(false);
        }
        private void Form3_Load(object sender, EventArgs e) { }
        public void Start()
        {
            RefreshWindow();
            string curFile = $"{Environment.CurrentDirectory}\\KEE.exe.config";
            if (File.Exists(curFile))
            {
                StartPosition = FormStartPosition.CenterParent;
                try
                {
                    button2_Update((bool)Properties.Settings.Default["AOT"]);
                }
                catch
                {
                    button2_Update(false);
                }
                ShowDialog();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Dispose();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            button2_Update(!(bool)Properties.Settings.Default["AOT"]);
        }
        private void button2_Update(bool option)
        {
            Properties.Settings.Default["AOT"] = option;
            Properties.Settings.Default.Save();
            ClosePanel(this, new EventArgs());

            if (option)
            {
                button2.BackColor = Color.Green;
                button2.Text = "ON";
            }
            else
            {
                button2.BackColor = Color.Red;
                button2.Text = "OFF";
            }
        }
    }
}