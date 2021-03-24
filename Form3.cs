using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KEE
{
    public partial class Form3 : Window
    {
        public event ClosePanelHandler ClosePanel;
        public delegate void ClosePanelHandler(object sender, EventArgs e);
        public Form3()
        {
            InitializeComponent();
        }
        private void Form3_Load(object sender, EventArgs e) { }
        public void Start()
        {
            RefreshWindow();
            string curFile = $"{Environment.CurrentDirectory}\\KEE.exe.config";
            if (File.Exists(curFile))
            {
                this.StartPosition = FormStartPosition.CenterParent;
                label1.Text = "Select a profile or manually set your colors.";
                this.ShowDialog();
            }
        }
        public void SaveColors()
        {
            Properties.Settings.Default.Save();
            ClosePanel(this, new EventArgs());
            ColorProfiles();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            ColorDialog newColor = new ColorDialog();
            if (newColor.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default["Color_BG"] = newColor.Color;
                SaveColors();
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            ColorDialog newColor = new ColorDialog();
            if (newColor.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default["Color_FG"] = newColor.Color;
                SaveColors();
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            ColorDialog newColor = new ColorDialog();
            if (newColor.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default["Button_BG"] = newColor.Color;
                SaveColors();
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            ColorDialog newColor = new ColorDialog();
            if (newColor.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default["Color_NonText"] = newColor.Color;
                SaveColors();
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            ColorDialog newColor = new ColorDialog();
            if (newColor.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default["TextBox_BG"] = newColor.Color;
                SaveColors();
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog newColor = new ColorDialog();
            if (newColor.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default["Color_Link"] = newColor.Color;
                SaveColors();
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            ColorDialog newColor = new ColorDialog();
            if (newColor.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default["Color_VLink"] = newColor.Color;
                SaveColors();
            }
        }
        private void button11_Click(object sender, EventArgs e)
        {
            ColorDialog newColor = new ColorDialog();
            if (newColor.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default["Copy"] = newColor.Color;
                SaveColors();
            }
        }
        private void button12_Click(object sender, EventArgs e)
        {
            ColorDialog newColor = new ColorDialog();
            if (newColor.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default["Error"] = newColor.Color;
                SaveColors();
            }
        }
        //Classic
        private void button2_Click(object sender, EventArgs e)
        {
            new Form1().DefaultColors();

            SaveColors();
        }
        //Dark
        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Color_BG"] = Color.FromArgb(42, 47, 56);   //Background Color
            Properties.Settings.Default["Color_FG"] = Color.FromArgb(179, 179, 179); //Text Color
            Properties.Settings.Default["Button_BG"] = Color.FromArgb(30, 34, 40);  //Menu HighLight Color
            Properties.Settings.Default["Color_NonText"] = Color.FromArgb(116, 129, 152);   //Menu HightlightBorderCOlor
            Properties.Settings.Default["TextBox_BG"] = Color.FromArgb(56, 64, 75); //Menu Check Background COlor
            Properties.Settings.Default["Color_Link"] = Color.FromArgb(166, 212, 255);
            Properties.Settings.Default["Color_VLink"] = Color.FromArgb(128, 0, 128);
            Properties.Settings.Default["Copy"] = Color.LightGreen;
            Properties.Settings.Default["Error"] = Color.DarkRed;

            SaveColors();
        }
        //Light
        private void button10_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Color_BG"] = Color.FromArgb(242, 242, 242);
            Properties.Settings.Default["Color_FG"] = Color.FromArgb(69, 69, 69);
            Properties.Settings.Default["Button_BG"] = Color.FromArgb(247, 247, 247);
            Properties.Settings.Default["Color_NonText"] = Color.FromArgb(96, 143, 226);
            Properties.Settings.Default["TextBox_BG"] = Color.FromArgb(225, 233, 244);
            Properties.Settings.Default["Color_Link"] = Color.FromArgb(0, 0, 255);
            Properties.Settings.Default["Color_VLink"] = Color.FromArgb(128, 0, 128);
            Properties.Settings.Default["Copy"] = Color.DarkGreen;
            Properties.Settings.Default["Error"] = Color.DarkRed;

            SaveColors();
        }
    }
}