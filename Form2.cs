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
            label1.Text = "# Find emotes by entering their full name in the search box," +
                "\n clicking the search results or drag-and-drop with RMB (best)." +
                "\n# Press [Win+V] to see the clipboard history." +
                "\n# Search \"gif\" for GIFs and \"png\" for PNGs." +
                "\n" +
                "\n# The 1st option is the default one, but you are free to try any of the 3:" +
                "\n[Option 1] - The image's background will be Discord's dark theme color." +
                "\n[Option 2] - An alternative / experimental transparent background." +
                "\n[Option 3] - A direct link to the image." +
                "\n[Best] - Drag and drop the emote with RMB for both transparency and gif";
            ShowDialog();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}