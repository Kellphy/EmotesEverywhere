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
                "\n clicking the search results or drag-and-drop with RMB (best)." +
                "\n# Win+V - check clipboard history." +
                "\n# Shift+LMB - add / remove an emote to / from favorites." +
                "\n# Search \"gif\" for GIFs and \"png\" for PNGs." +
                "\n" +
                "\n# RGB is the default option, but you are free to try any of the 3:" +
                "\n0) Best - Drag-and-Drop with RMB for both transparency and gif" +
                "\n1) RGB - The background color will be Discord's dark theme." +
                "\n2) DiB - An alternative / experimental transparent background." +
                "\n3) Link - A direct link to the image.";
            ShowDialog();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}