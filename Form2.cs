using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KEE
{
    public partial class Form2 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse);
        public Form2()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }
        private void Form2_Load(object sender, EventArgs e) { }
        public void Start()
        {
            ColorProfiles();
            this.StartPosition = FormStartPosition.CenterParent;
            label1.Text = "# Get emotes by entering their full name in the search box" +
                "\nor by clicking the search results." +
                "\n# You can press [Enter] / [Space] instead of clicking \"Search\"." +
                "\n# Press [Win+V] to see the clipboard history." +
                "\n# If you see the emotes panel snapping to a position when scrolling, click" +
                "\non an [Option] / info button to change the window's the main focus." +
                "\n" +
                "\n# The 1st option is the default one, but you are free to try any of the 3:" +
                "\n[Option 1] - The image's background will be Discord's dark theme color." +
                "\n[Option 2] - An alternative / experimental transparent background." +
                "\n[Option 3] - A direct link to the image.";
            this.ShowDialog();
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
        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}