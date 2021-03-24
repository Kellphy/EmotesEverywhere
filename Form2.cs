using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KEE
{
    public partial class Form2 : Window
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
            Border(false);
        }
        private void Form2_Load(object sender, EventArgs e) { }
        public void Start()
        {
            RefreshWindow();
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
        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}