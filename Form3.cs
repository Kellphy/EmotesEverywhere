using System;
using System.Drawing;
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
            StartPosition = FormStartPosition.CenterParent;
            label1.Text = "Select a theme or manually set your colors.";
            ShowDialog();
        }
        public override void SaveColors()
        {
            base.SaveColors();
            Invalidate();
            ClosePanel(this, new EventArgs());
        }
        public override void ColorProfiles()
        {
            base.ColorProfiles();
            label4.ForeColor = (Color)Properties.Settings.Default["Copy"];
            label5.ForeColor = (Color)Properties.Settings.Default["Error"];
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
        private void button13_Click(object sender, EventArgs e)
        {
            ColorDialog newColor = new ColorDialog();
            if (newColor.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default["Outline"] = newColor.Color;
                SaveColors();
            }
        }
        //Classic
        private void button2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Color_BG"] = Color.FromArgb(125, 133, 151);
            Properties.Settings.Default["Color_FG"] = Color.FromArgb(0, 48, 102);
            Properties.Settings.Default["Outline"] = Color.FromArgb(0, 48, 102);
            Properties.Settings.Default["Button_BG"] = Color.FromArgb(113, 122, 142);
            Properties.Settings.Default["Color_NonText"] = Color.FromArgb(92, 103, 125);
            Properties.Settings.Default["TextBox_BG"] = Color.FromArgb(151, 157, 172);
            Properties.Settings.Default["Color_Link"] = Color.FromArgb(0, 0, 255);
            Properties.Settings.Default["Color_VLink"] = Color.Purple;
            Properties.Settings.Default["Copy"] = Color.LightGreen;
            Properties.Settings.Default["Error"] = Color.DarkRed;

            SaveColors();
        }
        //Dark
        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Color_BG"] = Color.FromArgb(42, 47, 56);   //Background Color
            Properties.Settings.Default["Color_FG"] = Color.FromArgb(179, 179, 179); //Text Color
            Properties.Settings.Default["Outline"] = Color.FromArgb(179, 179, 179); //Outline Color
            Properties.Settings.Default["Button_BG"] = Color.FromArgb(30, 34, 40);  //Menu HighLight Color
            Properties.Settings.Default["Color_NonText"] = Color.FromArgb(116, 129, 152);   //Menu Hightlight Border Color
            Properties.Settings.Default["TextBox_BG"] = Color.FromArgb(56, 64, 75); //Menu Check Background Color
            Properties.Settings.Default["Color_Link"] = Color.FromArgb(166, 212, 255);
            Properties.Settings.Default["Color_VLink"] = Color.Purple;
            Properties.Settings.Default["Copy"] = Color.LightGreen;
            Properties.Settings.Default["Error"] = Color.DarkRed;

            SaveColors();
        }
        //Light
        private void button10_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Color_BG"] = Color.FromArgb(242, 242, 242);
            Properties.Settings.Default["Color_FG"] = Color.FromArgb(69, 69, 69);
            Properties.Settings.Default["Outline"] = Color.FromArgb(69, 69, 69);
            Properties.Settings.Default["Button_BG"] = Color.FromArgb(247, 247, 247);
            Properties.Settings.Default["Color_NonText"] = Color.FromArgb(96, 143, 226);
            Properties.Settings.Default["TextBox_BG"] = Color.FromArgb(225, 233, 244);
            Properties.Settings.Default["Color_Link"] = Color.FromArgb(0, 0, 255);
            Properties.Settings.Default["Color_VLink"] = Color.Purple;
            Properties.Settings.Default["Copy"] = Color.DarkGreen;
            Properties.Settings.Default["Error"] = Color.DarkRed;

            SaveColors();
        }
        // Night Blue
        private void button14_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Color_BG"] = Color.FromArgb(2, 54, 89);
            Properties.Settings.Default["Color_FG"] = Color.FromArgb(5, 140, 230);
            Properties.Settings.Default["Outline"] = Color.FromArgb(1, 39, 64);
            Properties.Settings.Default["Button_BG"] = Color.FromArgb(1, 39, 64);
            Properties.Settings.Default["Color_NonText"] = Color.FromArgb(2, 101, 166);
            Properties.Settings.Default["TextBox_BG"] = Color.FromArgb(2, 62, 102);
            Properties.Settings.Default["Color_Link"] = Color.FromArgb(1, 155, 255);
            Properties.Settings.Default["Color_VLink"] = Color.FromArgb(1, 39, 64);
            Properties.Settings.Default["Copy"] = Color.DarkGreen;
            Properties.Settings.Default["Error"] = Color.DarkRed;

            SaveColors();
        }
        //
        private void button15_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Color_BG"] = Color.FromArgb(42, 42, 42);
            Properties.Settings.Default["Color_FG"] = Color.Silver;
            Properties.Settings.Default["Outline"] = Color.FromArgb(42, 42, 42);
            Properties.Settings.Default["Button_BG"] = Color.FromArgb(53, 53, 53);
            Properties.Settings.Default["Color_NonText"] = Color.Silver;
            Properties.Settings.Default["TextBox_BG"] = Color.FromArgb(26, 26, 26);
            Properties.Settings.Default["Color_Link"] = Color.FromArgb(2, 188, 242);
            Properties.Settings.Default["Color_VLink"] = Color.FromArgb(225, 0, 225);
            Properties.Settings.Default["Copy"] = Color.FromArgb(0, 210, 210);
            Properties.Settings.Default["Error"] = Color.FromArgb(210, 105, 0);

            SaveColors();
        }
    }
}