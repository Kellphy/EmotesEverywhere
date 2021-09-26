using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EmotesEverywhere
{
    public partial class Form4 : Window
    {
        public event ClosePanelHandler ClosePanel;
        public delegate void ClosePanelHandler(object sender, EventArgs e);
        public Form4()
        {
            InitializeComponent();
            Borderless();
        }
        private void Form3_Load(object sender, EventArgs e) { }
        public void Start()
        {
            RefreshWindow();
            StartPosition = FormStartPosition.CenterParent;
            try
            {
                button2_Update((bool)Properties.Settings.Default["AOT"]);
            }
            catch
            {
                button2_Update(false);
            }

            label4.Text = "Emotes cache version: " + (string)Properties.Settings.Default["Cache"];
            QSLabel_Refresh();

            ShowDialog();
        }

        private void QSLabel_Refresh()
        {
            if (((string)Properties.Settings.Default["Quick_Save"]).Length > 0)
            {
                label3.Text = (string)Properties.Settings.Default["Quick_Save"];
            }
            else
            {
                label3.Text = "No Quick Save Path";
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
                button2.BackColor = (Color)Properties.Settings.Default["Copy"];
                button2.Text = "ON";
            }
            else
            {
                button2.BackColor = (Color)Properties.Settings.Default["Error"];
                button2.Text = "OFF";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", $"{new Form1().temp_path}");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", $"{Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "EmotesEverywhere")}");
            }
            catch
            {
                Process.Start("explorer.exe", $"{Application.LocalUserAppDataPath}");
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Paging"] = 50;
            Properties.Settings.Default.Save();
            ClosePanel(this, new EventArgs());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Paging"] = 500;
            Properties.Settings.Default.Save();
            ClosePanel(this, new EventArgs());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Paging"] = 4;
            Properties.Settings.Default.Save();
            ClosePanel(this, new EventArgs());
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Paging"] = 5;
            Properties.Settings.Default.Save();
            ClosePanel(this, new EventArgs());
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Quick_Save"] = string.Empty;
            Properties.Settings.Default.Save();
            ClosePanel(this, new EventArgs());
            QSLabel_Refresh();
        }
    }
}