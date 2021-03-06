using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DiscordCopy
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
            this.StartPosition = FormStartPosition.CenterParent;
            label1.Text = "# Get emotes by entering their full name in the search box" +
                "\nor by clicking the search results." +
                "\n# You can press [Enter] / [Space] instead of clicking \"Search\"." +
                "\n# Press [Win+V] to see the clipboard history." +
                "\n# If you see the emotes panel snapping to a position when scrolling, click" +
                "\non an [Option] / info button to change the window's the main focus.";
            this.ShowDialog();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}