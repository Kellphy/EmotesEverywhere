using System;
using System.Windows.Forms;

namespace KEE
{
    public partial class Form2 : Window
    {
        public Form2()
        {
            InitializeComponent();
            Border(false);
        }
        private void Form2_Load(object sender, EventArgs e) { }
        public void Start()
        {
            RefreshWindow();
            StartPosition = FormStartPosition.CenterParent;
            label1.Text =
                "# Find emotes by entering their full name in the search box," +
                "\n clicking the search results or drag-&-drop with RMB (best)." +
                "\n# Press [Win+V] to see the clipboard history." +
                "\n# Search \"gif\" for GIFs and \"png\" for PNGs." +
                "\n" +
                "\n# RGB is the default option, but you are free to try any of the 3:" +
                "\nRGB - The image's background will be Discord's dark theme color." +
                "\nDiB - An alternative / experimental transparent background." +
                "\nLink - A direct link to the image." +
                "\nBest results - Drag & Drop the emote with RMB for both transparency and gif";
            ShowDialog();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}