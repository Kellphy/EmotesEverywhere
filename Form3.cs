using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KEE
{
    public partial class Form3 : Form
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
            ColorProfiles();
            string curFile = $"{Environment.CurrentDirectory}\\KEE.exe.config";
            if (File.Exists(curFile))
            {
                this.StartPosition = FormStartPosition.CenterParent;
                label1.Text = "Select a profile or manually set your colors.";
                this.ShowDialog();
            }
        }
        public void ColorProfiles()
        {
            Color color_bg, color_fg, button_bg;
            string curFile = $"{Environment.CurrentDirectory}\\KEE.exe.config";
            if (File.Exists(curFile))
            {
                color_bg = (Color)Properties.Settings.Default["Color_BG"];
                color_fg = (Color)Properties.Settings.Default["Color_FG"];
                button_bg = (Color)Properties.Settings.Default["Button_BG"];
            }
            else
            {
                color_bg = new Form1().color_bg;
                color_fg = new Form1().color_fg;
                button_bg = new Form1().button_bg;
            }

            this.BackColor = color_bg;
            for (int ix = this.Controls.Count - 1; ix >= 0; ix--)
            {
                if (this.Controls[ix] is Button)
                {
                    this.Controls[ix].BackColor = button_bg;
                    this.Controls[ix].ForeColor = color_fg;
                }
                else if (this.Controls[ix] is Label)
                {
                    this.Controls[ix].ForeColor = color_fg;
                }
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

        private void button2_Click(object sender, EventArgs e)
        {
            new Form1().DefaultColors();

            SaveColors();
        }
        private void button10_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Color_BG"] = Color.FromArgb(242, 242, 242);
            Properties.Settings.Default["Color_FG"] = Color.FromArgb(69, 69, 69);
            Properties.Settings.Default["Button_BG"] = Color.FromArgb(247, 247, 247);
            Properties.Settings.Default["Color_NonText"] = Color.FromArgb(96, 143, 226);
            Properties.Settings.Default["TextBox_BG"] = Color.FromArgb(225, 233, 244);
            Properties.Settings.Default["Color_Link"] = Color.FromArgb(0, 0, 255);
            Properties.Settings.Default["Color_VLink"] = Color.FromArgb(128, 0, 128);

            SaveColors();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Color_BG"] = Color.FromArgb(42, 47, 56);   //Background Color
            Properties.Settings.Default["Color_FG"] = Color.FromArgb(179, 179, 179); //Text Color
            Properties.Settings.Default["Button_BG"] = Color.FromArgb(30, 34, 40);  //Menu HighLight Color
            Properties.Settings.Default["Color_NonText"] = Color.FromArgb(116, 129, 152);   //Menu HightlightBorderCOlor
            Properties.Settings.Default["TextBox_BG"] = Color.FromArgb(56, 64, 75); //Menu Check Background COlor
            Properties.Settings.Default["Color_Link"] = Color.FromArgb(166, 212, 255);
            Properties.Settings.Default["Color_VLink"] = Color.FromArgb(128, 0, 128);

            SaveColors();
        }
    }
}