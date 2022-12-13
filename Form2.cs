using System;
using System.Windows.Forms;

namespace EmotesEverywhere
{
    public partial class Form2 : Window
    {
        public Form2()
        {
            InitializeComponent();
            Borderless();
        }
        private void Form2_Load(object sender, EventArgs e) { }
        public void Start()
        {
            RefreshWindow();
            StartPosition = FormStartPosition.CenterParent;
            label1.Text =
                "- Search emotes and use them by clicking the search results," +
                "\n or by using the Drag and Drop with RMB (best)." +
                "\n- Win+V - check clipboard history." +
                "\n- Shift+LMB - add / remove an emote to / from favorites." +
                "\n- Search \"gif\" for GIFs and \"png\" for PNGs." +
                "\n" +
                "\n- Try any of the 4 available options of copying to clipboard:" +
                "\n0) Best - Drag-and-Drop with RMB for both transparency and gif" +
                "\n1) RGB - The background color will be Discord's dark theme." +
                "\n2) DiB - An alternative / experimental transparent background." +
                "\n3) Link - The direct link to the image." +
                "\n5) File - The file containing the image.";
            ShowDialog();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}